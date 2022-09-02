using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Bullet : Pool_Object
{
    protected override void Define_Object()
    {
        Object = GameObject.Find("Bullet");
    }

    protected override void Define_Quantity()
    {
        Quantity = 20;
    }
    
    protected override void Define_HideTime()
    {
        HideTime = 0;
    }
}
