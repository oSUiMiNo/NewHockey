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
    /// �ڑ��J�n(�I�t���C�����͎g�p���Ȃ�)
    /// </summary>
    public void Connect()
    {
        Debug.Log("Connect");

        //FPS����
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
            //�ڑ��J�n
            case PhotonState.CONNECTED:
                JoinLobby();
                State = PhotonState.WAITING;
                break;

            //���r�[�ŕ����I��
            case PhotonState.IN_LOBBY:
                {
                    //���[�����X�g���炤�܂ŊԂ�����̂őҋ@����
                    if (CachedRoomList == null) break;

                    //���܂̃��r�[�ɂ���l�̃��[�����X�g�����炢�A����������[������}�b�`���O�����T��
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

            //�Q�[����
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

        //�J�X�^���v���p�e�B
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
            //����Ȃ������͏��O
            if (info.PlayerCount >= MAX_PLAYER_IN_ROOM) return false;

            return true;
        }).ToList();

        if (list.Count > 0)
        {
            Debug.Log("�������������̂œK���ɂ͂���:" + list.Count);
            PhotonNetwork.JoinRoom(list[UnityEngine.Random.Range(0, list.Count)].Name);
            return true;
        }
        return false;
    }

    void CreateRoom()
    {
        Debug.Log("CreateRoom");
        RoomOptions roomOptions = new RoomOptions();

        //�J�X�^���v���p�e�B
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

    //PUN2�ł̃��[���擾
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
            Debug.Log("�����_���Ȑ��l" + randomNumber);
            PhotonNetwork.InstantiateRoomObject("Ball", new Vector3(0, 10, 0), Quaternion.identity);

            if (randomNumber == 0) SetMasterPos(true);
            else SetMasterPos(false);

            GetMasterPos(); if (!gotMasterPos) return;
            if (GetMasterPos())
            {
                Debug.Log("�}�X�^�[��Player0 OnJoinedRoom");
                //�v���C���[�쐬
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("�}�X�^�[��Player1 OnJoinedRoom");
                //�v���C���[�쐬
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
                Debug.Log("������Player1 OnJoinedRoom");
                //�v���C���[�쐬
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("������Player0 OnJoinedRoom");
                //�v���C���[�쐬
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
        //Debug.Log("�}�X�^�[�������ǂ��� " + GetMasterColor());
        if (PhotonNetwork.IsMasterClient)
        {
            if (player0 != null || player1 != null) return;
            if (GetMasterPos())
            {
                Debug.Log("�}�X�^�[��Player0 OnRoomPropertiesUpdate");
                //�v���C���[�쐬
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("�}�X�^�[��Player1 OnRoomPropertiesUpdate");
                //�v���C���[�쐬
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
                Debug.Log("������Player1 OnRoomPropertiesUpdate");
                //�v���C���[�쐬
                player1 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, 80), Quaternion.identity);

                Debug.Log(player1);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
            else
            {
                Debug.Log("������Player0 OnRoomPropertiesUpdate");
                //�v���C���[�쐬
                player0 = PhotonNetwork.Instantiate("OVRPlayerController_Oohira", new Vector3(0, 1, -80), Quaternion.identity);

                Debug.Log(player0);
                ball.GetComponent<Avatar>().photonView.RPC(nameof(LockAction), RpcTarget.All);
            }
        }
    }


    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    Debug.Log(newPlayer + " ���Q�����܂���");
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