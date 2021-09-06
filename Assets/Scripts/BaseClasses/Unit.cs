using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected BaseUnit baseUnit;

    public BaseUnit BaseUnit => baseUnit;

    public abstract void InitMoves();
}
