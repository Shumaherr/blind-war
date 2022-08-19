using UnityEngine;

public class Grenade : Item
{
    protected override void PassiveProps()
    {
    }

    protected override void Use()
    {
        Debug.Log("Grenade used");
    }
}