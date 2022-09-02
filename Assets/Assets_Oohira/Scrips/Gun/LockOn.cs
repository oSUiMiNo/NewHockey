using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LockOn : MonoBehaviour
{
    protected Pool_LockOnOverlay pool_LockOnOverlay = null;
    [SerializeField] protected Pool_Bullet pool_Bullet = null;

    [SerializeField] protected GameObject target = null;
    [SerializeField] protected GameObject eye = null;
    [SerializeField] protected Vector3 eye_Position = Vector3.zero;
    [SerializeField] protected GameObject burrel_Location = null;
    [SerializeField] protected Vector3 position_Burrel_Location = Vector3.zero;

    [SerializeField] protected List<GameObject> lockOnOverlays_Ball = null;
    [SerializeField] protected List<GameObject> targets = null;
    [SerializeField] protected List<float> targetAngles = null;

    [SerializeField] protected GunShoot gunShoot = null;
    protected bool whileLockOn;
    protected bool shootMode;

    [SerializeField] protected Vector3 shootDirection = new Vector3(0, 0.5f, 1);
    [SerializeField] protected float shootForce = 100;

    private void Start()
    {
        pool_LockOnOverlay = GameObject.Find("ObjectPool").GetComponent<Pool_LockOnOverlay>();
        pool_Bullet = GameObject.FindGameObjectWithTag("Pool").GetComponent<Pool_Bullet>();
        eye = GameObject.Find("CenterEyeAnchor");
        gunShoot = GameObject.FindGameObjectWithTag("Gun").GetComponent<GunShoot>();
        Difine_BurrelLocation();
    }

    private void FixedUpdate()
    {
        if (Difine_ShootMode())
        {
            UpdateLockOn();
        }
    }

    protected abstract void OnTriggerStay(Collider other);
    
    protected abstract void OnTriggerExit(Collider other);
    
    protected abstract void Difine_WhileLockOn(Collider other);

    protected abstract bool Difine_ShootMode();

    protected abstract void Difine_BurrelLocation();

    protected abstract void StartLockOn(Collider other);

    protected abstract void UpdateLockOn();
    
    protected void StopLockOn(Collider other)
    {
        for (int a = 0; a < lockOnOverlays_Ball.Count; a++)
        {
            if (other.gameObject == targets[a])
            {
                pool_LockOnOverlay.Object_Hide(lockOnOverlays_Ball[a]);
                targets.Remove(targets[a]);
                lockOnOverlays_Ball.Remove(lockOnOverlays_Ball[a]);
            }
        }
    }

    protected abstract IEnumerator ShootPerformance();

    protected abstract IEnumerator Shoot(GameObject target);
}
