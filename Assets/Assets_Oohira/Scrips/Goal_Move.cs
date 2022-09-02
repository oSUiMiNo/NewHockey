using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal_Move : MonoBehaviour
{
    [SerializeField] private Vector3 coordinate_Goal = Vector3.zero;
    [SerializeField] private float speed_Move = 1;
    [SerializeField] bool turn = false;
    private Rigidbody rb = null;
    private ZoneDiffinitionOfRoom zone = null;

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
        Debug.Log(1);
        rb = GetComponent<Rigidbody>();
        zone = GameObject.Find("RoomCore").GetComponent<ZoneDiffinitionOfRoom>();
        rb.velocity = new Vector3(1, 0, 0) * speed_Move;
        state = State.Ready;
    }

    private void FixedUpdate()
    {
        if (state != State.Ready) return;
        //Debug.Log(2);
        coordinate_Goal = zone.CoordinateFromPosition(transform.position);
        Move();
    }

    private void Move()
    {
        if (coordinate_Goal.x > 7 && !turn)
        {
            turn = true;
            rb.velocity *= -1;
        }
        if (coordinate_Goal.x < -7 && turn)
        {
            turn = false;
            rb.velocity *= -1;
        }
    }
}
