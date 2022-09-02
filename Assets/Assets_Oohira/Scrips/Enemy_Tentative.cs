using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Tentative : MonoBehaviour
{
    [SerializeField] private Vector3 coordinate_Enemy = Vector3.zero;
    [SerializeField] GameObject player = null;
    [SerializeField] private float speed_Move = 1;
    [SerializeField] private float speed_Shoot = 10;
    [SerializeField] private float shoot_Interval = 20;
    [SerializeField] private bool while_Shoot = false;
    [SerializeField] bool turn = false;
    private Rigidbody rb = null;
    private ZoneDiffinitionOfRoom zone = null;
    private Pool_Bullet_Enemy pool_Bullet_Enemy = null;

    private void Start()
    {
        StartCoroutine(Init(3f));
    }
    private IEnumerator Init(float waitTime)
    {
        yield return new WaitForSeconds(3f);
        rb = GetComponent<Rigidbody>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        pool_Bullet_Enemy = GameObject.FindGameObjectWithTag("Pool").GetComponent<Pool_Bullet_Enemy>();
        rb.velocity = new Vector3(1, 0, 0) * speed_Move;
    }

    private void FixedUpdate()
    {
        coordinate_Enemy = zone.CoordinateFromPosition(transform.position);
        Move();
        Shoot();
    }

    private void Move()
    {
        if (coordinate_Enemy.x > 8 && !turn)
        {
            turn = true;
            rb.velocity *= -1;
        }
        if (coordinate_Enemy.x < -8 && turn)
        {
            turn = false;
            rb.velocity *= -1;
        }
    }

    private void Shoot()
    {
        if (!while_Shoot) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        GameObject bullet = pool_Bullet_Enemy.Object_Discharge(transform.position, transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = direction * speed_Shoot;
        while_Shoot = false;
        StartCoroutine(Switch_Shoot());
    }

    private IEnumerator Switch_Shoot()
    {
        yield return new WaitForSeconds(shoot_Interval);
        while_Shoot = true;
    }
}
