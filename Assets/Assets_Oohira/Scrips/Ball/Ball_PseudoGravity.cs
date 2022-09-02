using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_PseudoGravity : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField] float pseudoGravity = 5.0f;


    private void Start()
    {
        A_Init();
    }
    private void FixedUpdate()
    {
        A_Update();
        PseudoGravity();  //疑似重力をかける
    }

    private void PseudoGravity()
    {
        //Debug.Log("rb " + rb);
        //Debug.Log("pseudoGravity " + pseudoGravity);
        rb.AddForce(0, pseudoGravity * -1, 0);
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
    [SerializeField] GameObject racket1;
    [SerializeField] GameObject racket2;
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
