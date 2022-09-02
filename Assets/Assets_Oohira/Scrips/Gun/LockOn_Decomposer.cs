using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOn_Decomposer : LockOn
{
    protected override void OnTriggerStay(Collider other)
    {
        Difine_WhileLockOn(other);
        StartLockOn(other);
    }
    protected override void OnTriggerExit(Collider other)
    {
        StopLockOn(other);
    }
    protected override bool Difine_ShootMode()
    {
        return gunShoot.decomposer;
    }
    protected override void Difine_BurrelLocation()
    {
        burrel_Location = burrel_Location = GameObject.Find("Barrel_Location_Decomposer");
    }
    protected override void Difine_WhileLockOn(Collider other)
    {
        whileLockOn = (other.gameObject.tag == "Ball" || other.gameObject.tag == "Bullet1");
    } 
    protected override void StartLockOn(Collider other)
    {
        if (whileLockOn)
        {
            GameObject target = other.gameObject;
            if (targets.Contains(target)) return;
            targets.Add(target);

            Vector3 position_Target = target.transform.position;
            Vector3 direction_FromEye = (eye_Position - position_Target).normalized;

            //GameObject lockOnOverlay_Ball = pool_LockOnOverlay.Object_Discharge(position_Target - direction_FromEye * 0.2f, Quaternion.LookRotation(direction_FromEye));
            GameObject lockOnOverlay_Ball = pool_LockOnOverlay.Object_Discharge(position_Target, Quaternion.LookRotation(direction_FromEye));
            lockOnOverlay_Ball.SetActive(false);
            lockOnOverlays_Ball.Add(lockOnOverlay_Ball);
        }
    }
    protected override void UpdateLockOn()
    {
        position_Burrel_Location = burrel_Location.transform.position;
        eye_Position = eye.transform.position;
        for (int a = 0; a < lockOnOverlays_Ball.Count; a++)
        {
            GameObject target = targets[a];
            Vector3 position_Target = target.transform.position;
            var direction_FromEye = (eye_Position - position_Target).normalized;
            lockOnOverlays_Ball[a].transform.position = position_Target;
            lockOnOverlays_Ball[a].transform.rotation = Quaternion.LookRotation(direction_FromEye);
            lockOnOverlays_Ball[a].SetActive(true);

        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch) || Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootPerformance());
            for (int a = 0; a < lockOnOverlays_Ball.Count; a++)
            {
                GameObject target = targets[a];
                StartCoroutine(Shoot(target));
                //if (target.tag == "Ball")
                //{
                //    Rigidbody rb = target.GetComponent<Rigidbody>();
                //    Ball_BasicMove ballMove = target.GetComponent<Ball_BasicMove>();
                //    rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
                //    ballMove.canChangeSpeed = true;
                //    ballMove.canKeepSpeed = false;
                //}
                //if(target.tag == "Bullet1")
                //{
                //    pool_Bullet.Object_Hide(target);
                //    StopLockOn(target.GetComponent<Collider>());
                //}
            }
        }
    }
    [SerializeField] GameObject Beam = null;
    protected override IEnumerator ShootPerformance()
    {
        Instantiate(Beam, position_Burrel_Location, burrel_Location.transform.rotation, burrel_Location.transform);
        yield return new WaitForSeconds(4f);
        gunShoot.Shoot();
        //gunShoot.source.PlayOneShot(gunShoot.fire);
    }
    protected override IEnumerator Shoot(GameObject target)
    {
        yield return new WaitForSeconds(4f);
        if (target.tag == "Ball")
        {
            Debug.Log("Shoot Ball");
            Rigidbody rb = target.GetComponent<Rigidbody>();
            Ball_BasicMove ballMove = target.GetComponent<Ball_BasicMove>();
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
            ballMove.canChangeSpeed = true;
            ballMove.canKeepSpeed = false;
        }
        if (target.tag == "Bullet1")
        {
            Debug.Log("Shoot Bullet");
            pool_Bullet.Object_Hide(target);
            StopLockOn(target.GetComponent<Collider>());
        }
    }
}
