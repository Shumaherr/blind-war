using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    private Unit _unit;

    private void Start()
    {
        _unit = GetComponent<Unit>();
    }

    private void OnMouseDown()
    {
        DoInteract();
    }

    ControllerType IController.ControllerType { get; } = ControllerType.Player;


    public void DoMove()
    {
    }

    public void DoAttack()
    {
        throw new NotImplementedException();
    }

    private void DoInteract()
    {
        if (!GameManager.Instance.TurnManager.IsLocalPlayerTurn() || ControllerManager.Instance.SelectedUnit == _unit || GameManager.Instance.TurnManager.Turn != _unit.Owner)
            return;
        RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
        //ControllerManager.Instance.SelectedUnit?.DeactivateDialog();
        EventManager.TriggerEvent("unitSelected", new Dictionary<string, object> { { "unit", _unit } });
        //Perks.ForEach(perk => perk.Use());
    }
}