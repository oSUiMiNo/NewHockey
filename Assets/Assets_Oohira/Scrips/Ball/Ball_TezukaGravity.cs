using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_TezukaGravity : MonoBehaviour
{
    //[SerializeField] bool useTezukaGravity_X = false;
    //[SerializeField] bool useTezukaGravity_Y = false;
    //[SerializeField] bool useTezukaGravity_Z = false;
    [SerializeField] bool useTezukaGravity_Soft = false;
    [SerializeField] bool useTezukaGravity_Hard = false;

    [SerializeField] GameObject tezukaJudgementRangeMark;
    [SerializeField] Vector3 tezukaGravityForward;
    [Range(0, 10)]
    [SerializeField] float tezykaJudgementRange_X = 0.05f;
    [Range(0, 10)]
    [SerializeField] float tedukaJudgementRange_Y = 0.05f;
    //[Range(0, 10)]
    //[SerializeField] float tezukaJudgementRange_Z = 3;
    [Range(0, 30)]
    [SerializeField] float tezukaMagnification_Soft = 0.2f;
    [Range(0, 30)]
    [SerializeField] float tezukaMagnification_Hard = 1.0f;
    [Range(0, 30)]
    [SerializeField] float tezukaNegativeZMagnification_Soft = 0.8f;
    [Range(0, 30)]
    [SerializeField] float tezukaNegativeZMagnification_Hard = 18.0f;
    [Range(0, 9)]
    [SerializeField] float tezukaRange_Soft = 9.0f;
    [Range(0, 9)]
    [SerializeField] float tezukaRange_Hard = 1.0f;

    private void Start()
    {
        A_Init();
    }
    private void FixedUpdate()
    {
        A_Update();
        TezukaGravity(rackets.racket0_Core);  //デフォルトのゆるい手塚ゾーン
        TezukaGravity(rackets.racket1_Core);
    }

    private void TezukaGravity(GameObject racket)
    {
        //if ((10 - Mathf.Abs(ballCoordinate.x) <= tezykaJudgementRange_X)) return;
        //if ((10 - Mathf.Abs(ballCoordinate.y) <= tedukaJudgementRange_Y)) return;

        float nowSpeed = rb.velocity.magnitude;

        int currentNegativeForward = 0;
        if (racket == this.rackets.racket0_Core) currentNegativeForward = 1;
        else currentNegativeForward = -1;

        Vector3 tezukaNegativeZMagnification_Soft = new Vector3(0f, 0f, this.tezukaNegativeZMagnification_Soft * currentNegativeForward);
        Vector3 tezukaNegativeZMagnification_Hard = new Vector3(0f, 0f, this.tezukaNegativeZMagnification_Hard * currentNegativeForward);

        //Vector3 racketForward = (racket.transform.position - transform.position).normalized;
        Vector3 gravityForward = (racket.transform.position - transform.position).normalized;
        Vector3 racketCoordinate = zone.CoordinateFromPosition(racket.transform.position);
        Vector3 tezukaGravity_Soft = gravityForward * nowSpeed * tezukaMagnification_Soft;//+ tezukaNegativeZMagnification_Soft;
        Vector3 tezukaGravity_Hard = gravityForward * nowSpeed * tezukaMagnification_Hard;//+ tezukaNegativeZMagnification_Hard;

        if (useTezukaGravity_Soft && Mathf.Abs(ballCoordinate.z - racketCoordinate.z) < tezukaRange_Soft)
            rb.AddForce(tezukaGravity_Soft);
        if (useTezukaGravity_Hard && Mathf.Abs(ballCoordinate.z - racketCoordinate.z) < tezukaRange_Hard)
            rb.AddForce(tezukaGravity_Hard);

        this.tezukaGravityForward = gravityForward; //確認用
    }

    //共通****************************************
    private DifineRackets rackets = null;
    private void BallComponent()
    {
        rackets = GameObject.Find("RoomCore").GetComponent<DifineRackets>();
    }
    private void Awake()
    {
        BallComponent();
    }
    [SerializeField] Vector3 primitiveCoordinate = new Vector3(0, 0, 0);
    [SerializeField] Vector3 ballCoordinate = new Vector3(0, 0, 0);
    private ZoneDiffinitionOfRoom zone = null;
    private Rigidbody rb = null;
    private void A_Init()
    {
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        Debug.Log("rb " + rb);
        primitiveCoordinate = zone.primitiveCoordinate; //ルーム座標の1目盛り辺りの長さを取得
    }
    private void A_Update()
    {
        ballCoordinate = zone.CoordinateFromPosition(transform.position);  //球のワールド座標をRoom座標に変換
    }
    //共通****************************************
}
