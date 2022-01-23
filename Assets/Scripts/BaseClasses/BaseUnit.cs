using UnityEngine;

public enum UnitType
{
    Stabbing,
    Cutting,
    Crushing,
    Distant,
    Magic
}

public abstract class BaseUnit : ScriptableObject
{
    [SerializeField] protected Sprite baseSprite;
    [SerializeField] protected byte damage;
    [SerializeField] private Sprite generalizedSprite;
    [SerializeField] protected UnitType killUnit;
    [SerializeField] protected byte moves;
    [SerializeField] protected UnitType unitType;
    [SerializeField] protected byte maxHealth;
    
    public UnitType UnitType => unitType;

    public UnitType KillUnit => killUnit;

    public byte Moves => moves;

    public Sprite GeneralizedSprite => generalizedSprite;

    public byte Damage => damage;

    public Sprite BaseSprite => baseSprite;

    public byte MaxHealth => maxHealth;
    public abstract void Use();
}