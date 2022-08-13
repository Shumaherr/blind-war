using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[RequireComponent(typeof(IController))]
public class Unit : MonoBehaviour
{
    protected int _moves;

    protected int _health;

    protected int _damage;

    protected int _level;

    protected bool isDead;
    
    protected IController _controller;

    [SerializeField] protected List<Item> inventory = new List<Item>();
    public event Action<Unit> OnUnitSelected;

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

    protected void InitMoves()
    {
        Moves = baseUnit.Moves;
    }

    public void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
        Debug.Log("Taken "+ amount + " damage. Health: " + Health);
    }
    
    public void ChangeMoves(int moves = 1)
    {
        Moves = Math.Max(0, _moves - moves);
    }

    public bool CanMove()
    {
        return _moves > 0;
    }

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

    protected void UnitDie()
    {
        IsDead = true;
    }
    public Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }

    protected void Start()
    {
        _controller = GetComponent<IController>();
        InitUnit();
    }
   
}