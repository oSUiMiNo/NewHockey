using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_OrbitAdjuster : MonoBehaviour
{
    [SerializeField] bool useOrbitAdjuster_X = false;
    [SerializeField] bool useOrbitAdjuster_Y = false;
    [SerializeField] bool useOrbitAdjuster_Z = false;
    [Tooltip("速度調整用の倍率")]
    [Range(0, 20)]
    [SerializeField] private float orbitAdjustMagnification = 2.0f;
    [Range(0, 10)]
    [SerializeField] float adjustJudgementRange_X = 0.1f;
    [Range(0, 10)]
    [SerializeField] float adjustJudgementRange_Y = 0.1f;

    private void Start()
    {
        A_Init();
    }
    private void FixedUpdate()
    {
        A_Update();
        StartCoroutine("OrbitAdjuster");  //速度ベクトルのどれかが収束してしまったらずらす力をかける
    }

    private IEnumerator OrbitAdjuster()
    {
        var dil = rb.velocity.normalized;
        var dil_x = Mathf.Abs(dil.x);
        var dil_y = Mathf.Abs(dil.y);
        var dil_z = Mathf.Abs(dil.z);
        if (useOrbitAdjuster_X && dil_x <= 0.01)
        {
            Debug.Log("Adjust_X");
            if (ballCoordinate.x > 0)
                rb.velocity += new Vector3(primitiveCoordinate.x * orbitAdjustMagnification, 0, 0);
            else
                rb.velocity += new Vector3(-primitiveCoordinate.x * orbitAdjustMagnification, 0, 0);

            useOrbitAdjuster_X = false;
            yield return new WaitForSeconds(2f);
            useOrbitAdjuster_X = true;
        }
        if (useOrbitAdjuster_Y && dil_y <= 0.01)
        {
            Debug.Log("Adjust_Y");
            if (ballCoordinate.y > 0)
                rb.velocity += new Vector3(0, primitiveCoordinate.y * orbitAdjustMagnification, 0);
            else
                rb.velocity += new Vector3(0, -primitiveCoordinate.y * orbitAdjustMagnification, 0);

            useOrbitAdjuster_Y = false;
            yield return new WaitForSeconds(2f);
            useOrbitAdjuster_Y = true;
        }
        if (useOrbitAdjuster_Z && dil_z <= 0.01)
        {
            Debug.Log("Adjust_Z");
            if (ballCoordinate.z > 0)
                rb.velocity += new Vector3(0, 0, primitiveCoordinate.z * orbitAdjustMagnification);
            else
                rb.velocity += new Vector3(0, 0, -primitiveCoordinate.z * orbitAdjustMagnification);

            useOrbitAdjuster_Z = false;
            yield return new WaitForSeconds(2f);
            useOrbitAdjuster_Z = true;
        }

        //useOrbitAdjuster_X = true;
        //useOrbitAdjuster_Y = true;
        //useOrbitAdjuster_Z = true;
    }

    private void Switch()
    {
        if (Mathf.Abs(10 - ballCoordinate.x) < adjustJudgementRange_X) useOrbitAdjuster_X = true;
        if (Mathf.Abs(10 - ballCoordinate.y) < adjustJudgementRange_Y) useOrbitAdjuster_Y = true;
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
        primitiveCoordinate = zone.primitiveCoordinate; //ルーム座標の1目盛り辺りの長さを取得
    }
    private void A_Update()
    {
        ballCoordinate = zone.CoordinateFromPosition(transform.position);  //球のワールド座標をRoom座標に変換
    }
    //共通****************************************
}
