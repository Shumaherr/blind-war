using System;
using UnityEngine;

public enum TurnStates
{
    PlayerTurn,
    AITurn
}

public class TurnManager : Singleton<TurnManager>
{
    private TurnStates _turn;

    public TurnStates Turn => _turn;

    [SerializeField] public TurnStates firstTurn = TurnStates.PlayerTurn;

    public delegate void OnTurnChangedDelegate(TurnStates newTurn);

    public event OnTurnChangedDelegate OnTurnChanged;

    private void Start()
    {
        _turn = firstTurn;
    }

    public void ChangeTurn()
    {
        ControllerManager.Instance.ClearSelected();
        _turn = _turn is TurnStates.PlayerTurn ? TurnStates.AITurn : TurnStates.PlayerTurn;
        if (OnTurnChanged != null)
            OnTurnChanged(_turn);
    }

    public bool isPlayerTurn()
    {
        return _turn == TurnStates.PlayerTurn;
    }
}