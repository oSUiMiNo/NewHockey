using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Reflector_NormalWall : Reflector
{
    [SerializeField] public Owners owner_Ball;
    [SerializeField] private GameObject line = null;
    private GameObject racket = null;

    float distance = 0;

    private Vector3 velocity = Vector3.zero;
    private Vector3 inDirection;
    private float sphereCastMargin = 0;
    private Vector3 normal;
    private RaycastHit hitInfo;

    //private static Vector3 velocity = Vector3.zero;
    //private static Vector3 inDirection;
    //private static float sphereCastMargin = 0;
    //private static Vector3 normal;

    [SerializeField] GameObject debugMarker;

    [SerializeField] private LayerMask layerMask;

    protected override void Init_Child() 
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }
    protected override void Define_RefloctMargin() { reflectMargin = 1.3f; }

    public override void Reflect(GameObject target, Vector3 velocity, Vector3 inDirection, RaycastHit hitInfo, float sphereCastMargin)
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        GetBallInfo(target);
        //photonView.RPC(nameof(GetBallInfo_RPC), RpcTarget.All, target.name);

        //distance = hitInfo.distance - sphereCastMargin;
        //float nextMoveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        //if (distance > nextMoveDistance + reflectMargin) return;
        
        ballMove.Create_ImpactPointOverlay(hitInfo);
        owner_Ball = ball.GetComponent<Owner>().player;

        this.velocity = velocity;
        this.inDirection = inDirection;
        this.sphereCastMargin = sphereCastMargin;
        this.normal = hitInfo.normal;
        //this.hitInfo = hitInfo;

        //SetProp(velocity, inDirection, hitInfo, sphereCastMargin, distance);
        //GetProp();

        if (GetComponent<WallType>().type == WallTypes.BackWall)
        {
            Debug.Log("プレイヤー0の壁に当たった");
            ballMove.wasStruck_ByPlayer0[0] = true;
            ballMove.wasStruck_ByPlayer0[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer1.Length; a++)
            {
                ballMove.wasStruck_ByPlayer1[a] = false;
            }
            avatar0.Charge(-0.5f);
        }
        else if (GetComponent<WallType>().type == WallTypes.BackWall)
        {
            Debug.Log("プレイヤー1の壁に当たった");
            ballMove.wasStruck_ByPlayer1[0] = true;
            ballMove.wasStruck_ByPlayer1[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer0.Length; a++)
            {
                ballMove.wasStruck_ByPlayer0[a] = false;
            }
        }

        if (ballMove.wasStruck_ByPlayer0[0])
        {
            Debug.Log("プレイヤー0が打った");
            owner_Ball = Owners.player1;
            line = Line1;
            racket = rackets.racket1_Core;

            if (ballMove.wasStruck_ByPlayer0[1])
            {
                Debug.Log("プレイヤー0reflect1");
                if (zone.CoordinateFromPosition(ball.transform.position).z <= -5) ballMove.divisionPointsVolimeForReflect = 4;
                else if (zone.CoordinateFromPosition(ball.transform.position).z <= -3) ballMove.divisionPointsVolimeForReflect = 2;
                else if (zone.CoordinateFromPosition(ball.transform.position).z <= -1) ballMove.divisionPointsVolimeForReflect = 1;
                else ballMove.divisionPointsVolimeForReflect = 1;
                ballMove.wasStruck_ByPlayer0 = new bool[ballMove.divisionPointsVolimeForReflect + 3];
                for (int a = 0; a < ballMove.divisionPointsVolimeForReflect + 3; a++)
                {
                    ballMove.wasStruck_ByPlayer0[a] = true;
                }

                Reflect_First();
                ballMove.wasStruck_ByPlayer0[1] = false;
            }
            else if (ballMove.wasStruck_ByPlayer0[2 + ballMove.reflectCounter] && (2 + ballMove.reflectCounter < ballMove.wasStruck_ByPlayer0.Length - 1))
            {
                Debug.Log("プレイヤー0reflect2");
                Reflect_Middle();
                ballMove.wasStruck_ByPlayer0[2 + ballMove.reflectCounter] = false;
                ballMove.reflectCounter++;
            }
            else if (ballMove.wasStruck_ByPlayer0[ballMove.wasStruck_ByPlayer0.Length - 1])
            {
                Debug.Log("プレイヤー0reflect3");
                Reflect_Final();
                ballMove.wasStruck_ByPlayer0[ballMove.wasStruck_ByPlayer0.Length - 1] = false;
                ballMove.reflectCounter = 0;
            }

            if (!ballMove.wasStruck_ByPlayer0[ballMove.wasStruck_ByPlayer0.Length - 1]) ballMove.wasStruck_ByPlayer0[0] = false;
        }
        else if (ballMove.wasStruck_ByPlayer1[0])
        {
            Debug.Log("プレイヤー1が打った");
            owner_Ball = Owners.player0;
            line = Line0;
            racket = rackets.racket0_Core;

            if (ballMove.wasStruck_ByPlayer1[1])
            {
                Debug.Log("プレイヤー1reflect1");
                if (zone.CoordinateFromPosition(ball.transform.position).z >= 5) ballMove.divisionPointsVolimeForReflect = 4;
                else if (zone.CoordinateFromPosition(ball.transform.position).z >= 3) ballMove.divisionPointsVolimeForReflect = 2;
                else if (zone.CoordinateFromPosition(ball.transform.position).z >= 1) ballMove.divisionPointsVolimeForReflect = 1;
                else ballMove.divisionPointsVolimeForReflect = 1;
                ballMove.wasStruck_ByPlayer1 = new bool[ballMove.divisionPointsVolimeForReflect + 3];
                for (int a = 0; a < ballMove.divisionPointsVolimeForReflect + 3; a++)
                {
                    ballMove.wasStruck_ByPlayer1[a] = true;
                }

                Reflect_First();
                ballMove.wasStruck_ByPlayer1[1] = false;
            }
            else if (ballMove.wasStruck_ByPlayer1[2 + ballMove.reflectCounter] && (2 + ballMove.reflectCounter < ballMove.wasStruck_ByPlayer1.Length - 1))
            {
                Debug.Log("プレイヤー1reflect2");
                Reflect_Middle();
                ballMove.wasStruck_ByPlayer1[2 + ballMove.reflectCounter] = false;
                ballMove.reflectCounter++;
            }
            else if (ballMove.wasStruck_ByPlayer1[ballMove.wasStruck_ByPlayer1.Length - 1])
            {
                Debug.Log("プレイヤー1reflect3");
                Reflect_Final();
                ballMove.wasStruck_ByPlayer1[ballMove.wasStruck_ByPlayer1.Length - 1] = false;
                ballMove.reflectCounter = 0;
            }
            if (!ballMove.wasStruck_ByPlayer1[ballMove.wasStruck_ByPlayer1.Length - 1]) ballMove.wasStruck_ByPlayer1[0] = false;
        }
        else
        {
            Debug.Log("reflect0");
            Reflect_Normal();
        }

        // 衝突予定先に接するように速度を調整
        var adjustVelocity = distance / Time.fixedDeltaTime * inDirection;
        ballMove.canKeepSpeed = false;
        rb.velocity = adjustVelocity;

        ballMove.ApplyReboundVelocity();
    }

  
    private void Reflect_First()
    {
        Vector3 start = ball.transform.position;
        Vector3 lineDirection = line.transform.position - start;
        int divisionVolume = ballMove.divisionPointsVolimeForReflect + 1;
        ballMove.distance_Z = (lineDirection / divisionVolume).z;
        Vector3 goal_Z = new Vector3(0, 0, start.z + ballMove.distance_Z);
        FlexibleReflection reflectInfo = new FlexibleReflection(ball, inDirection, normal, ballMove.constantSpeed, ballMove.constantSpeed, goal_Z, layerMask);
        Vector3 goal = reflectInfo.Destination;
        Vector3 outDirection = Vector3.Normalize(goal - start);
        ballMove.reboundVelocity = outDirection * ballMove.constantSpeed;
        //SetBallProps(start, outDirection * ballMove.constantSpeed);
    }
    private void Reflect_Middle()
    {
        Vector3 start = ball.transform.position;
        Vector3 goal_Z = new Vector3(0, 0, start.z + ballMove.distance_Z);
        FlexibleReflection reflectInfo = new FlexibleReflection(ball, inDirection, normal, ballMove.constantSpeed, ballMove.constantSpeed, goal_Z, layerMask);
        Vector3 goal = reflectInfo.Destination;
        Vector3 outDirection = Vector3.Normalize(goal - start);
        ballMove.reboundVelocity = outDirection * ballMove.constantSpeed;
        //SetBallProps(start, outDirection * ballMove.constantSpeed);
    }
    private void Reflect_Final()
    {    
        Vector3 start = ball.transform.position;
        Vector3 direction = GetPlayerTargetPosition() - start;
        Vector3 goal = new Vector3(start.x + direction.x, start.y + direction.y, start.z + direction.z);
        Vector3 outDirection = Vector3.Normalize(goal - start);
        ballMove.reboundVelocity = outDirection * ballMove.constantSpeed;
        //SetBallProps(start, outDirection * ballMove.constantSpeed);
    }
    private void Reflect_Normal()
    {
        Vector3 start = ball.transform.position;
        Reflection outDirection = new Reflection(ball, inDirection, normal, ballMove.constantSpeed, ballMove.constantSpeed + 0.02f, Vector3.zero);
        ballMove.reboundVelocity = outDirection.Velocity;
        ballMove.constantSpeed = outDirection.Velocity.magnitude;
        //Debug.Log(outDirection.Velocity);
        //SetBallProps(start, outDirection.Velocity);
    }

    [SerializeField] GameObject[] targets_Array = null;
    [SerializeField] List<GameObject> targets_List = null;
    public Vector3 GetPlayerTargetPosition()
    {
        targets_Array = GameObject.FindGameObjectsWithTag("Targets");
        foreach (GameObject aim in targets_Array)
        {
            //Debug.Log("ゴールリスト作る1" + goal);
            //Debug.Log("ゴールリスト作る2" + GetComponent<Reflector_Racket>().playerIdentification);
            //Debug.Log("ゴールリスト作る2" + goal.GetComponent<Goal>().playerIdentification);
            if (aim.GetComponent<Owner>().player == owner_Ball)
                targets_List.Add(aim);
        }
        
        int index_Random = Random.Range(0, targets_List.Count);
        Vector3 targetPosition_Random = targets_List[index_Random].transform.position;

        targets_List.Clear();
        return targetPosition_Random;
    }




    int volume = 4;
    private Vector3[] starts;
    private Vector3[] goals;
    public override void NewReflect(GameObject target, Vector3 velocity, Vector3 inDirection, RaycastHit hitInfo, float sphereCastMargin)
    {
        GetBallInfo(target);

        this.velocity = velocity;
        this.inDirection = inDirection;
        this.sphereCastMargin = sphereCastMargin;
        this.normal = hitInfo.normal;

        if (GetComponent<WallType>().type == WallTypes.BackWall)
        {
            Debug.Log("プレイヤー0の壁に当たった");
            ballMove.wasStruck_ByPlayer0[0] = true;
            ballMove.wasStruck_ByPlayer0[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer1.Length; a++)
            {
                ballMove.wasStruck_ByPlayer1[a] = false;
            }
            avatar0.Charge(-0.5f);
        }
        else if (GetComponent<WallType>().type == WallTypes.BackWall)
        {
            Debug.Log("プレイヤー1の壁に当たった");
            ballMove.wasStruck_ByPlayer1[0] = true;
            ballMove.wasStruck_ByPlayer1[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer0.Length; a++)
            {
                ballMove.wasStruck_ByPlayer0[a] = false;
            }
        }

        volume = 10;
        starts = new Vector3[volume];
        goals = new Vector3[volume];
        if (ballMove.wasStruck_ByPlayer0[0])
        {
            owner_Ball = Owners.player1;
            line = Line1;
            racket = rackets.racket1_Core;

            Debug.Log("新しい反射1");
            First();
            for (int a = 0; a < volume; a++)
            {
                ProcessReflect_Middle(a);
            }
            for (int a = 0; a < volume; a++)
            {
                StartCoroutine(Move(a));
            }
        }
        else if (ballMove.wasStruck_ByPlayer1[0])
        {
            owner_Ball = Owners.player0;
            line = Line0;
            racket = rackets.racket0_Core;

            Debug.Log("新しい反射2");
            First();
            for (int a = 0; a < volume; a++)
            {
                ProcessReflect_Middle(a);
            }
            for (int a = 0; a < volume; a++)
            {
                StartCoroutine(Move(a));
            }
        }
        else
        {
            Debug.Log("reflect0");
            Reflect_Normal();

            var adjustVelocity = distance / Time.fixedDeltaTime * inDirection;
            ballMove.canKeepSpeed = false;
            rb.velocity = adjustVelocity;

            ballMove.ApplyReboundVelocity();
        }


    }

    private void First()
    {
        starts[0] = ball.transform.position;
        Vector3 lineDirection = line.transform.position - starts[0];
        int divisionVolume = ballMove.divisionPointsVolimeForReflect + 1;
        ballMove.distance_Z = (lineDirection / divisionVolume).z;
    }
    private void ProcessReflect_Middle(int a)
    {
        Vector3 goal_Z = new Vector3(0, 0, starts[a].z + ballMove.distance_Z);
        FlexibleReflection reflectInfo = new FlexibleReflection(ball, inDirection, normal, ballMove.constantSpeed, ballMove.constantSpeed, goal_Z, layerMask);
        goals[a] = reflectInfo.Destination;
        if(a + 1 < volume) starts[a + 1] = goals[a];
    }
    private IEnumerator Move(int a)
    {
        Debug.DrawRay(starts[a], goals[a] - starts[a], Color.white, 8f, false);
        if (a > 0) yield return new WaitUntil(() => ball.transform.position == goals[a - 1]);
        ball.transform.position = Vector3.MoveTowards(starts[a], goals[a], ballMove.constantSpeed);
        Debug.Log("新しい反射3");
    }
    

}

