using System;
using UnityEngine.Networking;

public enum ControllerType
{
    Player,
    AI
}
public interface IController
{
    protected ControllerType ControllerType { get; }
    public abstract void DoMove();
    public abstract void DoAttack();

}