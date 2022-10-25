using System;
using System.Collections.Generic;
using UnityEngine;


public class TurnManager
{

    private int _playerIndex;

    public TurnManager()
    {
        _playerIndex = 0;
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

    public Player Turn => GameManager.Instance.Players[_playerIndex];

    public void ChangeTurn()
    {
        PlayerIndex = PlayerIndex == GameManager.Instance.Players.Count - 1 ? 0 : PlayerIndex++; //TODO Check that player active is
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