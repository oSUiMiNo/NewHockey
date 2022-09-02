using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexibleReflection
{
    public float Speed { get; private set; }
    public Vector3 Direction { get; private set; }
    public Vector3 Velocity { get; private set; }
    public Vector3 Destination { get; private set; }

    private const float sphereCastMargin = 0.01f; // SphereCast用のマージン

    public FlexibleReflection(GameObject target, Vector3 inDirection, Vector3 contactPointNormal, float inSpeed, float outSpeed, Vector3 Destination_Elemen2, LayerMask layerMask)
    {
        OutDestination(target, inDirection, contactPointNormal, inSpeed, outSpeed, Destination_Elemen2, layerMask);
    }

    private void OutDestination(GameObject target, Vector3 inDirection, Vector3 contactPointNormal, float inSpeed, float outSpeed, Vector3 destinationElement2, LayerMask layerMask)
    {
        float inNormal_Volume_Magnitude = Vector3.Dot(contactPointNormal, inDirection);

        Vector3 inNormal_Volume = inNormal_Volume_Magnitude * contactPointNormal;

        Vector3 outNormal_Volume = inNormal_Volume * -1;
        Vector3 horizontal_Volume = inDirection - inNormal_Volume;

        Vector3 Spherecast_direction = Vector3.Normalize(horizontal_Volume + outNormal_Volume);
        
        float offset = Physics.defaultContactOffset * 2;
        Vector3 origin = target.transform.position - Spherecast_direction * (sphereCastMargin + offset);
        float colliderRadius = target.transform.localScale.x / 2 + offset;
        Physics.SphereCast(origin, colliderRadius, Spherecast_direction, out RaycastHit hitInfo, 10000f, layerMask);

        Vector3 destinationElemen1 = hitInfo.point;
        Vector3 destination = new Vector3(destinationElemen1.x, destinationElemen1.y, destinationElement2.z);

        //Instantiate(debugMarker, outDirection2, Quaternion.identity);

        Destination = destination;
        Direction = Destination.normalized;
        Speed = outSpeed;
        Velocity = Direction * Speed;
    }
}
