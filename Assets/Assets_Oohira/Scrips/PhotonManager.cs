using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public GameObject player0;
    [SerializeField] public GameObject player1;
    [SerializeField] public GameObject ball;

    public enum PhotonState
    {
        INIT,
        CONNECTED,
        IN_LOBBY,
        IN_GAME,
        DISCONNECTED,
        WAITING,
    }

    #region Singleton
    static PhotonManager _instance = null;

    public static bool IsEmpty
    {
        get { return _instance == null; }
    }

    public static PhotonManager Instance
    {
        get
        {
            if (_instance == null)
            {
                System.Type type = typeof(PhotonManager);
                _instance = GameObject.FindObjectOfType(type) as PhotonManager;
            }

            return _instance;
        }
    }
    #endregion

    const int MAX_PLAYER_IN_ROOM = 2;
    private List<RoomInfo> CachedRoomList = null;
    public PhotonState State;

    static public bool IsOnline { get { return _instance && _instance.State == PhotonState.IN_GAME; } }
    
    public struct UserParam
    {
        public string Name;
    };

    UserParam _mySelf;
    UserParam[] _player = new UserParam[MAX_PLAYER_IN_ROOM];

    public UserParam Me { get => _mySelf; }
    public UserParam GetPlayer(int id)
    {
        return _player[id];
    }

    private void Start()
    {
        Connect();
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
    }

    /// <summary>
    /// 接続開始(オフライン時は使用しない)
    /// </summary>
    public void Connect()
    {
        Debug.Log("Connect");

        //FPS調整
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;

        PhotonNetwork.NickName = "test";
        PhotonNetwork.ConnectUsingSettings();

        State = PhotonState.WAITING;
    }

    public void LeaveRoom()
    {
        Debug.Log("LeaveRoom");
        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        State = PhotonState.CONNECTED;
    }

    private void Update()
    {
#if DEBUG
        if (Input.GetKeyDown(KeyCode.N))
        {
            CachedRoomList.ForEach(info =>
            {
                Debug.Log(info.CustomProperties["UserName"]);
                Debug.Log(info.Name);
            });
            Debug.Log(PhotonNetwork.CountOfPlayers);
        }
#endif

        switch (State)
        {
            //接続開始
            case PhotonState.CONNECTED:
                JoinLobby();
                State = PhotonState.WAITING;
                break;

            //ロビーで部屋選択
            case PhotonState.IN_LOBBY:
                {
                    //ルームリストもらうまで間があるので待機する
                    if (CachedRoomList == null) break;

                    //いまのロビーにいる人のルームリストをもらい、もらったルームからマッチング相手を探す
                    if (CheckAndJoinRoom())
                    {
                        State = PhotonState.WAITING;
                    }
                    else
                    {
                        CreateRoom();
                        State = PhotonState.IN_GAME;
                        //StartCoroutine(Ready(3f));
                    }
                }
                break;

            //ゲーム中
            case PhotonState.IN_GAME:
                break;
        }
    }

    void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby:" + PhotonNetwork.CurrentLobby.Name);

        RoomOptions roomOptions = new RoomOptions();

        //カスタムプロパティ
        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        prop["GUID"] = Guid.NewGuid().ToString();
        prop["Name"] = "test";
        PhotonNetwork.SetPlayerCustomProperties(prop);

        _mySelf.Name = "test";
        State = PhotonState.IN_LOBBY;
    }

    bool CheckAndJoinRoom()
    {
        Debug.Log("CheckAndJoinRoom");

        var list = CachedRoomList.Where(info =>
        {
            //入れない部屋は除外
            if (info.PlayerCount >= MAX_PLAYER_IN_ROOM) return false;

            return true;
        }).ToList();

        if (list.Count > 0)
        {
            Debug.Log("部屋があったので適当にはいる:" + list.Count);
            PhotonNetwork.JoinRoom(list[UnityEngine.Random.Range(0, list.Count)].Name);
            return true;
        }
        return false;
    }

    void CreateRoom()
    {
        Debug.Log("CreateRoom");
        RoomOptions roomOptions = new RoomOptions();

        //カスタムプロパティ
        ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();
        roomProp["UserName"] = "test";
        roomProp["GameState"] = 0;

        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = MAX_PLAYER_IN_ROOM;
        roomOptions.CustomRoomProperties = roomProp;
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "UserName", "GameState" };
        PhotonNetwork.CreateRoom(Guid.NewGuid().ToString(), roomOptions, TypedLobby.Default);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        base.OnLeftRoom();
        PhotonNetwork.Disconnect();
    }


    void UpdateUserStatus()
    {
        Debug.Log("UpdateUserStatus");
        Photon.Realtime.Room room = PhotonNetwork.CurrentRoom;
        if (room == null)
        {
            return;
        }

        int i = 0;
        foreach (var pl in room.Players.Values)
        {
            if (pl.CustomProperties["GUID"] == null) continue;
            if (pl.CustomProperties["UserName"] == null) continue;

            _player[i].Name = pl.CustomProperties["UserName"].ToString();
        }
    }

    //PUN2でのルーム取得
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        base.OnRoomListUpdate(roomList);

        CachedRoomList = roomList;
    }

    public int randomNumber;
    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");

        randomNumber = UnityEngine.Random.Range(0, 2);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("ランダムな数値" + randomNumber);
            PhotonNetwork.InstantiateRoomObject("Ball", new Vector3(0, 10, 0), Quaternion.identity);

            if (randomNumber == 0) SetMasterPos(true);
            else SetMasterPos(false);

            GetMasterPos(); if (!gotMasterPos) return;
            if (GetMasterPos())
            {
                Debug.Log("マスターはPlayer0 OnJoinedRoom");
                //プレイヤー作成
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("マスターはPlayer1 OnJoinedRoom");
                //プレイヤー作成
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
        else
        {
            GetMasterPos(); if (!gotMasterPos) return;
            if (GetMasterPos())
            {
                Debug.Log("自分はPlayer1 OnJoinedRoom");
                //プレイヤー作成
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("自分はPlayer0 OnJoinedRoom");
                //プレイヤー作成
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }

        UpdateUserStatus();
        State = PhotonState.IN_GAME;
        //StartCoroutine(Ready(3f));
    }

    
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //Debug.Log("マスターが白かどうか " + GetMasterColor());
        if (PhotonNetwork.IsMasterClient)
        {
            if (player0 != null || player1 != null) return;
            if (GetMasterPos())
            {
                Debug.Log("マスターはPlayer0 OnRoomPropertiesUpdate");
                //プレイヤー作成
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("マスターはPlayer1 OnRoomPropertiesUpdate");
                //プレイヤー作成
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
        else
        {
            if (player0 != null && player1 != null) return;
            if (GetMasterPos())
            {
                Debug.Log("自分はPlayer1 OnRoomPropertiesUpdate");
                //プレイヤー作成
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("自分はPlayer0 OnRoomPropertiesUpdate");
                //プレイヤー作成
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
    }


    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    Debug.Log(newPlayer + " が参加しました");
    //    //GivePlayers();

    //    if (GetMasterPos()) photonView.RPC(nameof(AllowAction), RpcTarget.All);
    //    else photonView.RPC(nameof(AllowAction), RpcTarget.All);
    //}


    private IEnumerator Ready(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        State = PhotonState.IN_GAME;
    }

    private static ExitGames.Client.Photon.Hashtable myProp = new ExitGames.Client.Photon.Hashtable();
    private static bool gotMasterPos = false;
    public static void SetMasterPos(bool isMaster)
    {
        Debug.Log("SetMasterPos");
        myProp["masterIsPlayer0"] = isMaster;
        myProp["GotMasterPos"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(myProp);
        myProp.Clear();
    }
    public static bool GetMasterPos()
    {
        Debug.Log("GetMasterPos");
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["masterIsPlayer0"]);
        gotMasterPos = (PhotonNetwork.CurrentRoom.CustomProperties["GotMasterPos"] is bool got) ? got : false;
        return (PhotonNetwork.CurrentRoom.CustomProperties["masterIsPlayer0"] is bool isMaster) ? isMaster : false;
    }

    [PunRPC]
    private void LockAction(GameObject masterPlayer)
    {
        Debug.Log("LockAction");
        if (masterPlayer.GetPhotonView().Owner.IsMasterClient)
        {
            ball.GetComponent<Ball_BasicMove>().enabled = false;
        }
    }

    [PunRPC]
    private void AllowAction(GameObject masterPlayer)
    {
        if (masterPlayer.GetPhotonView().Owner.IsMasterClient)
        {
            ball.GetComponent<Ball_BasicMove>().enabled = true;
        }
    }
}