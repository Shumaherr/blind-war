using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class Perk: MonoBehaviour
{
    protected bool IsPassive;

    public abstract void Use();
    
}