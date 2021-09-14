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
        _moves = baseUnit.Moves;
    }
    
    public void ChangeMoves(int moves = 1)
    {
        _moves = Math.Max(0, _moves - moves);
    }

    public void DoMove()
    {
        
        //TODO Ограничить количество ходов
        while (_currentPath.Count > 0 && Moves > 0)
        {
            ControllerManager.Instance.MoveUnitToTile(transform, _currentPath.Dequeue());
            ChangeMoves();
            
        }
    }

    public void DoTurn()
    {
        InitMoves();
        List<List<Vector3Int>> pathes = new List<List<Vector3Int>>();
        foreach (var unit in GameManager.Instance.PlayerUnits.Where(pair => pair.Value.BaseUnit.UnitType == BaseUnit.KillUnit))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, unit.Key));
        }

        foreach (var city in GameManager.Instance.AllCities.Where(pair => pair.Value.Owner == CityOwner.Player))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, city.Key));
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
    
    public override Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }
}