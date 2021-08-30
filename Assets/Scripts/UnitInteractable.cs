using System;
using UnityEngine;

public class UnitInteractable : BaseInteractable
{
    [SerializeField] private BaseUnit unit;

    private int _moves;

    public override void Interact()
    {
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += ChangeTurn;
        Debug.Log("Unit: " + unit.UnitType);
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

    private void InitMoves()
    {
        _moves = unit.Moves;
    }

    private void OnMouseDown()
    {
        if (!TurnManager.Instance.isPlayerTurn())
            return;
        ControllerManager.Instance.SelectedUnit = this;
        Debug.Log("Unit clicked" + unit.UnitType);
    }
}