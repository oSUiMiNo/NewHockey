using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
//using System;

public class RoomDoorWay : MonoBehaviourPunCallbacks
{
    public static RoomDoorWay instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private GameManager gameManager = null;
    public GameData gameData;
    public bool tryingConectingToMasterServer = false;
    public bool isConectedToMasterServer = false;
    public bool ready = false;

    public GameObject avatar0 = null;
    public GameObject avatar1 = null;
    public GameObject ball = null;


    private void Start()
    {
        gameManager = GameManager.instance;
        Random.InitState(System.DateTime.Now.Millisecond);
        StartCoroutine(SetObjects());
    }

    public void ConnectToMasterServer()
    {
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("マスターサーバーへの接続を試みます");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Join()
    {
        ready = false;
      
        //後で実装*****************************************************
        //PhotonNetwork.NickName = gameManager.playerName;
        //後で実装*****************************************************

        Debug.Log("ランダムなルームへの参加を試みます");
        PhotonNetwork.JoinRandomRoom();  // ランダムなルームに参加する
    }

    public void Leave()
    {
        Debug.Log("ルームからの退出を試みます");
        PhotonNetwork.LeaveRoom();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("マスターサーバーに接続しました");
        tryingConectingToMasterServer = false;
        isConectedToMasterServer = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Photonサーバーから切断しました");
        //Debug.Log(cause);
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("再度マスターサーバーへの接続を試みます");
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("ルームが無いので作成します");
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;  // ルームの参加人数を2人に設定する
        PhotonNetwork.CreateRoom(null, roomOptions);  // ランダムで参加できるルームが存在しないなら、新規でルームを作成する
    }


    private bool masterIsBlue = false;
    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        int randomNumber = Random.Range(0, 2);
        randomNumber = 1;

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("生成1");
            if (avatar0 || avatar1) return;
            //Debug.Log("生成2");
            if (randomNumber == 0)
            {
                //Debug.Log("マスターの色は白 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);
            }
            else
            {
                //Debug.Log("マスターの色は黒 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            //PhotonNetwork.InstantiateRoomObject("BallMonitor", new Vector3(0f, 10f, 0f), Quaternion.identity);
            PhotonNetwork.Instantiate("Ball_New", new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
        else
        {
            if (GameObject.Find("Avatar0(Clone)"))
            {
                //Debug.Log("自分は参加者3");
                //Debug.Log("自分の色は黒 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            else
            {
                //Debug.Log("自分は参加者4");
                //Debug.Log("自分の色は白 OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);
            }
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("ルームから退出しました");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + " が参加しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log(otherPlayer + " が退出しました");

        //後で実装*****************************************************
        //GivePlayers();
        //後で実装*****************************************************

        Debug.Log("退出します");
        Leave();
        gameManager.Load_Menu();
    }

  
    public Player[] GetPlayers() { return PhotonNetwork.PlayerList; }

    public void GivePlayers() { gameManager.players = PhotonNetwork.PlayerList; }




    [PunRPC]
    public void LockAction(GameObject masterAvatar)
    {
        Debug.Log("LockAction");
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //後で実装*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = false;
            //後で実装*****************************************************
        }
    }

    [PunRPC]
    public void AllowAction(GameObject masterAvatar)
    {
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //後で実装*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = true;
            //後で実装*****************************************************

            if (GameObject.Find("FirstFloor") != null)
                GameObject.Find("FirstFloor").SetActive(false);
        }
    }

    public bool avatar_0 = false;
    public bool avatar_1 = false;
    public bool ball_ = false;
    private void Update()
    {
        if (!Ready())
        {
            if (GameObject.Find("Avatar0(Clone)"))
                avatar_0 = true;
            if (GameObject.Find("Avatar1(Clone)"))
                avatar_1 = true;
            if (GameObject.Find("Ball_New(Clone)"))
                ball_ = true;
        }
    }
    public bool Ready()
    {
        if (!avatar_0) return false;
        if (!avatar_1) return false;
        if (!ball_) return false;

        ready = true;
        return true;
    }

    private IEnumerator SetObjects()
    {
        yield return new WaitUntil(() => Ready());
        avatar0 = GameObject.Find("Avatar0(Clone)");
        avatar1 = GameObject.Find("Avatar1(Clone)");
        ball = GameObject.Find("Ball(Clone)");
    }

    //public bool Ready()
    //{
    //    if (!avatar0) return false;
    //    if (!avatar1) return false;
    //    if (!hockeySet) return false;

    //    ready = true;
    //    return true;
    //}
}