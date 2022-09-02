using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_ImpactOverlay : Pool_Object
{
    protected override void Define_Object()
    {
        Object = GameObject.Find("ImpactOverlay");
    }

    protected override void Define_Quantity()
    {
        Quantity = 30;
    }

    protected override void Define_HideTime()
    {
        HideTime = 6f;
    }
}
