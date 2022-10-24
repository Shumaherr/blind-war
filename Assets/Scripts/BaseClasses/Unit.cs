using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IController))]
public class Unit : MonoBehaviour
{
    public delegate void OnUnitDieDelegate(Unit unit);

    [SerializeField] protected List<Item> inventory = new();
    [SerializeField] protected BaseUnit baseUnit;

    protected IController _controller;

    protected int _damage;

    protected int _health;

    protected int _level;
    protected int _moves;

    protected bool isDead;

    public Player Owner { get; private set; }

    public List<Item> Inventory
    {
        get => inventory;
        set => inventory = value;
    }

    public List<Perk> Perks { get; } = new();


    protected virtual int Health
    {
        get => _health;
        set => _health = value;
    }

    public int Damage => _damage;

    public int Moves
    {
        get => _moves;
        private set
        {
            _moves = value;
            OnMovesChanged?.Invoke(_moves, BaseUnit.Moves);
        }
    }

    public bool IsDead
    {
        get => isDead;
        set
        {
            isDead = value;
            if (value)
                OnUnitDie?.Invoke(this);
        }
    }

    public BaseUnit BaseUnit => baseUnit;

    protected void Start()
    {
        _controller = GetComponent<IController>();
        InitUnit();
    }

    public event Action<Unit> OnUnitSelected;
    public event Action<int, int> OnMovesChanged;
    public event OnUnitDieDelegate OnUnitDie;

    protected void InitMoves()
    {
        Moves = baseUnit.Moves;
    }

    private void OnEnable()
    {
        EventManager.StartListening("turnChanged", OnTurnChanged);
    }

    private void OnDisable()
    {
        EventManager.StopListening("turnChanged", OnTurnChanged);
    }

    public void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
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
        OnUnitDie?.Invoke(this);
    }

    public Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }

    private void OnTurnChanged(Dictionary<string, object> dictionary)
    {
        var newTurn = (Player) dictionary["newTurn"];
        if (newTurn == Owner)
            InitMoves();
    }
}