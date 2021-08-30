using UnityEngine;

public class BattleSystem : MonoBehaviour
{
  
    public bool Fight(BaseUnit firstFighter, BaseUnit secondFighter)
    {
        return firstFighter.KillUnit == secondFighter.UnitType ? firstFighter : secondFighter;
    }

    
}
