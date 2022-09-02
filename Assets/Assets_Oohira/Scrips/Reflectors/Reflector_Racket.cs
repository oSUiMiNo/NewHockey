using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reflector_Racket : Reflector
{
    //[SerializeField] float easeAngleMagnification = 2;
    [SerializeField] public Owner owner_Racket;

    [SerializeField] Ball_TezukaGravity tezukaGravity;
    [SerializeField] ParticleSystem StrikeEffect = null;
    [SerializeField] public AudioClip fire;
    [SerializeField] public AudioSource source;

    [SerializeField] Reflector_Smash reflector_Smash = null;
    [SerializeField] float Speed = 10;
    [SerializeField] float speedZ = 10f;
    [SerializeField] float speedY = 1f;

    [SerializeField] GameObject[] goals_Array = null;
    [SerializeField] List<GameObject> goals_List = null;
    [SerializeField] float smashSpeed = 20;

    protected override void Init_Child()
    {
        owner_Racket = GetComponent<Owner>();

        goals_Array = GameObject.FindGameObjectsWithTag("Goal");
        foreach (GameObject goal in goals_Array)
        {
            if (goal.GetComponent<Owner>().player != GetComponent<Owner>().player)
                goals_List.Add(goal);
        }
    }

    protected override void Define_RefloctMargin()
    {
        //reflectMargin = 1f;
    }


    public override void Reflect(GameObject target, Vector3 velocity, Vector3 direction, RaycastHit hitInfo, float sphereCastMargin)
    {
        GetBallInfo(target);

        //if (!ballMove.canReflect) return;
        //float distance = hitInfo.distance - sphereCastMargin;
        //float nextMoveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        //if (distance > nextMoveDistance + reflectMargin) return;

        if (ballMove.canChange_CanStrike == true) ballMove.canStrike = true;
        if (!ballMove.canStrike) return;

        ballMove.canChange_CanStrike = false;
        ballMove.canStrike = false;

        //Debug.Log("ラケットに当たった3");
        ballMove.canChangeSpeed = true;
        ballMove.canKeepSpeed = false;

        GameObject impactPointOverlay = ballMove.Create_ImpactPointOverlay(hitInfo);
        //Debug.Log("オーバーレイの向き2" + impactPointOverlay.transform.forward);
        //Instantiate(StrikeEffect, impactPointOverlay.transform.position, impactPointOverlay.transform.rotation);
        StrikeEffect.transform.position = impactPointOverlay.transform.position + impactPointOverlay.transform.forward;
        StrikeEffect.transform.rotation = impactPointOverlay.transform.rotation;
        StrikeEffect.Play();

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.Touch) || Input.GetMouseButton(0))
        {
            Debug.Log("スマッシュ");
            //reflector_Smash = GetComponent<Reflector_Smash>();
            //reflector_Smash.Smash();
            Smash();
            StartCoroutine(VibrateRightController(1f, 1f, 0.5f));
            StartCoroutine(B());
        }
        else
        {
            if (owner_Racket.player == Owners.player0)
            {
                //Speed = GetAcc().magnitude;
                //Speed = OVRInput.GetLocalControllerAcceleration(controller).magnitude;
                //Speed = StrikePower * 100;
                ball.GetComponent<Rigidbody>().velocity = impactPointOverlay.transform.forward * Speed;
                ballMove.changeSpeed = Speed;

                avatar0.Charge(0.05f);
                StartCoroutine(VibrateRightController(1f, 1f, 0.1f));
                Debug.Log("加速度" + Speed);
            }
            else
            {
                ball.GetComponent<Rigidbody>().velocity = impactPointOverlay.transform.forward * Speed;
                ballMove.changeSpeed = Speed;
            }
            StartCoroutine(A());
        }
        //UnityEditor.EditorApplication.isPaused = true;
        Debug.Log("strike");
        source.PlayOneShot(fire);

        if (owner_Racket.player == Owners.player0)
        {
            ballMove.wasStruck_ByPlayer0[0] = true;
            ballMove.wasStruck_ByPlayer0[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer1.Length; a++)
            {
                ballMove.wasStruck_ByPlayer1[a] = false;
            }
        }
        else
        {
            ballMove.wasStruck_ByPlayer1[0] = true;
            ballMove.wasStruck_ByPlayer1[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer0.Length; a++)
            {
                ballMove.wasStruck_ByPlayer0[a] = false;
            }
        }
    }

    private IEnumerator A()
    {
        //Debug.Log("コルーチンA");
        yield return new WaitForSeconds(1f);
        ballMove.canChange_CanStrike = true;
        //tezukaGravity.enabled = true;
    }
    private IEnumerator B()
    {
        yield return new WaitForSeconds(6f);
        ballMove.canChange_CanStrike = true;
        //tezukaGravity.enabled = true;
    }
    private IEnumerator VibrateRightController(float freqency, float amplitude, float time)
    {
        OVRInput.SetControllerVibration(freqency, amplitude, OVRInput.Controller.RTouch);
        yield return new WaitForSeconds(time);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
    }

    [SerializeField, Range(0f, 1f)] private float _powerAdjust = 0.2f;
    [SerializeField] private float _minReleasePower = 0.01f;
    [SerializeField] private float _maxReleasePower = 100f;
    [SerializeField] OVRInput.Controller controller = OVRInput.Controller.RTouch;
    private Vector3 GetAcc()
    {

        float mag = OVRInput.GetLocalControllerAcceleration(controller).magnitude;
        if (mag < _minReleasePower)
        {
            mag = 0f;
        }
        if (mag > _maxReleasePower)
        {
            mag = _maxReleasePower;
        }
        Vector3 acc = transform.forward * (mag * _powerAdjust);
        return acc;
    }



    private void Update()
    {
        ProcessStrikePower();
    }

    [SerializeField] Vector3 CurrentRacketPosition;
    [SerializeField] Vector3 LastRacketPosition;
    [SerializeField] float StrikePower;
    [SerializeField] float debugStrikePower;
    private void ProcessStrikePower()
    {
        CurrentRacketPosition = this.transform.position;

        if (LastRacketPosition != null)
            StrikePower = (LastRacketPosition - CurrentRacketPosition).magnitude;
        debugStrikePower = StrikePower * 100;
        LastRacketPosition = CurrentRacketPosition;
    }

    public void Smash()
    {
        Debug.Log("ゴールの数" + goals_List.Count);
        int index_Random = Random.Range(0, goals_List.Count);
        Vector3 goalPosition_Random = goals_List[index_Random].transform.position;

        Vector3 SmashDirection_Random = goalPosition_Random - ball.transform.position;
        goals_List.Clear();

        ball.GetComponent<Rigidbody>().velocity = SmashDirection_Random * smashSpeed;
        ballMove.changeSpeed = smashSpeed;
    }
    private IEnumerator A(float smashTime)
    {
        yield return new WaitForSeconds(smashTime);
    }




    public override void NewReflect(GameObject target, Vector3 velocity, Vector3 inDirection, RaycastHit hitInfo, float sphereCastMargin)
    {
        GetBallInfo(target);

        //if (!ballMove.canReflect) return;
        //float distance = hitInfo.distance - sphereCastMargin;
        //float nextMoveDistance = rb.velocity.magnitude * Time.fixedDeltaTime;
        //if (distance > nextMoveDistance + reflectMargin) return;

        if (ballMove.canChange_CanStrike == true) ballMove.canStrike = true;
        if (!ballMove.canStrike) return;

        ballMove.canChange_CanStrike = false;
        ballMove.canStrike = false;

        //Debug.Log("ラケットに当たった3");
        ballMove.canChangeSpeed = true;
        ballMove.canKeepSpeed = false;

        GameObject impactPointOverlay = ballMove.Create_ImpactPointOverlay(hitInfo);
        //Debug.Log("オーバーレイの向き2" + impactPointOverlay.transform.forward);
        //Instantiate(StrikeEffect, impactPointOverlay.transform.position, impactPointOverlay.transform.rotation);
        StrikeEffect.transform.position = impactPointOverlay.transform.position + impactPointOverlay.transform.forward;
        StrikeEffect.transform.rotation = impactPointOverlay.transform.rotation;
        StrikeEffect.Play();

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger, OVRInput.Controller.Touch) || Input.GetMouseButton(0))
        {
            Debug.Log("スマッシュ");
            //reflector_Smash = GetComponent<Reflector_Smash>();
            //reflector_Smash.Smash();
            Smash();
            StartCoroutine(VibrateRightController(1f, 1f, 0.5f));
            StartCoroutine(B());
        }
        else
        {
            if (owner_Racket.player == Owners.player0)
            {
                //Speed = GetAcc().magnitude;
                //Speed = OVRInput.GetLocalControllerAcceleration(controller).magnitude;
                //Speed = StrikePower * 100;
                ball.GetComponent<Rigidbody>().velocity = impactPointOverlay.transform.forward * Speed;
                ballMove.changeSpeed = Speed;

                avatar0.Charge(0.05f);
                StartCoroutine(VibrateRightController(1f, 1f, 0.1f));
                Debug.Log("加速度" + Speed);
            }
            else
            {
                ball.GetComponent<Rigidbody>().velocity = impactPointOverlay.transform.forward * Speed;
                ballMove.changeSpeed = Speed;
            }
            StartCoroutine(A());
        }
        //UnityEditor.EditorApplication.isPaused = true;
        Debug.Log("strike");
        source.PlayOneShot(fire);

        if (owner_Racket.player == Owners.player0)
        {
            ballMove.wasStruck_ByPlayer0[0] = true;
            ballMove.wasStruck_ByPlayer0[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer1.Length; a++)
            {
                ballMove.wasStruck_ByPlayer1[a] = false;
            }
        }
        else
        {
            ballMove.wasStruck_ByPlayer1[0] = true;
            ballMove.wasStruck_ByPlayer1[1] = true;
            for (int a = 0; a < ballMove.wasStruck_ByPlayer0.Length; a++)
            {
                ballMove.wasStruck_ByPlayer0[a] = false;
            }
        }
    }
}
