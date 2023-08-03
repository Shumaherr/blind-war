using System;
using UnityEngine;

[Serializable]
public abstract class Perk
{
    protected bool IsPassive;

    public abstract void Use();
    protected abstract void SubscribeToEvents();
}