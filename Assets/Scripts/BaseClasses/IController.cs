public enum ControllerType
{
    Player,
    AI
}

public interface IController
{
    protected ControllerType ControllerType { get; }
    public void DoMove();
    public void DoAttack();
}