using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnit : Unit
{
    private Queue<Vector3Int> _currentPath;
    
    private void Start()
    {

    }

    public override void InitMoves()
    {
    }

    public void DoMove()
    {
        
        //TODO Ограничить количество ходов
        while (_currentPath.Count > 0)
        {
            ControllerManager.Instance.MoveUnitToTile(this.transform, _currentPath.Dequeue());
        }
    }

    public void DoTurn()
    {
        List<List<Vector3Int>> pathes = new List<List<Vector3Int>>();
        Pathfinding pathfinding = new Pathfinding();
        foreach (var unit in GameManager.Instance.PlayerUnits.Where(pair => pair.Value.BaseUnit.UnitType == BaseUnit.KillUnit))
        {
            pathes.Add(pathfinding.FindPath(GameManager.Instance.Grid,GameManager.Instance.Grid.WorldToCell(transform.position), unit.Key));
        }

        foreach (var city in GameManager.Instance.AllCities.Where(pair => pair.Value.Owner == CityOwner.Player))
        {
            pathes.Add(pathfinding.FindPath(GameManager.Instance.Grid,GameManager.Instance.Grid.WorldToCell(transform.position), city.Key));
        }

        int min = 0;
        for (int i = 1; i < pathes.Count; i++)
        {
            if (pathes[i].Count < pathes[min].Count)
            {
                min = i;
            }
        }
        
        pathes[min].RemoveAt(0);
        pathes[min].RemoveAt(pathes[min].Count - 1);
        _currentPath = new Queue<Vector3Int>(pathes[min]);
        DoMove();
    }
}