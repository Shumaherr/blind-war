using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    private Renderer[] _renderer;
    private SpriteRenderer _spriteRenderer;

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
        
    }

    private void Awake()
    {
        _renderer = GetComponentsInChildren<Renderer>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public event Action<int, int> OnMovesChanged;
    public event OnUnitDieDelegate OnUnitDie;

    public void InitMoves()
    {
        Moves = baseUnit.Moves;
        _spriteRenderer.sprite = BaseUnit.BaseSprite;
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

    public void InitUnit(Player owner)
    {
        InitHealth();
        InitMoves();
        InitDamage();
        Owner = owner;
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
        var newTurn = (Player) dictionary["whoseTurn"];
        if (newTurn == Owner)
        {
            ShowUnit();
            InitMoves();
        }
        else
        {
            HideUnit();
        }
    }
    
    public void ChangeVisibility()
    {
        if (ControllerManager.Instance.IsNearPlayerUnit(transform.position))
            ShowUnit();
        else
            HideUnit();
    }
    
    public void HideUnit()
    {
        _renderer.ToList().ForEach(spriteRenderer => spriteRenderer.enabled = false);
    }

    public void ShowUnit()
    {
        _renderer.ToList().ForEach(spriteRenderer => spriteRenderer.enabled = true);
    }

    public void ShowGeneralizedSprite()
    {
        _spriteRenderer.sprite = BaseUnit.GeneralizedSprite;
        _spriteRenderer.enabled = true;
    }
    
}