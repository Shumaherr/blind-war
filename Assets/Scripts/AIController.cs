using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Debug = UnityEngine.Debug;

public class AIController : BaseController, IController
{
    private ControllerType controllerType;
    private List<Unit> visibleUnits = new List<Unit>();
    private Dictionary<Vector3Int, List<Vector3Int>> cachedPaths = new Dictionary<Vector3Int, List<Vector3Int>>();
    
    private static Pathfinding pathfinding = new Pathfinding();

    ControllerType IController.ControllerType => controllerType;

    public void DoMove()
    {
        var randomCell = GameManager.Instance.GetFreeRandomNeighbourCell(Unit.GetUnitCell());
        MoveTo(randomCell);
    }

    private void CheckVisibility()
    {
        //visibleUnits.RemoveAll(unit => !IsVisible(unit));
        visibleUnits.Clear();
        var neighbors = Utils.Neighbors(Unit.GetUnitCell(), 2);
        foreach (var cell in neighbors)
        {
            if (ControllerManager.Instance.GetUnitAtPosition(cell) is { } unit)
            {
                visibleUnits.Add(unit);
            }
        }
    }

    private bool IsVisible(Unit unit)
    {
        // Implement this method
        throw new System.NotImplementedException();
    }


    private bool AttackVisibleUnit()
    {
        foreach (var unit in visibleUnits)
        {
            if (CanAttack(unit))
            {
                Attack(unit);
                return true;
            }
        }

        return false;
    }

    private bool CanAttack(Unit unit)
    {
        return unit != null;
    }

    private void Attack(Unit unit)
    {
        if (unit == null)
        {
            return;
        }
        ControllerManager.Instance.StartBattle(Unit, unit);
        Unit.ChangeMoves();
    }


    private Unit GetClosestUnit()
    {
        Unit closestUnit = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Unit unit in visibleUnits)
        {
            float distance = Vector3.Distance(currentPosition, unit.transform.position);
            if (distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }

    private void MoveTo(Vector3Int cellPos)
    {
        Debug.Log("Moving to " + cellPos);
        ControllerManager.Instance.MoveUnitToTile(transform, cellPos, true);
        Unit.ChangeMoves(MapManager.Instance.GetMoveCosts(Unit.BaseUnit,
            GameManager.Instance.Grid.GetTile(cellPos)));
    }

    private void EndTurn()
    {
        GameManager.Instance.TurnManager.ChangeTurn();
    }

    public void DoAttack()
    {
        // Implement this method
        throw new System.NotImplementedException();
    }

    public void DoTurn()
    {
        while (Unit.Moves > 0)
        {
            CheckVisibility();
            if (visibleUnits.Count > 0)
            {
                AttackVisibleUnit();
                continue;
            }
            DoMove();
        }
        EndTurn();
    }
}
