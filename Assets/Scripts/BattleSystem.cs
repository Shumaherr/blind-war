public class BattleSystem
{
  
    public bool Fight(BaseUnit firstFighter, BaseUnit secondFighter)
    {
        return firstFighter.KillUnit == secondFighter.UnitType;
    }

    
}
