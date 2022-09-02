using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Pool_Bullet pool_Bullet = null;
    [SerializeField] private ZoneDiffinitionOfRoom zone = null;
    [SerializeField] private Vector3 coordinate_Bullet = Vector3.zero;

    private enum State
    {
        Wait,
        Ready
    }
    private State state;
    private void Start()
    {
        StartCoroutine(Init());
    }
    private IEnumerator Init()
    {
        state = State.Wait;
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        pool_Bullet = GameObject.FindGameObjectWithTag("Pool").GetComponent<Pool_Bullet>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        state = State.Ready;
    }


    private void FixedUpdate()
    {
        if (state != State.Ready) return;
        coordinate_Bullet = zone.CoordinateFromPosition(transform.position);
        CollectBullet();
    }

    private void CollectBullet()
    {
        if (Mathf.Abs(coordinate_Bullet.x) < 11 && Mathf.Abs(coordinate_Bullet.y) < 11 && Mathf.Abs(coordinate_Bullet.z) < 11) return;
        //Debug.Log("Hide2");
        pool_Bullet.Object_Hide(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("‚ ‚½‚Á‚½I" + other.tag);
        if (other.tag != "Wall" && other.tag != "Racket1" && other.tag != "Bullet1" && other.tag != "Bullet0" && other.tag != "Enemy" && other.tag != "Ball") return;
        //if (other.tag != "Wall") return;
        //Debug.Log("Hide1");
        pool_Bullet.Object_Hide(this.gameObject);
    }
}
