using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Bullet_Enemy : Pool_Object
{
    protected override void Define_Object()
    {
        Object = GameObject.Find("Bullet_Enemy");
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
