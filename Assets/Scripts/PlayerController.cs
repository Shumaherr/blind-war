using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayerController : BaseController, IController
{
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

    public void DoTurn()
    {
        throw new NotImplementedException();
    }

    private void DoInteract()
    {
        if (!GameManager.Instance.TurnManager.IsLocalPlayerTurn() || ControllerManager.Instance.SelectedUnit == Unit || GameManager.Instance.TurnManager.Turn != Unit.Owner)
            return;
        RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
        //ControllerManager.Instance.SelectedUnit?.DeactivateDialog();
        EventManager.TriggerEvent("unitSelected", new Dictionary<string, object> { { "unit", Unit } });
        //Perks.ForEach(perk => perk.Use());
    }
}