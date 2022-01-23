using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected int _moves;

    protected int _health;

    protected int _level;
    
    
    [SerializeField] protected BaseUnit baseUnit;

    public int Moves => _moves;

    public BaseUnit BaseUnit => baseUnit;

    public abstract void InitMoves();
    
    public void InitHealth()
    {
        _health = baseUnit.MaxHealth;
    }
    public abstract Vector3Int GetUnitCell();
}