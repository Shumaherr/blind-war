using UnityEngine;

public enum TurnStates
{
    PlayerTurn,
    AITurn
}

public class TurnManager : Singleton<TurnManager>
{
    public delegate void OnTurnChangedDelegate(TurnStates newTurn);

    [SerializeField] public TurnStates firstTurn = TurnStates.PlayerTurn;

    public TurnStates Turn { get; private set; }

    public event OnTurnChangedDelegate OnTurnChanged;

    private void Start()
    {
        Turn = firstTurn;
    }

    public void ChangeTurn()
    {
        ControllerManager.Instance.ClearSelected();
        Turn = Turn is TurnStates.PlayerTurn ? TurnStates.AITurn : TurnStates.PlayerTurn;
        OnTurnChanged?.Invoke(Turn);
    }

    public bool isPlayerTurn()
    {
        return Turn == TurnStates.PlayerTurn;
    }
}