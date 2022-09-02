using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Reflection1 : MonoBehaviour
{
	private Rigidbody rb;
	[SerializeField] private float bounce = 3;
	[SerializeField] private Vector3 confirmForward = new Vector3(0, 0, 0);
	[SerializeField] private Vector3 confirmVelocity = new Vector3(0, 0, 0);
	[SerializeField] private float confirmMagnitude = 0f;
	[SerializeField] private Vector3 v;
	[SerializeField] private Vector3 vFor;
	[SerializeField] private float vMag;
	[SerializeField] private float vMag_Max = 5;
	[SerializeField] private float vMag_Min = 1;
	[SerializeField] bool isGravity = true;
	[SerializeField] float gravity = -0.5f;
	bool first = true;
	private void Start()
	{
		first = true;
		rb = this.gameObject.GetComponent<Rigidbody>();
		rb.AddForce(500, 50, 300);
	}
	private void Update()
	{

	}
	private void FixedUpdate()
	{
		if (isGravity == true)
		{
			rb.AddForce(0, gravity * -1, 0);
		}

		//confirmVelocity = rb.velocity;
		//confirmMagnitude = rb.velocity.magnitude;
		//confirmForward = rb.velocity / rb.velocity.magnitude;
		//v = rb.velocity;
		//vMag = rb.velocity.magnitude;
		//vFor = rb.velocity / Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		//if (vMag < vMag_Min)
		//{
		//	vMag = vMag_Min;
		//}
		//if (vMag > vMag_Max)
		//{
		//	vMag = vMag_Max;
		//}
		//vFor = v / Mathf.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		//v = vMag * vFor;
		//rb.velocity = v;
		//ë¨ìxÇÃâ∫å¿è„å¿
		//if (vMag < 0)
		//      {
		//	vFor = v / vMag * -1;
		//      }
		//      else
		//      {
		//	vFor = v / vMag;
		//}
		//rb.velocity = v;
	}
	private void OnTriggerEnter(Collider other)
	{

		//vMag = vMag * bounce;
		//rb.velocity = rb.velocity * -1;
	}
	private void OnCollisionEnter(Collision collision)
	{
		//	if(first)
		//       {
		//		v = rb.velocity;
		//		vMag = rb.velocity.magnitude;
		//		vFor = rb.velocity / vMag;
		//		first = false;
		//	}
		//	else
		//       {
		//		vMag = vMag * bounce;
		//		v = vMag * vFor * -1;
		//           if (vMag < vMag_Min)
		//           {
		//               vMag = vMag_Min;
		//           }
		//           if (vMag > vMag_Max)
		//           {
		//               vMag = vMag_Max;
		//           }
		//           vFor = v / vMag;
		//		rb.velocity = v;
		//	}
	}
}