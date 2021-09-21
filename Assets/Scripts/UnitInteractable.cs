using System;
using UnityEngine;

public class UnitInteractable : Unit
{
    public delegate void OnUnitSelectedDelegate(UnitInteractable unit);
    public event OnUnitSelectedDelegate OnUnitSelected;
    
    
    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += ChangeTurn;
        Debug.Log("Unit: " + baseUnit.UnitType);
        InitMoves();
    }

    private void ChangeTurn(TurnStates newturn)
    {
        switch (newturn)
        {
            case TurnStates.PlayerTurn:
                InitMoves();
                break;
            case TurnStates.AITurn:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newturn), newturn, null);
        }
    }

    public void ChangeMoves(int moves = 1)
    {
        _moves = Math.Max(0, _moves - moves);
    }

    public bool CanMove()
    {
        return _moves > 0;
    }

    public override void InitMoves()
    {
        _moves = baseUnit.Moves;
    }

    public override Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }

    private void OnMouseDown()
    {
        if (!TurnManager.Instance.isPlayerTurn())
            return;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
        ControllerManager.Instance.SelectedUnit = this;
        if (OnUnitSelected != null) 
            OnUnitSelected.Invoke(this);
        Debug.Log("Unit clicked" + baseUnit.UnitType);
    }
    
    public void UsePerk()
    {
        switch (BaseUnit.UnitType)
        {
            case UnitType.Swordman:
                break;
            case UnitType.Spearman:
                GameManager.Instance.HighlightCellWithoutEnemy();
                break;
            case UnitType.Horseman:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}