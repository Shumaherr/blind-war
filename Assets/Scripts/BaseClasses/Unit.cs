using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected int _moves;

    protected int _health;

    protected int _level;

    protected List<Item> inventory;
    protected List<Action> perks;
    [SerializeField] protected BaseUnit baseUnit;


    protected virtual int Health
    {
        get => _health;
        set => _health = value;
    }
    public int Moves => _moves;

    public BaseUnit BaseUnit => baseUnit;

    public abstract void InitMoves();

    protected void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
    }
    
    public void InitHealth()
    {
        _health = baseUnit.MaxHealth;
    }
    public abstract Vector3Int GetUnitCell();
}