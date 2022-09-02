using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_LockOnOverlay : Pool_Object
{
    protected override void Define_Object()
    {
        Object = GameObject.Find("LockOnOverlay");
    }
    protected override void Define_Quantity()
    {
        Quantity = 15;
    }
    protected override void Define_HideTime()
    {
        HideTime = 0;
    }
}
