using UnityEngine;

public class BattleSystem
{
    public static Unit Fight(Unit firstFighter, Unit secondFighter)
    {
        if (firstFighter.BaseUnit.KillUnit == secondFighter.BaseUnit.UnitType)
        {
            Debug.Log("BattleSystem: " + secondFighter.BaseUnit.UnitType + " killed by" + firstFighter.BaseUnit.UnitType);
            return firstFighter;
        }
        if (secondFighter.BaseUnit.KillUnit == firstFighter.BaseUnit.UnitType)
        {
            Debug.Log("BattleSystem: " + firstFighter.BaseUnit.UnitType + " killed by" + secondFighter.BaseUnit.UnitType);
            return secondFighter;
        }
        firstFighter.TakeDamage(secondFighter.Damage);
        return null;
    }
}