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
    [SerializeField] private Sprite generalizedSprite;
    [SerializeField] protected byte moves;

    public UnitType UnitType => unitType;

    public UnitType KillUnit => killUnit;

    public byte Moves => moves;

    public Sprite GeneralizedSprite => generalizedSprite;

    public byte Damage => damage;

    public Sprite BaseSprite => baseSprite;

    [SerializeField] protected byte damage;

    [SerializeField] protected Sprite baseSprite;

    public abstract void Use();

}
