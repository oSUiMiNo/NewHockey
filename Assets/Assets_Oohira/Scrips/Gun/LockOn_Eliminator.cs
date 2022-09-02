using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LockOn_Eliminator : LockOn
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
        return gunShoot.eliminator;
    }
    protected override void Difine_BurrelLocation()
    {
        burrel_Location = burrel_Location = GameObject.Find("Barrel_Location_Eliminator");
    }
    protected override void Difine_WhileLockOn(Collider other)
    {
        whileLockOn = (other.gameObject.tag == "Ball");
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

            GameObject lockOnOverlay_Ball = pool_LockOnOverlay.Object_Discharge(position_Target, Quaternion.LookRotation(direction_FromEye));
            lockOnOverlay_Ball.SetActive(false);
            lockOnOverlays_Ball.Add(lockOnOverlay_Ball);
        }
    }
    protected override void UpdateLockOn()
    {
        eye_Position = eye.transform.position;

        for (int a = 0; a < lockOnOverlays_Ball.Count; a++)
        {
            position_Burrel_Location = burrel_Location.transform.position;
            GameObject target = targets[a];
            Vector3 position_Target = target.transform.position;
            Vector3 direction_FromGun = (position_Target - position_Burrel_Location).normalized;
            Vector3 DiagonalLine = burrel_Location.transform.forward;
            float angle_Target = Vector3.Angle(DiagonalLine, direction_FromGun);
            targetAngles.Add(angle_Target);
        }

        float angle_Target_Min = targetAngles.Min();
        int key_Target = 0;
        for (int a = 0; a < lockOnOverlays_Ball.Count; a++)
        {
            if (targetAngles[a] == angle_Target_Min) key_Target = a;
            else Debug.Log("‚È‚ñ‚Å‚âI");
        }

        GameObject target_Narrowed = targets[key_Target];
        GameObject lockOnOverlays_Ball_Narrowed = lockOnOverlays_Ball[key_Target];

        Vector3 position_Target_Narrowed = target_Narrowed.transform.position;
        var direction_FromEye = (eye_Position - position_Target_Narrowed).normalized;

        lockOnOverlays_Ball_Narrowed.transform.position = position_Target_Narrowed;
        lockOnOverlays_Ball_Narrowed.transform.rotation = Quaternion.LookRotation(direction_FromEye);
        lockOnOverlays_Ball_Narrowed.SetActive(true);

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Touch) || Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ShootPerformance());
            //gunShoot.source.PlayOneShot(gunShoot.fire);
            Rigidbody rb = target_Narrowed.GetComponent<Rigidbody>();
            Ball_BasicMove ballMove = target_Narrowed.GetComponent<Ball_BasicMove>();
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
            ballMove.canChangeSpeed = true;
            ballMove.canKeepSpeed = false;
        }
    }

    protected override IEnumerator ShootPerformance()
    {
        yield return new WaitForSeconds(0f);
        gunShoot.Shoot();
    }
    protected override IEnumerator Shoot(GameObject target)
    {
        yield return null;
    }
}
