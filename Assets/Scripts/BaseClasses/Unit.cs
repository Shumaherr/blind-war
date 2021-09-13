using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] protected BaseUnit baseUnit;
    
    protected int _moves;

    public int Moves => _moves;

    public BaseUnit BaseUnit => baseUnit;

    public abstract void InitMoves();
}
