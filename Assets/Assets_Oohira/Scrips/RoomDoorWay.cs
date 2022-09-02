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
        Debug.Log("�}�X�^�[�T�[�o�[�ւ̐ڑ������݂܂�");
        PhotonNetwork.ConnectUsingSettings();
    }
    public void Join()
    {
        ready = false;
      
        //��Ŏ���*****************************************************
        //PhotonNetwork.NickName = gameManager.playerName;
        //��Ŏ���*****************************************************

        Debug.Log("�����_���ȃ��[���ւ̎Q�������݂܂�");
        PhotonNetwork.JoinRandomRoom();  // �����_���ȃ��[���ɎQ������
    }

    public void Leave()
    {
        Debug.Log("���[������̑ޏo�����݂܂�");
        PhotonNetwork.LeaveRoom();
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("�}�X�^�[�T�[�o�[�ɐڑ����܂���");
        tryingConectingToMasterServer = false;
        isConectedToMasterServer = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Photon�T�[�o�[����ؒf���܂���");
        //Debug.Log(cause);
        tryingConectingToMasterServer = true;
        isConectedToMasterServer = false;
        Debug.Log("�ēx�}�X�^�[�T�[�o�[�ւ̐ڑ������݂܂�");
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("���[���������̂ō쐬���܂�");
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;  // ���[���̎Q���l����2�l�ɐݒ肷��
        PhotonNetwork.CreateRoom(null, roomOptions);  // �����_���ŎQ���ł��郋�[�������݂��Ȃ��Ȃ�A�V�K�Ń��[�����쐬����
    }


    private bool masterIsBlue = false;
    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ�����܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************

        int randomNumber = Random.Range(0, 2);
        randomNumber = 1;

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("����1");
            if (avatar0 || avatar1) return;
            //Debug.Log("����2");
            if (randomNumber == 0)
            {
                //Debug.Log("�}�X�^�[�̐F�͔� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);
            }
            else
            {
                //Debug.Log("�}�X�^�[�̐F�͍� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            //PhotonNetwork.InstantiateRoomObject("BallMonitor", new Vector3(0f, 10f, 0f), Quaternion.identity);
            PhotonNetwork.Instantiate("Ball_New", new Vector3(0f, 10f, 0f), Quaternion.identity);
        }
        else
        {
            if (GameObject.Find("Avatar0(Clone)"))
            {
                //Debug.Log("�����͎Q����3");
                //Debug.Log("�����̐F�͍� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar1", new Vector3(0f, 5f, 80f), Quaternion.Euler(0, 180, 0));
            }
            else
            {
                //Debug.Log("�����͎Q����4");
                //Debug.Log("�����̐F�͔� OnRoomPropertiesUpdate");
                PhotonNetwork.Instantiate("Avatar0", new Vector3(0f, 5f, -80f), Quaternion.identity);
            }
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("���[������ޏo���܂���");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer + " ���Q�����܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //Debug.Log(otherPlayer + " ���ޏo���܂���");

        //��Ŏ���*****************************************************
        //GivePlayers();
        //��Ŏ���*****************************************************

        Debug.Log("�ޏo���܂�");
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
            //��Ŏ���*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = false;
            //��Ŏ���*****************************************************
        }
    }

    [PunRPC]
    public void AllowAction(GameObject masterAvatar)
    {
        if (masterAvatar.GetPhotonView().Owner.IsMasterClient)
        {
            //��Ŏ���*****************************************************
            //masterAvatar.GetComponent<PieceSelector>().enabled = true;
            //��Ŏ���*****************************************************

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