using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected int _moves;

    protected int _health;

    protected int _damage;

    protected int _level;

    protected bool isDead;

    protected List<Item> inventory;
    protected List<Action> perks;
    [SerializeField] protected BaseUnit baseUnit;


    protected virtual int Health
    {
        get => _health;
        set => _health = value;
    }
    public int Damage => _damage;
    public int Moves => _moves;

    public bool IsDead
    {
        get => isDead;
        set
		{
			isDead = value;
			if(value)
				OnUnitDie?.Invoke(this);
		}
    }

    public BaseUnit BaseUnit => baseUnit;

	public delegate void OnUnitDieDelegate(Unit unit);
    public event OnUnitDieDelegate OnUnitDie;

    public abstract void InitMoves();

    public void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
		Debug.Log("Taken "+ amount + " damage. Health: " + Health);
    }

    private void InitHealth()
    {
        _health = baseUnit.MaxHealth;
    }

    private void InitDamage()
    {
        _damage = BaseUnit.BaseDamage;
    }

    protected void InitUnit()
    {
        InitHealth();
        InitMoves();
        InitDamage();
        IsDead = false;
    }

    protected abstract void UnitDie();
    public abstract Vector3Int GetUnitCell();
}