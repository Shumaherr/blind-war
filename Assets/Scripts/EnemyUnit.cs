using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnit : Unit
{
    private List<Vector3Int> _currentPath;

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
        int lastStep = 0;
        for (int i = 1; i < _currentPath.Count; i++)
        {
            if(Moves == 0)
                break;
            ControllerManager.Instance.MoveUnitToTile(transform, _currentPath[i]);
            lastStep = i;
            ChangeMoves();
        }
        GameManager.Instance.ChangeEnemyCell(_currentPath[0], _currentPath[lastStep]);
    }

    public void DoTurn()
    {
        InitMoves();
        List<List<Vector3Int>> pathes = new List<List<Vector3Int>>();
        foreach (var unit in GameManager.Instance.PlayerUnits.Where(pair =>
            pair.Value.BaseUnit.UnitType == BaseUnit.KillUnit))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, unit.Key));
        }

        foreach (var city in GameManager.Instance.AllCities.Where(pair => pair.Value.Owner == CityOwner.Player))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, city.Key));
        }
        if(pathes.Count == 0) //TODO what to do if no targets was found
            return;
        int min = 0;
        for (int i = 1; i < pathes.Count; i++)
        {
            if (pathes[i].Count < pathes[min].Count)
            {
                min = i;
            }
        }
        
        //pathes[min].RemoveAt(0);
        pathes[min].RemoveAt(pathes[min].Count - 1);
        _currentPath = new List<Vector3Int>(pathes[min]);
        DoMove();
        DoFight();
        ChangeMoves();
    }

    private void DoFight()
    {
        Vector3Int unitCell = GetUnitCell();
        foreach (var cell in Utils.Neighbors(unitCell))
        {
            if (!CanMove())
                return;
            if (GameManager.Instance.HasPlayerUnit(cell) &&
                GameManager.Instance.PlayerUnits[cell].BaseUnit.UnitType == BaseUnit.KillUnit)
            {
                ControllerManager.Instance.StartBattle(this,
                    GameManager.Instance.PlayerUnits[cell]);
                ChangeMoves();
            }

            if (GameManager.Instance.HasPlayerCity(cell))
            {
                GameManager.Instance.GetCityInCell(cell).TakeDamage(BaseUnit.Damage);
                ChangeMoves();
            }
        }
    }

    public override Vector3Int GetUnitCell()
    {
        return GameManager.Instance.Grid.WorldToCell(transform.position);
    }

    public bool CanMove()
    {
        return _moves > 0;
    }
}