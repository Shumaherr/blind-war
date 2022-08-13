
using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayerController: MonoBehaviour, IController
    {
        private ControllerType _controllerType = ControllerType.Player;
        private Unit _unit;
        ControllerType IController.ControllerType => _controllerType;
        

        public void DoMove()
        {
            
        }

        public void DoAttack()
        {
            throw new System.NotImplementedException();
        }
        
        public void DoInteract()
        {
            if (!TurnManager.Instance.isPlayerTurn())
                return;
            RuntimeManager.PlayOneShot("event:/SFX/ui/select", transform.position);
            //ControllerManager.Instance.SelectedUnit?.DeactivateDialog();
            ControllerManager.Instance.SelectedUnit = _unit;
            EventManager.TriggerEvent("unitSelected", new Dictionary<string, object>{{"unit", _unit}});
            //Perks.ForEach(perk => perk.Use());
            
        }

        private void OnMouseDown()
        {
            DoInteract();
        }

        private void Start()
        {
            _unit = GetComponent<Unit>();
        }
    }
