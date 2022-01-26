using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected string name;
    protected Sprite icon;

    protected abstract void PassiveProps();
    protected abstract void Use();
}
