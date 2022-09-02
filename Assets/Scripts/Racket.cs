using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Racket : MonoBehaviour
{
    [SerializeField] private float movepower = 3;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        Moove();
    }
    private void Moove()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 IdouHoukou = new Vector3(x, 0, z);
        rb.velocity = IdouHoukou * movepower;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}