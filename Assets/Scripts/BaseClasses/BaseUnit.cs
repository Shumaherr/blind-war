using UnityEngine;

public enum UnitType
{
    Swordman,
    Spearman,
    Horseman
};

public abstract class BaseUnit : ScriptableObject
{
    [SerializeField] protected UnitType unitType;
    [SerializeField] protected UnitType killUnit;
    [SerializeField] protected byte moves;

    public UnitType UnitType => unitType;

    public UnitType KillUnit => killUnit;

    public byte Moves => moves;

    public byte Damage => damage;

    [SerializeField] protected byte damage;

    [SerializeField] protected GameObject unitPrefab;

    public abstract void Use();

}
