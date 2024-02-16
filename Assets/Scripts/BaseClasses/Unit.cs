using System;
using System.Collections.Generic;
using System.Linq;
using BaseClasses;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Unit : MonoBehaviour
{
    public delegate void OnUnitDieDelegate(Unit unit);

    [SerializeField] protected List<Item> inventory = new();
    [SerializeField] protected BaseUnit baseUnit;

    public IController Controller { get; set; }

    protected int _damage;

    protected int _health;

    protected int _level;
    protected int _moves;

    protected bool isDead;
    
    private Renderer[] _renderer;
    private SpriteRenderer _spriteRenderer;
    
    private Light2D[] _lights;

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
        set
        {
            _health = value >= 0 ? value : 0;
            if (_health == 0)
            {
                IsDead = true;
            }
        
        }
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
        Controller = GetComponent<IController>();
        
    }

    private void Awake()
    {
        _renderer = GetComponentsInChildren<Renderer>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _lights = GetComponentsInChildren<Light2D>();
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

    public void KillUnit()
    {
        Destroy(gameObject);
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
            ShowBaseSprite();
            ShowUnit();
            InitMoves();
        }
        else
        {
            ShowGeneralizedSprite();
            ChangeVisibility();
            ActivateTilePerls();
        }
    }

    private void ActivateTilePerls() {
        var cell = GetUnitCell();
        var tile = GameManager.Instance.Grid.GetTile(cell);
        var tilePerks = MapManager.Instance.TilesData[tile].PerkName;
        if (tilePerks.Length > 0)
        {
            var perk = new PerkFactory().GetPerk(tilePerks);
            Type type = perk.GetType();
            //If unit has perk of this type, then we don't need to add it
            if (Perks.Any(p => p.GetType() == type))
                return;
            Perks.Add(perk);
            perk?.Use();
        }

    }

    public void ChangeVisibility()
    {
        HideUnit();
        if (ControllerManager.Instance.IsNearAlienUnit(transform.position))
            ShowGeneralizedSprite();
    }
    
    public void HideUnit()
    {
        _renderer.ToList().ForEach(spriteRenderer => spriteRenderer.enabled = false);
        _lights.ToList().ForEach(light2D => light2D.enabled = false);
    }

    public void ShowUnit()
    {
        if(Owner.Type == PlayerType.AI)
            return;
        _renderer.ToList().ForEach(spriteRenderer => spriteRenderer.enabled = true);
        _lights.ToList().ForEach(light2D => light2D.enabled = true);
    }

    public void ShowGeneralizedSprite()
    {
        _spriteRenderer.sprite = BaseUnit.GeneralizedSprite;
        _spriteRenderer.enabled = true;
    }
    
    public void ShowBaseSprite()
    {
        _spriteRenderer.sprite = BaseUnit.BaseSprite;
        _spriteRenderer.enabled = true;
    }
    
}