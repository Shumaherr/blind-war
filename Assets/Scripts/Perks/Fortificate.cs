using System;
using UnityEngine;


public class Fortificate : Perk
{
    private bool _isActive = false;
    private int _maxDuration = 1;
    private int _currentDuration;
    private int _maxAttacksAmount = 1;
    private int _currentAttacksAmount;

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += DecreaseDuration;
    }

    private void DecreaseDuration(TurnStates newturn)
    {
        if (!_isActive)
            return;
        if (newturn == TurnStates.PlayerTurn)
        {
            _currentDuration--;
            if (_currentDuration <= 0)
            {
                _isActive = false;
                _currentDuration = 0;
            }
        }
    }

    public void Activate()
    {
        if (!_isActive)
        {
            _isActive = true;
            _currentDuration = _maxDuration;
        }
    }

    public void SetUp(int maxDuration)
    {
        _maxDuration = maxDuration;
    }

    public bool IsActive()
    {
        return _isActive;
    }
    
    public void DecreaseAttacksAmount()
    {
        _currentAttacksAmount--;
        if (_currentAttacksAmount <= 0)
        {
            _currentAttacksAmount = 0;
            _isActive = false;
            _currentDuration = 0;
        }
    }


    public override void Use()
    {
        Activate();
    }
}