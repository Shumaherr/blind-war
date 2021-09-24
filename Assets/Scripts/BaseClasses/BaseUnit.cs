using UnityEngine;

public enum UnitType
{
    Swordman,
    Spearman,
    Horseman
}

public abstract class BaseUnit : ScriptableObject
{
    [SerializeField] protected Sprite baseSprite;

    [SerializeField] protected byte damage;
    [SerializeField] private Sprite generalizedSprite;
    [SerializeField] protected UnitType killUnit;
    [SerializeField] protected byte moves;
    [SerializeField] protected UnitType unitType;

    public UnitType UnitType => unitType;

    public UnitType KillUnit => killUnit;

    public byte Moves => moves;

    public Sprite GeneralizedSprite => generalizedSprite;

    public byte Damage => damage;

    public Sprite BaseSprite => baseSprite;

    public abstract void Use();
}