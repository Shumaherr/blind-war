using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class TurnManager
{

    private int _playerIndex;

    public TurnManager()
    {
        PlayerIndex = -1;
    }

    private int PlayerIndex
    {
        get => _playerIndex;
        set
        {
            _playerIndex = value;
            if (value == 0)
            {
                TurnsCounter++;
            }
        }
    }

    private int _turnsCounter;

    public int TurnsCounter
    {
        get => _turnsCounter;
        set
        {
            _turnsCounter = value;
            EventManager.TriggerEvent("newTurn", new Dictionary<string, object>());
        }
    }

    public Player Turn => GameManager.Instance.Players[PlayerIndex];

    public void ChangeTurn()
    {
        int nextIndex = PlayerIndex;
        do
        {
            nextIndex = nextIndex == GameManager.Instance.Players.Count - 1 ? 0 : ++nextIndex;
        } while (!GameManager.Instance.Players[nextIndex].Active);

        PlayerIndex = nextIndex;
        EventManager.TriggerEvent("turnChanged", new Dictionary<string, object> { { "whoseTurn", Turn } });
        Debug.Log("Turn:" + Turn.Name);
    }

    
    public bool IsPlayerTurn(Player player)
    {
        return Turn == player;
    }

    public bool IsLocalPlayerTurn()
    {
        return Turn.Type == PlayerType.LocalPlayer;
    }
}