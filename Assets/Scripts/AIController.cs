using UnityEngine;

public class AIController: MonoBehaviour, IController
{
    private ControllerType controllerType;

    ControllerType IController.ControllerType => controllerType;

    public void DoMove()
    {
        Debug.Log($"AI Unit moves");
    }

    public void DoAttack()
    {
        
    }
}