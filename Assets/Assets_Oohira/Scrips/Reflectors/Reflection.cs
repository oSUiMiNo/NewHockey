using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflection
{
    public float Speed { get; private set; }
    public Vector3 Direction { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 Destination { get; private set; }

    public Reflection(GameObject target, Vector3 inDirection, Vector3 contactPointNormal, float inSpeed, float outSpeed, Vector3 fixedPosition)
    {
        OutDestination(inDirection, contactPointNormal, inSpeed, outSpeed);
    }

    private void OutDestination(Vector3 inDirection, Vector3 contactPointNormal, float inSpeed, float outSpeed)
    {
        float inNormal_Volume_Magnitude = Vector3.Dot(contactPointNormal, inDirection);

        Vector3 inNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 outNormal_Volume = inNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 destination = horizontal_Volume + outNormal_Volume;

        Destination = destination;
        Direction = Destination.normalized;
        Speed = outSpeed;
        Velocity = Direction * Speed;
    }
}
