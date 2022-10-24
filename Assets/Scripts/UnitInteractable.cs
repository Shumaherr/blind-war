using System;
using TMPro;
using UnityEngine;

public class UnitInteractable : Unit
{
    protected Transform _dialogBox;
    protected string _dialogText;
    protected Healthbar _healthbar;

    protected TextMeshPro _textMeshPro;
    protected TurnBar _turnBar;

    protected override int Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthbar.SetHealthLevel((float)_health / baseUnit.MaxHealth);
            if (value <= 0)
                UnitDie();
        }
    }

    public int Moves
    {
        get => _moves;
        set
        {
            _moves = value;
            _turnBar.SetTurnText(_moves, BaseUnit.Moves);
        }
    }

    public string DialogText
    {
        get => _dialogText;
        set
        {
            _dialogText = value;
            _textMeshPro.text = _dialogText;
        }
    }


    protected virtual void Awake()
    {
        //enabled = true;

        _healthbar = GetComponentInChildren<Healthbar>();
        _turnBar = GetComponentInChildren<TurnBar>();
        //DeactivateDialog();
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        //TurnManager.Instance.OnTurnChanged += ChangeTurn;
        InitUnit();
        _healthbar.SetHealthLevel(Health / BaseUnit.MaxHealth);
    }

    
}