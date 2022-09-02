using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using DG.Tweening; 

public class Ball : MonoBehaviourPunCallbacks
{
    private enum State
    {
        Wait,
        Ready,
        BothReady,
        Init,
        Update
    }
    public enum MoveState
    {
        First,
        Move,
        Reflect,
        ToPlayer,
        Idle
    }
    public enum StrikeState
    {
        StruckByPlayer0,
        StruckByPlayer1,
        Idle
    }
    public enum ToPlayerState
    {
        ToPlayer0,
        ToPlayer1,
        Idle
    }
    [SerializeField] private State state;
    public MoveState moveState = MoveState.Idle;
    public StrikeState strikeState = StrikeState.Idle;
    public ToPlayerState toPlayerState = ToPlayerState.Idle;
    public Owners owner_Ball;


    [SerializeField] private float reflectAngle;
    [SerializeField] private Vector3 positiveZAxis;

    Rigidbody rb;
    [SerializeField] bool visualizeSphereCast = false;
    [SerializeField] GameObject sphereCast;
    int randomNumber;
    [SerializeField] private Vector3 firstDirection;
    [SerializeField] private Vector3 struckDirection;


    [SerializeField] float speed = 3;
    [SerializeField] float margin = 1;

    [SerializeField] GameObject racket0;
    [SerializeField] GameObject racket1;

    [SerializeField] GameObject line0;
    [SerializeField] GameObject line1;
    [SerializeField] float distance_Z;
    [SerializeField] int divisionPointsVolume;
    [SerializeField] int passingPointsVolume = 0;
    [SerializeField] int count = 0;
    
    //[SerializeField] private Vector3[] points;
    //[SerializeField] private Vector3[] normals;
    [SerializeField] private List<Vector3> points = new List<Vector3>();
    [SerializeField] private List<Vector3> normals = new List<Vector3>();

    [SerializeField] private Vector3 lastPoint;
    [SerializeField] private Vector3 lastNormal;

    [SerializeField] private int ���]������;
    void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        moveState = MoveState.First;

        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        StartCoroutine(ConfirmPreparation());
        yield return new WaitUntil(() => state == State.BothReady);
        //yield return new WaitForSeconds(1);


        line0 = GameObject.Find("Lines_Player0");
        line1 = GameObject.Find("Lines_Player1");
        racket0 = GameObject.Find("Racket0");
        racket1 = GameObject.Find("Racket1");
        passingPointsVolume = 2;

        sphereCast = GameObject.Find("Sphere");
        rb = GetComponent<Rigidbody>();
        randomNumber = UnityEngine.Random.Range(-3, 3);

        StartCoroutine(Reversal());

        //moveState = MoveState.Move;
        state = State.Ready;
    }

    [SerializeField] private bool player0Ready = false;
    [SerializeField] private bool player1Ready = false;
    private IEnumerator ConfirmPreparation()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        if(PhotonNetwork.IsMasterClient) photonView.RPC(nameof(BothReady), RpcTarget.All, "player1");
        else                             photonView.RPC(nameof(BothReady), RpcTarget.All, "player0");

        yield return new WaitUntil(() => player0Ready && player1Ready);
        state = State.BothReady;
    }
    [PunRPC]
    private void BothReady(string player)  //�����͌�ŃQ�[���}�l�[�W���[�ɈڐA����
    {
        if (player == "player1") player1Ready = true;
        else                     player0Ready = true;
    }


    private void FixedUpdate()
    {
        if (state != State.Ready) return;
        //if (moveState == MoveState.Reflect) StartCoroutine(Reversal());
        //if (moveState == MoveState.Move) Move();
        if (toPlayerState != ToPlayerState.Idle) Process();
    }

    private IEnumerator Reversal()
    {
        if (state != State.BothReady) ���]������++;
        Debug.Log("���]������" + ���]������);
        yield return new WaitForSeconds(0);
        if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(Reversal_0), RpcTarget.All, transform.position + lastNormal * margin, lastNormal);

        //yield return new WaitUntil(() => moveState == MoveState.Idle);
        //if (PhotonNetwork.IsMasterClient) photonView.RPC(nameof(Reversal_1), RpcTarget.All);
    }
   
    [PunRPC]
    private void Reversal_0(Vector3 point_0, Vector3 normal_0)
    {
        transform.position = point_0;

        Debug.Log("���]�̏�����0");
        count = 0;
        points.Clear();
        normals.Clear();
        Debug.Log("���]�̏�����1");
        Debug.Log(points.Count);

        points.Add(point_0);
        normals.Add(normal_0);
        ProcessReflect_Middle(0);
        StartCoroutine(Wait(0));

        if (owner_Ball == Owners.player0) positiveZAxis.z = -1;
        else                              positiveZAxis.z = 1;

        reflectAngle = Vector3.Angle(positiveZAxis, outDirection);
        if (reflectAngle >= 70) passingPointsVolume = 8;
        else if (reflectAngle >= 60) passingPointsVolume = 7;
        else if (reflectAngle >= 50) passingPointsVolume = 6;
        else if (reflectAngle >= 40) passingPointsVolume = 5;
        else if (reflectAngle >= 30) passingPointsVolume = 4;
        else if (reflectAngle >= 20) passingPointsVolume = 3;
        else if (reflectAngle >= 9) passingPointsVolume = 2;
        else passingPointsVolume = 1;

        moveState = MoveState.Idle;

        Debug.Log("���]�̏�����2");
        for (int a = 1; a < passingPointsVolume; a++)
        {
            ProcessReflect_Middle(a);
        }
        for (int a = 1; a < passingPointsVolume + 1; a++)
        {
            StartCoroutine(Wait(a));
        }
        //StartCoroutine(Wait(passingPointsVolume));
        Debug.Log("���]�̏�����3");
        moveState = MoveState.Move;

    }
    [PunRPC]
    private void Reversal_1()
    {
        Debug.Log("���]�̏�����2");
        for (int a = 1; a < passingPointsVolume; a++)
        {
            ProcessReflect_Middle(a);
        }
        for (int a = 1; a < passingPointsVolume + 1; a++)
        {
            StartCoroutine(Wait(a));
        }
        //StartCoroutine(Wait(passingPointsVolume));
        Debug.Log("���]�̏�����3");
        moveState = MoveState.Move;
    }


        Vector3 outDirection = Vector3.zero;
    private void ProcessReflect_Middle(int a)
    {
        Color rayColor = Color.white;
        float radius = transform.localScale.x / 2;
        RaycastHit hitInfo;
        if (a < passingPointsVolume - 1 && passingPointsVolume != 1)                                //�Ō�̃J�E���g�ȊO
        {
            if (moveState == MoveState.First)            //�Q�[���J�n��̈�ԍŏ�
            {
                Debug.Log("First  " + a);
                //���]�̓���***********************************
                outDirection = firstDirection.normalized;
            }
            else if (strikeState != StrikeState.Idle)    //���P�b�g�őł��ꂽ����
            {
                Debug.Log("Middle0  " + a);
                //���]�̓���***********************************
                outDirection = struckDirection.normalized;
            }
            else                                         //����ȊO
            {
                Debug.Log("Middle1  " + a);
                if (a == 0)                   //a��0�̍ŏ��̃��[�v
                {
                    //���]�̓���***********************************
                    Vector3 inDirection = (points[0] - lastPoint).normalized;
                    outDirection = (OutDestination_General(inDirection, normals[0]) - points[0]).normalized;
                }
                else                          //a��1�`�Ō�܂ł̃��[�v
                {
                    Vector3 inDirection = (points[a] - points[a - 1]).normalized;
                    
                    Vector3 lineDirection = Vector3.zero;
                    if (owner_Ball == Owners.player1) lineDirection = line1.transform.position - points[1];
                    if (owner_Ball == Owners.player0) lineDirection = line0.transform.position - points[1];
                    int divisionVolume = passingPointsVolume - 2;
                    distance_Z = (lineDirection / divisionVolume).z;
                    Vector3 goal_Z = new Vector3(0, 0, points[a].z + distance_Z);
                    
                    outDirection = (OutDestination_Flex(inDirection, normals[a], goal_Z) - points[a]).normalized;
                }
            }
            rayColor = Color.red;
            strikeState = StrikeState.Idle;
        }
        else if (a == passingPointsVolume - 1)                      //�Ō�̃J�E���g�̎�
        {
            Debug.Log("Final  " + a);
            outDirection = (GetPlayerTargetPosition() - points[a]).normalized;
            rayColor = Color.blue;
        }
        Debug.Log("���C��΂�����  " + outDirection + ", ���C�̌��_  " + points[a]);
        Physics.SphereCast(points[a], radius, outDirection, out hitInfo, 10000f, layerMask_Wall);
        Debug.Log("���C���������ꏊ  " + hitInfo.point + ", ���C�̒���  " + hitInfo.distance);
        Debug.DrawRay(points[a], outDirection * hitInfo.distance, rayColor, 8f, false);
        Debug.DrawRay(points[a], outDirection * 5, Color.green, 2f, false);
        Instantiate(sphereCast, hitInfo.point + hitInfo.normal * margin, Quaternion.identity);
        //if (a + 1 < points.Length) Debug.Log("���̃C���f�b�N�X  " + (a + 1));
        //if (a + 1 < points.Length) points[a + 1] = hitInfo.point + hitInfo.normal * margin;
        //if (a + 1 < normals.Length) normals[a + 1] = hitInfo.normal;
        if (a + 1 < passingPointsVolume + 1) points.Add(hitInfo.point + hitInfo.normal * margin);
        if (a + 1 < passingPointsVolume + 1) normals.Add(hitInfo.normal);
    }
    private IEnumerator Wait(int a)
    {
        //Debug.Log("Wait0  " + a);
        if (count <= passingPointsVolume)   //�J�E���g��0�`�C���f�b�N�X�̍ő�+1�܂�
        {
            //Debug.Log("Wait1-0  " + a);
            yield return new WaitUntil(() => transform.position == points[a]);
            //Debug.Log("Wait1-1  " + a);
            count++;
            //Debug.Log("Count  " + count);
            if (count == passingPointsVolume)
            {
                if (owner_Ball == Owners.player0) toPlayerState = ToPlayerState.ToPlayer0;
                if (owner_Ball == Owners.player1) toPlayerState = ToPlayerState.ToPlayer1;
            }
            Move1();
        }

        //���]�̓���***********************************
        if (count == passingPointsVolume + 1)
        {
            //Debug.Log("Wait2-0  " + a);
            yield return new WaitUntil(() => transform.position == points[a]);
            //Debug.Log("Wait2-1  " + a);

            if (toPlayerState == ToPlayerState.ToPlayer0)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("WWWWWW0");
                    photonView.RPC(nameof(W), RpcTarget.All, "Idle", "player1", points[a - 1], normals[a], Vector3.zero);
                }
            }
            if(toPlayerState == ToPlayerState.ToPlayer1)
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("WWWWWW1");
                    photonView.RPC(nameof(W), RpcTarget.All, "Idle", "player0", points[a - 1], normals[a], Vector3.zero);
                }
            }
        }
    }
  

    private void Move()
    {
        //Debug.Log("Move");
        Vector3 goal;
        if (count < passingPointsVolume + 1)
        {
            //Debug.Log("Move  " + count);
            goal = points[count];
            transform.position = Vector3.MoveTowards(transform.position, goal, speed);
        }
    }

    private void Move1()
    {
        if (count < passingPointsVolume + 1)
        {
            float distance = (transform.position - points[count]).magnitude;
            float time = distance / speed;
            this.transform.DOMove(points[count], time).SetEase(Ease.Linear);
        }
    }


    private Vector3 OutDestination_General(Vector3 inDirection, Vector3 contactPointNormal)
    {
        float inNormal_Volume_Magnitude = Mathf.Abs(Vector3.Dot(contactPointNormal, inDirection));

        Vector3 outNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 inNormal_Volume = outNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);

        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;

        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Wall);
        //Debug.DrawRay(origin, Spherecast_direction * 120, Color.black, 5f, false);

        Vector3 destination = hitInfo.point;
        //Debug.Log("Destination_Genaral" + hitInfo.point);

        return destination;
    }
    private Vector3 OutDestination_Flex(Vector3 inDirection, Vector3 contactPointNormal, Vector3 destinationElement2)
    {
        float inNormal_Volume_Magnitude = Mathf.Abs(Vector3.Dot(contactPointNormal, inDirection));

        Vector3 outNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 inNormal_Volume = outNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);

        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2 + offset;

        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask_Wall);
        //Debug.DrawRay(origin, Spherecast_direction * 120, Color.black, 5f, false);

        Vector3 destinationElement1 = hitInfo.point;
        Vector3 destination = new Vector3(destinationElement1.x, destinationElement1.y, destinationElement2.z);
        //Debug.Log("Destination_Flex" + hitInfo.point);

        return destination;
    }


    [SerializeField] LayerMask layerMask_Wall;
    [SerializeField] LayerMask layerMask_Racket_Collider;
    [SerializeField] List<GameObject> sphereCasts = new List<GameObject>();
    [SerializeField] int count_ProcessForwardDetection = 0;
    private void Process()
    {
        ProcessRacketDetection("Racket1", layerMask_Racket_Collider, 2f);
        ProcessRacketDetection("Racket0", layerMask_Racket_Collider, 1f);
        count_ProcessForwardDetection = 0;
    }
    private void ProcessRacketDetection(string name_ReflectorObject, LayerMask layerMask, float reflectMargin)
    {
        Vector3 velocity = rb.velocity;

        Vector3 direction;
        if (name_ReflectorObject == "Racket0") direction = (racket0.transform.position - transform.position).normalized;
        else if (name_ReflectorObject == "Racket1") direction = (racket1.transform.position - transform.position).normalized;
        else direction = velocity.normalized;

        Vector3 origin = transform.position;
        float colliderRadius = transform.localScale.x / 2;
        bool isHit = Physics.SphereCast(origin, colliderRadius, direction, out RaycastHit hitInfo, 10000f, layerMask);
        if (visualizeSphereCast)
        {
            //Debug.Log(name_ReflectorObject + ",  " + hitInfo.collider.gameObject);
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.white, 0.02f, false);
            //sphereCasts[count_ProcessForwardDetection - 1].transform.position = origin + direction * hitInfo.distance;
            //sphereCasts[count_ProcessForwardDetection - 1].transform.localScale = new Vector3(1, 1, 1) * colliderRadius * 2;
        }
        //Debug.Log("�v���Z�X1");
        if (!isHit) return;
        if (hitInfo.collider.gameObject.tag != name_ReflectorObject) return;
        //Debug.Log("�v���Z�X2");
        float distance = hitInfo.distance;
        float nextMoveDistance = speed * Time.fixedDeltaTime;
        //Debug.Log(distance + ", " + (nextMoveDistance + reflectMargin));
        if (distance > nextMoveDistance + reflectMargin) return;
        Debug.Log("�v���Z�X3");

        //���]�̓���***********************************
        //struckDirection = hitInfo.normal;
        //count = 0;
        //toPlayerState = ToPlayerState.Idle;
        //if (name_ReflectorObject == "Racket0")
        //{
        //    strikeState = StrikeState.StruckByPlayer0;
        //    owner_Ball = Owners.player1; Debug.Log("�I�[�i�[�`�F���W0  ���P�b�g");
        //}
        //else
        //{
        //    strikeState = StrikeState.StruckByPlayer1;
        //    owner_Ball = Owners.player0; Debug.Log("�I�[�i�[�`�F���W1  ���P�b�g");
        //}
        //points = new Vector3[passingPointsVolume + 1];
        //normals = new Vector3[passingPointsVolume + 1];
        //points[0] = transform.position;
        //for (int a = 0; a < passingPointsVolume + 1; a++)
        //{
        //    ProcessReflect_Middle(a);
        //    StartCoroutine(Wait(a));
        //}


        if (toPlayerState == ToPlayerState.ToPlayer0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("WWW0");
                photonView.RPC(nameof(W), RpcTarget.All, "StruckByPlayer0", "player1", Vector3.zero, Vector3.zero, hitInfo.normal);
            }
        }
        if(toPlayerState == ToPlayerState.ToPlayer1)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.Log("WWW1");
                photonView.RPC(nameof(W), RpcTarget.All, "StruckByPlayer1", "player0", Vector3.zero, Vector3.zero, hitInfo.normal);
            }
        }
    }

    

    [PunRPC]
    private void W(string strikeState, string owner_Ball,  Vector3 lastPoint, Vector3 lastNormal, Vector3 struckDirection)
    {
        toPlayerState = ToPlayerState.Idle;

        Debug.Log("WWW2");
        Enum.TryParse(strikeState, out StrikeState S); Debug.Log(S);
        Enum.TryParse(owner_Ball, out Owners O); Debug.Log(O);

        this.strikeState = S;
        this.owner_Ball = O;

        this.lastPoint = lastPoint;  //�O�̍Ō�
        this.lastNormal = lastNormal;  //�O�̖@��
        this.struckDirection = struckDirection;
        
        StartCoroutine(Reversal());
    }



    [SerializeField] GameObject[] targets_Array = null;
    [SerializeField] List<GameObject> targets_List = null;
    public Vector3 GetPlayerTargetPosition()
    {
        targets_Array = GameObject.FindGameObjectsWithTag("Targets");
        foreach (GameObject aim in targets_Array)
        {
            if (aim.GetComponent<Owner>().player == owner_Ball)
            {
                targets_List.Add(aim);
            }
        }

        int index_Random = UnityEngine.Random.Range(0, targets_List.Count);
        Vector3 targetPosition_Random = targets_List[index_Random].transform.position;

        targets_List.Clear();
        return targetPosition_Random;
    }
}