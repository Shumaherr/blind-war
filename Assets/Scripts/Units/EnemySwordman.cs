using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordman : EnemyUnitBase
{
    public override void DoMove()
    {
        base.DoMove();
        
    }

    public override void DoTurn()
    {
        base.DoTurn();
        DoFight();
        ChangeMoves();
    }
}
