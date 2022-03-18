using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item: MonoBehaviour
{
    [SerializeField] private Sprite icon;
    [SerializeField] private string name;

    public Sprite Icon => icon;

    protected abstract void PassiveProps();
    protected abstract void Use();
}
