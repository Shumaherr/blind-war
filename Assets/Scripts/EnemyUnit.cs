using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyUnit : Unit
{
    
    private void Start()
    {
       
    }

    public override void InitMoves()
    {
    }

    public void DoMove()
    {
        
    }

    public void DoTurn()
    {
        List<List<Vector3Int>> pathes = new List<List<Vector3Int>>();
        foreach (var unit in GameManager.Instance.PlayerUnits.Where(pair => pair.Value.BaseUnit.UnitType == BaseUnit.KillUnit))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, unit.Key));
        }

        foreach (var city in GameManager.Instance.AllCities.Where(pair => pair.Value.Owner == CityOwner.Player))
        {
            pathes.Add(GameManager.Instance.GetPath(transform.position, city.Key));
        }
    }
}