public class BattleSystem
{
  
    public BaseUnit Fight(BaseUnit firstFighter, BaseUnit secondFighter)
    {
        if (firstFighter.UnitType == secondFighter.UnitType)
            return null;
        return firstFighter.KillUnit == secondFighter.UnitType ? firstFighter : secondFighter;
    }

    
}
