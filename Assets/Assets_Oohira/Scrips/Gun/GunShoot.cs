using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;

    public AudioClip fire;
    public AudioSource source;

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")]
    [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")]
    [SerializeField] private float shotPower = 10f;
    [Tooltip("Casing Ejection Speed")]
    [SerializeField] private float ejectPower = 150f;


    [SerializeField] private Pool_LockOnOverlay pool_LockOnOverlay = null;
    [SerializeField] private Pool_Bullet pool_Bullet = null;

    [SerializeField] public bool paralyzer = true;
    [SerializeField] public bool eliminator = false;
    [SerializeField] public bool decomposer = false;

    [SerializeField] GameObject eliminator_Mesh;
    [SerializeField] GameObject decomposer_Mesh;

    [SerializeField] private Vector3 shootDirection = new Vector3(0, 0, 1);
    [SerializeField] private float shootForce = 0;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        pool_LockOnOverlay = GameObject.FindGameObjectWithTag("Pool").GetComponent<Pool_LockOnOverlay>();
        pool_Bullet = GameObject.FindGameObjectWithTag("Pool").GetComponent<Pool_Bullet>();

        eliminator_Mesh.GetComponent<Renderer>().enabled = false;
        eliminator_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;

        decomposer_Mesh.GetComponent<Renderer>().enabled = false;
        decomposer_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;
    }

    void FixedUpdate()
    {
        //コントローラーのトリガーかマウスの左ボタンが押された場合に発射
        //if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Touch) || Input.GetMouseButtonDown(0))
        //{
        //    if(paralyzer)
        //    {
        //        //Calls animation on the gun that has the relevant animation events that will fire
        //        gunAnimator.SetTrigger("Fire");

        //        Shoot();
        //    }
        //}
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.Touch) || Input.GetMouseButtonDown(1))
        {
            ModeChange();
        }
    }

    private void ModeChange()
    {
        if(paralyzer && !eliminator && !decomposer)
        {
            Debug.Log("Eliminator");
            
            paralyzer = false;
            
            eliminator = true;
            eliminator_Mesh.GetComponent<Renderer>().enabled = true;
            //Eliminator_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
            pool_LockOnOverlay.Object_HideAll();
            return;
        }
        if (!paralyzer && eliminator && !decomposer)
        {
            Debug.Log("Decomposer");
            
            eliminator = false;
            eliminator_Mesh.GetComponent<Renderer>().enabled = false;
            //Eliminator_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;

            decomposer = true;
            decomposer_Mesh.GetComponent<Renderer>().enabled = true;
            //Decomposer_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = true;
            pool_LockOnOverlay.Object_HideAll();
            return;
        }
        if (!paralyzer && !eliminator && decomposer)
        {
            Debug.Log("Paralyzer");
            decomposer = false;
            decomposer_Mesh.GetComponent<Renderer>().enabled = false;
            //Decomposer_Mesh.transform.GetChild(0).gameObject.GetComponent<Renderer>().enabled = false;

            paralyzer = true;
            pool_LockOnOverlay.Object_HideAll();
            return;
        }
    }

    public void Shoot()
    {
        if (muzzleFlashPrefab)
        {
            //発射時のフラッシュエフェクト
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //音
            source.PlayOneShot(fire);

            //フラッシュエフェクトを消す
            Destroy(tempFlash, destroyTimer);
        }
        //弾丸が設定されてなかったらリターン
        if (!bulletPrefab) return;
        //弾丸を生成して飛ばす
        pool_Bullet.Object_Discharge(barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.transform.forward * shotPower);
    }

    ////This function creates a casing at the ejection slot
    //void CasingRelease()
    //{
    //    //Cancels function if ejection slot hasn't been set or there's no casing
    //    if (!casingExitLocation || !casingPrefab) return;

    //    //Create the casing
    //    GameObject tempCasing;
    //    tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
    //    //Add force on casing to push it out
    //    tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
    //    //Add torque to make casing spin in random direction
    //    tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

    //    //Destroy casing after X seconds
    //    Destroy(tempCasing, destroyTimer);
    //}

}

