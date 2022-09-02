using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

public class Ball_BasicMove : MonoBehaviourPunCallbacks//, IPunInstantiateMagicCallback
{
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
        //Debug.Log("rb " + rb);
        primitiveCoordinate = zone.primitiveCoordinate; //ルーム座標の1目盛り辺りの長さを取得
    }
    private void A_Update()
    {
        ballCoordinate = zone.CoordinateFromPosition(transform.position);  //球のワールド座標をRoom座標に変換
    }
    //共通****************************************



    [SerializeField] float nowSpeed; //確認用
    [SerializeField] float ballCoordinate_Mag = 0; //確認用

    private Vector3 lastDirection;  //1フレーム前の進行方向を保存しとく。
    [SerializeField] private bool rec = true;
    [SerializeField] private int recVolume = 10;
    [SerializeField] private Vector3[] record = new Vector3[10];

    [Tooltip("ONならDefaultContactOffsetの値を衝突検知に使用")]
    [SerializeField] private bool useContactOffset = true;
    private float contactOffset;
    private const float sphereCastMargin = 0.01f; // SphereCast用のマージン
    //private const float sphereCastMargin = 2f; // SphereCast用のマージン

    [SerializeField] public bool canChange_CanStrike = true;
    [SerializeField] public bool canStrike = false;
    [SerializeField] public bool canReflect = false;
    
    //[SerializeField] public List<bool> wasStruck_ByPlayer0 = null;
    //[SerializeField] public List<bool> wasStruck_ByPlayer1 = null;
    [SerializeField] public bool[] wasStruck_ByPlayer0;
    [SerializeField] public bool[] wasStruck_ByPlayer1;
    [SerializeField] public int divisionPointsVolimeForReflect = 1;
    [SerializeField] public int reflectCounter = 0;
    [SerializeField] public float distance_Z = 0;

    [SerializeField] public Vector3[] passingPoints;

    [SerializeField] public Vector3? reboundVelocity; // 反射計算の後に反射速度を代入する用
    [SerializeField] public bool canKeepSpeed = true; //これがtrueの時は速度を保つ処理を実行し続ける。反射の時など速度を変える処理を行う間はfalseにする。
    [SerializeField] public bool canChangeSpeed = false;
    [Tooltip("弾の速度")]
    [Range(0, 1000)]
    [SerializeField] public float constantSpeed = 10.0f;
    [SerializeField] public float changeSpeed = 50.0f;
    [Range(0, 50)]
    [SerializeField] float minSpeed = 3.0f;
    [SerializeField]
    [Range(0, 1500)] float maxSpeed = 120f;

    [SerializeField] bool first = true;
    [SerializeField] Vector3 firstFource = new Vector3(1, 1, 1);

    [SerializeField] bool visualizeSphereCast = false;

    private SphereCollider col = null;

    [SerializeField] private Reflector reflector = null;
    [SerializeField] private PhotonManager photonManager = null;

    private void Components()
    {
        col = GetComponent<SphereCollider>();
    }
    private IEnumerator Init()
    {
        Debug.Log("ボール準備中0");
        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        Debug.Log("ボール準備中1");
        A_Init();
        Components();

        // isTrigger=false で使用する場合はContinuous Dynamicsに設定
        if (!col.isTrigger && rb.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        //Rigidbodyの重力の使用禁止
        if (rb.useGravity)
        {
            rb.useGravity = false;
        }
        contactOffset = Physics.defaultContactOffset;
        canKeepSpeed = true;
        canChange_CanStrike = true;
        wasStruck_ByPlayer0 = new bool[2] { false, false };
        wasStruck_ByPlayer1 = new bool[2] { false, false };
        record = new Vector3[recVolume];
        state = State.Ready;
    }

    private enum State
    {
        Wait,
        Ready
    }
    private State state;
    private void Start()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        StartCoroutine(Init());
    }

    private void FixedUpdate()
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("ボール準備中2");
        if (state != State.Ready) return;
        Debug.Log("ボール準備中3");
        if (rackets.racket0_Core == null || rackets.racket1_Core == null) return;

        Debug.Log("ボール稼働中");

        A_Update();

        //確認用********
        ballCoordinate_Mag = ballCoordinate.magnitude;
        nowSpeed = rb.velocity.magnitude;
        //確認用********

        if (first)
        {
            rb.AddForce(firstFource);
            StartCoroutine("First");
        }
        Record1();
        //FixPosition1();
        //Record2();
        FixPosition2();
        RestrictSpeed();  //スピードの上限下限
        //OverlapDetection();  // 重なりを解消
        //ApplyReboundVelocity();  // 前フレームで反射していたら反射後速度を反映
        KeepConstantSpeed();  //球の速度を一定に保つ
        ChangeSpeed();
        Process();

        //if ((transform.position - rackets.racket0_Core.transform.position).magnitude < 5f)
        //    racketMove0.Record();
        //if ((transform.position - rackets.racket1_Core.transform.position).magnitude < 5f)
        //    racketMove1.Record();
        
    }


    private IEnumerator First()
    {
        //if (!first) yield return null;

        //rb.AddForce(firstFource);

        yield return new WaitForSeconds(0.3f);
        first = false;
    }

    public void ApplyReboundVelocity()
    {
        rb.velocity = reboundVelocity.Value;
        reboundVelocity = null;
        canKeepSpeed = true;
    }

    //public void ApplyReboundVelocity()
    //{
    //   photonView.RPC(nameof(ApplyReboundVelocity_RPC), RpcTarget.All);
    //}

    //[PunRPC]
    //public void ApplyReboundVelocity_RPC()
    //{
    //    if (reboundVelocity == null) return;

    //    rb.velocity = reboundVelocity.Value;
    //    reboundVelocity = null;
    //    canKeepSpeed = true;
    //}

    // 速度を一定に保つ
    // 衝突や引っかかりによる減速を上書きする役目
    private void KeepConstantSpeed()
    {
        if (!canKeepSpeed) return;
        if (canChangeSpeed) return;

        var velocity = rb.velocity;
        var nowSpeedMag = velocity.magnitude;
        var SpeedMag = Mathf.Abs(constantSpeed);

        if (Mathf.Abs(SpeedMag - nowSpeedMag) >= 1)
        {
            var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
            rb.velocity = dir * constantSpeed;
        }
    }

    public void ChangeSpeed()
    {
        if (canKeepSpeed) return;
        if (!canChangeSpeed) return;

        var velocity = rb.velocity;

        var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
        rb.velocity = dir * changeSpeed;
    }
    //private IEnumerator EndChangeSpeed()
    //{
    //    yield return new WaitForSeconds(1);
    //    canChangeSpeed = false;
    //    canKeepSpeed = true;
    //}



    private void RestrictSpeed()
    {
        var velocity = rb.velocity;
        nowSpeed = velocity.magnitude;

        if (constantSpeed > maxSpeed) constantSpeed = maxSpeed;
        if (constantSpeed < minSpeed) constantSpeed = minSpeed;
    }





    //オブジェクトとの重なりを検知して解消するように位置補正する。主にTransform.positionで移動してきた外部オブジェクトを回避するのに使う。
    private void OverlapDetection()
    {
        // Overlap
        var colliders = Physics.OverlapSphere(col.transform.position, col.radius);
        var isOverlap = 1 < colliders.Length;
        if (isOverlap)
        {
            var pushDirection = Vector3.zero;
            var pushDistance = 0f;
            var totalPushPos = Vector3.zero;
            var pushCount = 0;

            foreach (var targetCollider in colliders)
            {
                // 自身のコライダーなら無視する
                if (targetCollider == col) continue;

                var isCollision = Physics.ComputePenetration(
                    col, col.transform.position, col.transform.rotation,
                    targetCollider, targetCollider.transform.position, targetCollider.transform.rotation,
                    out pushDirection, out pushDistance);

                if (isCollision && pushDistance != 0)
                {
                    totalPushPos += pushDistance * pushDirection;
                    pushCount++;
                }
            }

            if (pushCount != 0)
            {
                var pos = transform.position;
                pos += totalPushPos / pushCount;
                transform.position = pos;
            }
        }
    }
    

    private GameObject GetTargetClosest(Vector3 origin, float colliderRadius, Vector3 direction, LayerMask layerMask)
    {
        var hits = Physics.SphereCastAll(origin, colliderRadius, direction, 1000f, layerMask).Select(h => h.transform.gameObject).ToList();

        if (0 < hits.Count())
        {
            float max_target_distance = float.MinValue;
            GameObject target = null;

            foreach (var hit in hits)
            {
                float target_distance = Vector3.Distance(transform.position, hit.transform.position);

                if (target_distance > max_target_distance)
                {
                    max_target_distance = target_distance;
                    target = hit.transform.gameObject;
                }
            }
            return target;
        }
        else
        {
            return null;
        }
    }
    GameObject target;
    private void ProcessForwardDetection1(string name_ReflectorObject, LayerMask layerMask_Collider, LayerMask layerMask_Body, float sphereCastMargin)
    {
        //count_ProcessForwardDetection_1++;
        //if (sphereCasts_1.Count < count_ProcessForwardDetection_1) sphereCasts_1.Add(Instantiate(sphereCast));
        //if(count_ProcessForwardDetection >= 4) UnityEditor.EditorApplication.isPaused = true;
        //Debug.Log(count_ProcessForwardDetection_1 + " " + name_ReflectorObject);
        Vector3 velocity = rb.velocity;

        Vector3 direction_Core;
        Vector3 direction_Body;
        if (name_ReflectorObject == "Racket0")
        {
            direction_Core = (rackets.racket0_Core.transform.position - transform.position).normalized;
           // direction_Body = (rackets.racket0_Body.transform.position - transform.position).normalized;
        }
        else
        {
            direction_Core = (rackets.racket1_Core.transform.position - transform.position).normalized;
            //direction_Body = (rackets.racket0_Body.transform.position - transform.position).normalized;
        }
    
        float offset = useContactOffset ? contactOffset * 2 : 0;
        Vector3 origin = transform.position - direction_Core * (sphereCastMargin + offset);
        float colliderRadius = transform.localScale.x / 2 + offset;

        target = GetTargetClosest(origin, colliderRadius, direction_Core, layerMask_Collider);
        //if (!a) return;
        if(rackets.racket0_Collider_0 != target) rackets.racket0_Collider_0.SetActive(false);
        if (rackets.racket0_Collider_1 != target) rackets.racket0_Collider_1.SetActive(false);
        direction_Body = (target.transform.position - transform.position).normalized;

        bool isHit = Physics.SphereCast(origin, colliderRadius, direction_Body, out RaycastHit hitInfo, 100000f, layerMask_Collider);
        if (visualizeSphereCast)
        {
            Debug.DrawRay(origin, direction_Body * hitInfo.distance, Color.white, 0.02f, true);
            //sphereCasts_1[count_ProcessForwardDetection_1 - 1].transform.position = origin + direction_Core * hitInfo.distance;
            //sphereCasts_1[count_ProcessForwardDetection_1 - 1].transform.localScale = new Vector3(1, 1, 1) * colliderRadius * 2;
            sphereCast.transform.position = origin + direction_Core * hitInfo.distance;
            sphereCast.transform.localScale = new Vector3(1, 1, 1) * colliderRadius * 2;
        }

        if (!isHit) return;
        if (target.tag != name_ReflectorObject) return;
        Debug.Log("コライダーは " + target + "0" + rackets.racket0_Collider_0.activeSelf + "1" + rackets.racket0_Collider_1.activeSelf + "HitInfo" + hitInfo.collider.gameObject.name);
        Reflector reflector = target.GetComponent<Reflector>();
        //float distance = hitInfo.distance - sphereCastMargin;
        //float nextMoveDistance = rb.velocity.magnitude * Time.fixedDeltaTime + 0.5f;
        //if (distance > nextMoveDistance) return;
        //else canReflect = true;
        //Debug.Log("r  " + reflector);
        //Debug.Log("v  " + velocity);
        //Debug.Log("d  " + direction);
        //Debug.Log("h  " + hitInfo.collider.gameObject.name);
        //Debug.Log("s  " + sphereCastMargin);
        //Debug.Log("g  " + this.gameObject);
        reflector.Reflect(gameObject, velocity, direction_Core, hitInfo, sphereCastMargin);
        //canReflect = false;
        //if (rackets.racket0_Collider_0 != target) rackets.racket0_Collider_0.SetActive(true);
        //if (rackets.racket0_Collider_1 != target) rackets.racket0_Collider_1.SetActive(true);
        rackets.racket0_Collider_0.SetActive(true);
        rackets.racket0_Collider_1.SetActive(true);
        //StartCoroutine(A(target));
        //a = false;
    }
    bool a = true;
    IEnumerator A(GameObject target)
    {
        yield return new WaitForSeconds(2);
        if (rackets.racket0_Collider_0 != target) rackets.racket0_Collider_0.SetActive(true);
        if (rackets.racket0_Collider_1 != target) rackets.racket0_Collider_1.SetActive(true);
        a = true;
    }

    [SerializeField] LayerMask layerMask_Room;
    [SerializeField] LayerMask layerMask_Racket_Collider;
    [SerializeField] LayerMask layerMask_Racket_Body;
    [SerializeField] GameObject sphereCast;
    [SerializeField] List<GameObject> sphereCasts = new List<GameObject>();
    [SerializeField] int count_ProcessForwardDetection = 0;
    //[SerializeField] List<GameObject> sphereCasts_1 = new List<GameObject>();
    //[SerializeField] int count_ProcessForwardDetection_1 = 0;
    // 前方方向を監視して1フレーム後に衝突している場合は反射ベクトルを計算する。
    private void ProcessForwardDetection(string name_ReflectorObject, LayerMask layerMask, float sphereCastMargin, float reflectMargin)
    {
        //count_ProcessForwardDetection++;
        //if(sphereCasts.Count < count_ProcessForwardDetection) sphereCasts.Add(Instantiate(sphereCast));
        //if(count_ProcessForwardDetection >= 4) UnityEditor.EditorApplication.isPaused = true;
        //Debug.Log(count_ProcessForwardDetection + " " + name_ReflectorObject);
        Vector3 velocity = rb.velocity;

        Vector3 direction;
        if (name_ReflectorObject == "Racket0") direction = (rackets.racket0_Core.transform.position - transform.position).normalized;
        else if (name_ReflectorObject == "Racket1") direction = (rackets.racket1_Core.transform.position - transform.position).normalized;
        else direction = velocity.normalized;
        
        float offset = useContactOffset ? contactOffset * 2 : 0;
        Vector3 origin = transform.position - direction * (sphereCastMargin + offset);
        float colliderRadius = transform.localScale.x / 2 + offset;
        //GameObject target = GetTargetClosest(origin, colliderRadius, direction, layerMask);
        bool isHit = Physics.SphereCast(origin, colliderRadius, direction, out RaycastHit hitInfo, 10000f, layerMask);
        //if (visualizeSphereCast)
        //{
        //    Debug.DrawRay(origin, direction * hitInfo.distance, Color.white, 0.02f, false);
        //    sphereCasts[count_ProcessForwardDetection - 1].transform.position = origin + direction * hitInfo.distance;
        //    sphereCasts[count_ProcessForwardDetection - 1].transform.localScale = new Vector3(1, 1, 1) * colliderRadius * 2;
        //}

        if (!isHit) return;
        if (hitInfo.collider.gameObject.tag != name_ReflectorObject) return;

        float distance = hitInfo.distance - sphereCastMargin;
        float nextMoveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        if (distance > nextMoveDistance + reflectMargin) return;

        //R_target = gameObject;
        //R_velocity = velocity;
        //R_direction = direction;
        //R_hitInfo = hitInfo;
        //R_sphereCastMargin = sphereCastMargin;
        //photonView.RPC(nameof(Reflect), RpcTarget.All);

        Reflector reflector = hitInfo.collider.gameObject.GetComponent<Reflector>();
        reflector.Reflect(gameObject, velocity, direction, hitInfo, sphereCastMargin);
        //reflector.NewReflect(gameObject, velocity, direction, hitInfo, sphereCastMargin);
        //canReflect = false;
    }

    private void Process()
    {
        ProcessForwardDetection("Wall", layerMask_Room, 0.01f, 1.2f);  // 進行方向に衝突対象があるかどうか確認
 
        float ball_racket_Coordinate_Mag_1 = (transform.position - rackets.racket1_Core.transform.position).magnitude / primitiveCoordinate.magnitude;
        ProcessForwardDetection("Racket1", layerMask_Racket_Collider, 0, 0.8f);

        float ball_racket_Coordinate_Mag_0 = (transform.position - rackets.racket0_Core.transform.position).magnitude / primitiveCoordinate.magnitude;
        ProcessForwardDetection("Racket0", layerMask_Racket_Collider, 0, 0.8f);
 
        count_ProcessForwardDetection = 0;
    }

    private GameObject R_target;
    private Vector3 R_velocity;
    private Vector3 R_direction;
    private RaycastHit R_hitInfo;
    private float R_sphereCastMargin;
    [PunRPC]
    private void Reflect()
    {
        Reflector reflector = R_hitInfo.collider.gameObject.GetComponent<Reflector>();
        reflector.Reflect(gameObject, R_velocity, R_direction, R_hitInfo, R_sphereCastMargin);
    }



    public GameObject Create_ImpactPointOverlay(RaycastHit hitInfo)
    {
        Pool_ImpactOverlay pool_ImpactOverlay = GameObject.Find("ObjectPool").GetComponent<Pool_ImpactOverlay>();
        Quaternion normalRotation = Quaternion.LookRotation(hitInfo.normal);
        //Debug.Log("impact");
        GameObject impactOverlay = pool_ImpactOverlay.Object_Discharge(hitInfo.point + hitInfo.normal * 0.5f, normalRotation);
        pool_ImpactOverlay.Object_Hide(impactOverlay);
        //Debug.Log("オーバーレイの向き1" + hitInfo.normal);
        return impactOverlay;
    }



    private void FixPosition1()
    {
        int speedDecreation = 5;

        //Debug.Log("coordinate1" + ballCoordinate);
        if (Mathf.Abs(ballCoordinate.x) < 11 && Mathf.Abs(ballCoordinate.y) < 11 && Mathf.Abs(ballCoordinate.z) < 11) return;

        //Debug.Log("coordinate2" + ballCoordinate);
        rec = false;

        for (int i = 0; i < recVolume; i++)
        {
            Debug.Log("Fixposition1");
            if (Mathf.Abs(record[i].x) < 8.9 && Mathf.Abs(record[i].y) < 8.9 && Mathf.Abs(record[i].z) < 8.9)
            {
                Debug.Log("Fixposition2");
                Debug.Log(record[i + 2]);
                //transform.position = zone.PositionFromCoordinate(record[i]);
                transform.position = zone.PositionFromCoordinate(record[i + 2]);
                maxSpeed -= speedDecreation;
                constantSpeed -= speedDecreation;
                rb.velocity = rb.velocity.normalized * constantSpeed;
                Debug.Log(constantSpeed);

                //var velocity = rb.velocity;
                //var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
                //rb.velocity = dir * constantSpeed;

                //rec = true;
                return;
            }
        }
        rec = true;
    }






    private void Record1()
    {
        if (!rec) return;

        //衝突直前のフレームの速度を1フレームの間だけ記録しておく用。
        var velocity = rb.velocity;
        if (velocity != Vector3.zero)
        {
            lastDirection = velocity.normalized;
        }

        for (int i = 0; i < recVolume - 1; i++)
        {
            if (!rec) break;
            record[(recVolume - 1) - i] = record[(recVolume - 1) - i - 1];
            record[0] = ballCoordinate;

            //Debug.Log((recVolume - 1 - i).ToString() + " " + record[recVolume - 1 - i].ToString());
            //if (i == recVolume - 2) Debug.Log("0 " + record[0].ToString());
        }
    }

    private void Record0()
    {
        //record[4] = record[3];
        //record[3] = record[2];
        //record[2] = record[1];
        //record[1] = record[0];
        //record[0] = ballCoordinate;

        //Debug.Log("99" + record[recVolume - 1]);
        //Debug.Log("0" + record[0]);
    }

    //要改良
    int rec_Number = 0;
    private void Record2()
    {
        if (!rec) return;
        
        //衝突直前のフレームの速度を1フレームの間だけ記録しておく用。
        var velocity = rb.velocity;
        if (velocity != Vector3.zero)
        {
            lastDirection = velocity.normalized;
        }

        if (rec_Number > recVolume) rec_Number = 0;
        rec_Number++;
        record[rec_Number] = ballCoordinate;
    }

    private void FixPosition2()
    {
        float speedDecreation = 0.1f;

        //Debug.Log("coordinate1" + ballCoordinate);
        if (Mathf.Abs(ballCoordinate.x) < 10.5 && Mathf.Abs(ballCoordinate.y) < 10.5 && Mathf.Abs(ballCoordinate.z) < 10.5) return;

        //Debug.Log("coordinate2" + ballCoordinate);
        rec = false;

        for (int i = 0; i < recVolume; i++)
        {
            Debug.Log("Fixposition1");
            if (Mathf.Abs(record[i].x) < 8.9 && Mathf.Abs(record[i].y) < 8.9 && Mathf.Abs(record[i].z) < 8.9)
            {
                canReflect = true;
                Debug.Log("Fixposition2");
                Debug.Log(record[i]);
                transform.position = zone.PositionFromCoordinate(record[i]);
                //transform.position = zone.PositionFromCoordinate(record[i + 2]);
                //maxSpeed -= speedDecreation;
                constantSpeed -= speedDecreation;
                rb.velocity = rb.velocity.normalized * constantSpeed;
                //Debug.Log(constantSpeed);

                //var velocity = rb.velocity;
                //var dir = velocity != Vector3.zero ? velocity.normalized : lastDirection;
                //rb.velocity = dir * constantSpeed;

                //rec = true;
                return;
            }

            if(i >= recVolume) transform.position = zone.PositionFromCoordinate(new Vector3(0, 0, 0));
        }
        rec = true;
    }

    //void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    Debug.Log("作られた1");
    //    photonManager = GameObject.FindGameObjectWithTag("Photon").GetComponent<PhotonManager>();
    //    photonManager.ball = gameObject;
    //    Debug.Log("作られた2");
    //}
}