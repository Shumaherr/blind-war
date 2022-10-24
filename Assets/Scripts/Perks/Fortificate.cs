using System;
using System.Collections.Generic;

public class Fortificate : Perk
{
    private int _currentAttacksAmount;
    private int _currentDuration;
    private bool _isActive;
    private int _maxAttacksAmount = 1;
    private int _maxDuration = 1;


    private void OnEnable()
    {
        EventManager.StartListening("newTurn", DecreaseDuration);
    }

    private void DecreaseDuration(Dictionary<string, object> obj)
    {
        if (!_isActive)
            return;
        _currentDuration--;
        if (_currentDuration <= 0)
        {
            _isActive = false;
            _currentDuration = 0;
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