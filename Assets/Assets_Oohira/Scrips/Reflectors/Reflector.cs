using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public abstract class Reflector : MonoBehaviourPunCallbacks
{
    [SerializeField] protected float changeSpeedTime = 0;

    [SerializeField] protected float reflectMargin = 0;

    protected GameObject ball = null;

    private RoomDoorWay roomDoorWay = null;

    protected ZoneDiffinitionOfRoom zone = null;
    protected GameObject objectPool = null;
    protected Ball_BasicMove ballMove = null;
    protected Rigidbody rb = null;
    protected Pool_ImpactOverlay pool_ImpactOverlay = null;

    [SerializeField] protected GameObject Line0 = null;
    [SerializeField] protected GameObject Line1 = null;

    [SerializeField] protected Avatar avatar0 = null;
    [SerializeField] protected Avatar avatar1 = null;

    protected DifineRackets rackets = null;
    
    private void Start()
    {
        //StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        roomDoorWay = RoomDoorWay.instance;
        Init_Child();
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        objectPool = GameObject.Find("ObjectPool");
        pool_ImpactOverlay = objectPool.GetComponent<Pool_ImpactOverlay>();
        Define_RefloctMargin();
        Line0 = GameObject.Find("Lines_Player0");
        Line1 = GameObject.Find("Lines_Player1");

        Debug.Log(rackets);
        Debug.Log(rackets.racket0_Core);
        avatar0 = roomDoorWay.avatar0.GetComponent<Avatar>();
        avatar1 = roomDoorWay.avatar1.GetComponent<Avatar>();
        Debug.Log(avatar0);
        Debug.Log(Line0); Debug.Log(Line1);
    }

    protected abstract void Init_Child();

    protected abstract void Define_RefloctMargin();

    protected void GetBallInfo(GameObject target)
    {
        ball = target;
        ballMove = ball.GetComponent<Ball_BasicMove>();
        rb = ball.GetComponent<Rigidbody>();
    }


    [PunRPC]
    protected void GetBallInfo_RPC(string targetName)
    {
        ball = GameObject.Find(targetName);
        ballMove = ball.GetComponent<Ball_BasicMove>();
        rb = ball.GetComponent<Rigidbody>();
    }
   


    //あとでballmoveの変数をプロパティで読み取り専用にして、速度等はこっち側で定義するようにして引数を減らす。
    //public abstract void Reflect(GameObject target, Vector3 velocity, Vector3 direction, RaycastHit hitInfo, float sphereCastMargin, float distance);
    public abstract void Reflect(GameObject target, Vector3 velocity, Vector3 inDirection, RaycastHit hitInfo, float sphereCastMargin);
    public abstract void NewReflect(GameObject target, Vector3 velocity, Vector3 inDirection, RaycastHit hitInfo, float sphereCastMargin);
}