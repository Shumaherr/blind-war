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

    [SerializeField] protected List<Item> inventory = new List<Item>();

    public List<Item> Inventory
    {
        get => inventory;
        set => inventory = value;
    }

    public List<Perk> Perks { get; } = new List<Perk>();
    [SerializeField] protected BaseUnit baseUnit;


    protected virtual int Health
    {
        get => _health;
        set => _health = value;
    }
    public int Damage => _damage;
    public virtual int Moves
    {
        get => _moves;
        set => _moves = value;
    }

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

    protected abstract void InitMoves();

    public abstract void TakeDamage(int amount);


    private void InitHealth()
    {
        _health = baseUnit.MaxHealth;
    }

    private void InitDamage()
    {
        _damage = BaseUnit.BaseDamage;
    }

    public void InitUnit()
    {
        InitHealth();
        InitMoves();
        InitDamage();
        IsDead = false;
        
    }

    protected abstract void UnitDie();
    public Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }
}