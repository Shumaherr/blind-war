using UnityEngine;

public abstract class Perk : MonoBehaviour
{
    protected bool IsPassive;

    public abstract void Use();
}