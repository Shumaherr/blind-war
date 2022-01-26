using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : Item
{
    protected override void PassiveProps()
    {
        return;
    }

    protected override void Use()
    {
        //TODO
        Destroy(this.gameObject);
    }
}
