using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Avatar : MonoBehaviourPunCallbacks//, IPunInstantiateMagicCallback
{
    [SerializeField] private float charge = 0;
    [SerializeField] private Image gageImage = null;
    //[SerializeField] private PhotonManager photonManager = null;
    [SerializeField] private RoomDoorWay roomDoorWay = null;
    [SerializeField] private DifineRackets rackets = null;

    [SerializeField] private GameObject roomCore = null;

    private enum State
    {
        Wait,
        Ready
    }
    private State state;
    private void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        gageImage = GameObject.Find("Gage1").GetComponent<Image>();
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
        //rackets.Init();
    }

    public void Charge(float chargePoint)
    {
        charge += chargePoint;
        if (charge <= 0) charge = 0;
        gageImage.fillAmount = charge;
    }



    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{

    //    Debug.Log("作られた1");
    //    SetThis(gameObject);
    //    Debug.Log("作られた2");
    //}


    //public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    //{
    //    Debug.Log(propertiesThatChanged);
    //    GetThis();
    //    Debug.Log("プレイヤー追加1");
    //    if (!playerWasSet) return;

    //    Debug.Log("プレイヤー追加2");
    //    Debug.Log(GetThis());
    //    Debug.Log(RoomDoorWay.instance.avatar0);
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        if (gameObject.name == "Avatar0(Clone)")
    //        {
    //            Debug.Log("player0追加");
    //            RoomDoorWay.instance.avatar0 = GetThis();
    //        }
    //        else
    //        {
    //            Debug.Log("player1追加");
    //            RoomDoorWay.instance.avatar1 = GetThis();
    //        }
    //    }
    //    else
    //    {
    //        if (gameObject.name == "Avatar0(Clone)")
    //        {
    //            Debug.Log("player0追加");
    //            RoomDoorWay.instance.avatar0 = GetThis();
    //        }
    //        else
    //        {
    //            Debug.Log("player1追加");
    //            RoomDoorWay.instance.avatar1 = GetThis();
    //        }
    //    }

    //    Debug.Log("ラケット設定1");
    //    rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
    //    //rackets.Init();
    //    playerWasSet = false;
    //    //photonView.RPC(nameof(AddThis), RpcTarget.All);
    //}

    //[PunRPC]
    //private void AddThis()
    //{
    //    Debug.Log("プレイヤー追加1");
    //    Debug.Log(playerWasSet);
    //    if (!playerWasSet) return;

    //    Debug.Log("プレイヤー追加2");
    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        if (gameObject.name == "Avatar0(Clone)")
    //        {
    //            Debug.Log("player0追加");
    //            roomDoorWay.avatar0 = GetThis();
    //        }
    //        else
    //        {
    //            Debug.Log("player1追加");
    //            roomDoorWay.avatar1 = GetThis();
    //        }
    //    }
    //    else
    //    {
    //        if (gameObject.name == "Avatar0(Clone)")
    //        {
    //            Debug.Log("player0追加");
    //            roomDoorWay.avatar0 = GetThis();
    //        }
    //        else
    //        {
    //            Debug.Log("player1追加");
    //            roomDoorWay.avatar1 = GetThis();
    //        }
    //    }

    //    playerWasSet = false;
    //}


    //private static ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
    //[SerializeField] private static bool playerWasSet = false;
    //public static void SetThis(GameObject player)
    //{
    //    Debug.Log("SetPlayer");
    //    prop["Player"] = player.name;
    //    prop["PlayerWasSet"] = true;
    //    PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
    //    prop.Clear();
    //}
    //public static GameObject GetThis()
    //{
    //    Debug.Log("GetPlayer");
    //    Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["Player"]);
    //    playerWasSet = (PhotonNetwork.CurrentRoom.CustomProperties["PlayerWasSet"] is bool got) ? got : false;
    //    string player = (PhotonNetwork.CurrentRoom.CustomProperties["Player"] is string p) ? p : null;
    //    return GameObject.Find(player);
    //}



    //[PunRPC]
    //public void LockAction()
    //{
    //    Debug.Log("LockAction");
    //    GetComponent<PieceSelector>().enabled = false;
    //}

    //[PunRPC]
    //public void AllowAction()
    //{
    //    GetComponent<PieceSelector>().enabled = true;
    //}
}
