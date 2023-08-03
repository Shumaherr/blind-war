using System;
using System.Collections.Generic;
using UnityEngine;

public class Fortificate : Perk {
    private int _currentAttacksAmount;
    private int _currentDuration;

    public bool IsActive
    {
        get => _isActive;
        private set
        {
            if (!value) {
                DeaktivatePerk();
            }

            _isActive = value;
        }
    }

    private int _maxAttacksAmount = 1;
    private int _maxDuration = 2;
    private bool _isActive;

    public Fortificate() {
        SubscribeToEvents();
    }

    protected sealed override void SubscribeToEvents() {
        EventManager.StartListening("newTurn", DecreaseDuration);
    }

    private void DecreaseDuration(Dictionary<string, object> obj) {
        if (!IsActive)
            return;
        _currentDuration--;
        if (_currentDuration <= 0) {
            IsActive = false;
            _currentDuration = 0;
        }
    }

    public void Activate() {
        if (!IsActive) {
            IsActive = true;
            _currentDuration = _maxDuration;
            Debug.Log("Fortificate activated");
        }
    }

    private void DeaktivatePerk() {
        Debug.Log("Fortificate deactivated");
    }

    public void SetUp(int maxDuration) {
        _maxDuration = maxDuration;
    }

    public void DecreaseAttacksAmount() {
        _currentAttacksAmount--;
        if (_currentAttacksAmount <= 0) {
            _currentAttacksAmount = 0;
            IsActive = false;
            _currentDuration = 0;
            Debug.Log("Fortificate deactivated");
        }
    }

    public override void Use() {
        Activate();
    }
}