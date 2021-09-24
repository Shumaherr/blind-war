using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected int _moves;
    [SerializeField] protected BaseUnit baseUnit;

    public int Moves => _moves;

    public BaseUnit BaseUnit => baseUnit;

    public abstract void InitMoves();

    public abstract Vector3Int GetUnitCell();
}