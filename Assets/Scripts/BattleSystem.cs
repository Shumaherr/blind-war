public class BattleSystem
{
    public static Unit Fight(Unit firstFighter, Unit secondFighter)
    {
        if (firstFighter.BaseUnit.KillUnit == secondFighter.BaseUnit.UnitType)
            return firstFighter;
        if (secondFighter.BaseUnit.KillUnit == firstFighter.BaseUnit.UnitType)
            return secondFighter;
        else
        {
            firstFighter.TakeDamage(secondFighter.Damage);
            return null;
        }
    }
}