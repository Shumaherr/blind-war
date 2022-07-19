using System;
using System.Linq;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UnitInteractable : Unit
{
    public event Action<UnitInteractable> OnUnitSelected;
    
    protected Transform _dialogBox;
    protected Healthbar _healthbar;
    protected TurnBar _turnBar;
    protected string _dialogText;

    protected TextMeshPro _textMeshPro;

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
    
    public override int Moves
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

    public override void TakeDamage(int amount)
    {
        Health = _health > amount ? Health -= amount : 0;
        Debug.Log("Taken "+ amount + " damage. Health: " + Health);
    }

    protected virtual void Awake()
    {
        //enabled = true;
        
        _healthbar = GetComponentInChildren<Healthbar>();
        _turnBar = GetComponentInChildren<TurnBar>();
        //DeactivateDialog();
        _textMeshPro = GetComponentInChildren<TextMeshPro>();
        TurnManager.Instance.OnTurnChanged += ChangeTurn;
        InitUnit();
        _healthbar.SetHealthLevel(Health/BaseUnit.MaxHealth);
    }

    protected void ChangeTurn(TurnStates newturn)
    {
        switch (newturn)
        {
            case TurnStates.PlayerTurn:
                InitMoves();
                break;
            case TurnStates.AITurn:
                if(GetComponent<Fortificate>())
                    Debug.Log(name + " is fortified");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newturn), newturn, null);
        }
    }

    public void ChangeMoves(int moves = 1)
    {
        Moves = Math.Max(0, _moves - moves);
    }

    public bool CanMove()
    {
        return _moves > 0;
    }

    protected override void InitMoves()
    {
        Moves = baseUnit.Moves;
    }

    

    protected void OnMouseDown()
    {
        Debug.Log(OnUnitSelected?.GetInvocationList());
        if (!TurnManager.Instance.isPlayerTurn())
            return;
        RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
        //ControllerManager.Instance.SelectedUnit?.DeactivateDialog();
        ControllerManager.Instance.SelectedUnit = this;
        OnUnitSelected?.Invoke(this);
        Perks.ForEach(perk => perk.Use());
        
    }

    protected override void UnitDie()
    {
        IsDead = true;
    }
    
}