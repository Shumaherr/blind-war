using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EnemyUnitBase : Unit
{
    protected List<Vector3Int> _currentPath;
    private SpriteRenderer _renderer;

    protected override int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (value <= 0)
                UnitDie();
        }
    }

    private void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _renderer.sprite = BaseUnit.GeneralizedSprite;
        //InitUnit();
    }

    private void Update()
    {
        ChangeVisibility();
    }

    public void HideUnit()
    {
        _renderer.enabled = false;
    }

    public void ShowUnit()
    {
        _renderer.enabled = true;
    }

    protected void InitMoves()
    {
        _moves = baseUnit.Moves;
    }

    public void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
        Debug.Log("Taken " + amount + " damage. Health: " + Health);
    }

    public void ChangeMoves(int moves = 1)
    {
        _moves = Math.Max(0, _moves - moves);
    }

    public virtual void DoMove()
    {
    }

    public virtual void DoTurn()
    {
        InitMoves();
    }

    public void ChangeVisibility()
    {
        if (IsNearPlayerUnit())
            ShowUnit();
        else
            HideUnit();
    }

    protected void DoFight()
    {
        //TODO
        /*if (!CanMove())
            return;
        var unitCell = GetUnitCell();
        foreach (var cell in Utils.Neighbors(unitCell))
            if (GameManager.Instance.HasPlayerUnit(cell))
            {
                ControllerManager.Instance.StartBattle(this,
                    GameManager.Instance.PlayerUnits[cell]);
                ChangeMoves();
            }*/
    }

    private bool CanMove()
    {
        return _moves > 0;
    }

    protected void UnitDie()
    {
        IsDead = true;
    }

    private bool IsNearPlayerUnit()
    {
        //TODO
        //return Utils.Neighbors(GetUnitCell()).Any(i => GameManager.Instance.HasPlayerUnit(i));
        return true;
    }
}