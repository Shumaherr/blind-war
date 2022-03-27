using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHorseman : EnemyUnitBase
{
    public override void DoMove()
    {
        var lastStep = 0;
        for (var i = 1; i < _currentPath.Count; i++)
        {
            if (Moves == 0)
                break;
            ControllerManager.Instance.MoveUnitToTile(transform, _currentPath[i]);
            lastStep = i;
            ChangeMoves();
        }

        GameManager.Instance.ChangeEnemyCell(_currentPath[0], _currentPath[lastStep]);
    }
    
    public override void DoTurn()
    {
        InitMoves();
        var pathes = new List<List<Vector3Int>>();
        foreach (var unit in GameManager.Instance.PlayerUnits)
            pathes.Add(GameManager.Instance.GetPath(transform.position, unit.Key));
        
        if (pathes.Count == 0) //TODO what to do if no targets was found
            return;
        var min = 0;
        for (var i = 1; i < pathes.Count; i++)
            if (pathes[i].Count < pathes[min].Count)
                min = i;

        //pathes[min].RemoveAt(0);
        if (pathes[min] != null)
        {
            pathes[min].RemoveAt(pathes[min].Count - 1);
            _currentPath = new List<Vector3Int>(pathes[min]);
            DoMove();
        }

        DoFight();
        ChangeMoves();
    }
}
