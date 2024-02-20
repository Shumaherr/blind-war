using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIController : BaseController, IController
{
    private ControllerType controllerType;
    private List<Unit> visibleUnits = new List<Unit>();

    ControllerType IController.ControllerType => controllerType;

    public void DoMove()
    {
        CheckVisibility();
        if (AttackVisibleUnit())
        {
            EndTurn();
            return;
        }

        if (visibleUnits.Count == 0)
        {
            HangAround();
        }
        else
        {
            MoveToUnit();
        }

        EndTurn();
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
        if(unit == null)
            return false;
        return true;
    }

    private void Attack(Unit unit)
    {
        if (unit == null)
        {
            return;
        }
        ControllerManager.Instance.StartBattle(Unit, unit);
    }

    private void MoveToUnit()
    {
        if (visibleUnits.Count > 0)
        {
            var unit = GetClosestUnit();
            MoveTo(unit.GetUnitCell());
        }
    }

    private void HangAround()
    {
        while (Unit.Moves > 0)
        {
            Vector3Int randomCell = GetRandomWalkableCell();
            MoveTo(randomCell);
        }
    }

    private Vector3Int GetRandomWalkableCell()
    {
        List<Vector3Int> walkableCells = new List<Vector3Int>();
        Tilemap tilemap = GameManager.Instance.Grid;
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cellPos) && !GameManager.Instance.IsCellTaken(cellPos))
                {
                    walkableCells.Add(cellPos);
                }
            }
        }

        if (walkableCells.Count > 0)
        {
            int randomIndex = Random.Range(0, walkableCells.Count);
            return walkableCells[randomIndex];
        }
        else
        {
            return Unit.GetUnitCell(); // Return current cell if no walkable cell is found
        }
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

    private void MoveTo(Vector3Int celPos)
    {
        Pathfinding pathfinding = new Pathfinding();
        List<Vector3Int> path = pathfinding.FindPath(GameManager.Instance.Grid, Unit.GetUnitCell(), celPos);
        Debug.Log("Moving to " + celPos + " with path " + path);
        if (path is { Count: > 1 })
        {
            for (var i = 1; i < path.Count; i++)
            {
                if (Unit.Moves == 0)
                    break;
                if (CanAttack(ControllerManager.Instance.GetUnitAtPosition(path[i])))
                {
                    Attack(ControllerManager.Instance.GetUnitAtPosition(path[i]));
                    return;
                }
                ControllerManager.Instance.MoveUnitToTile(transform, path[i], true);
                Unit.ChangeMoves(MapManager.Instance.GetMoveCosts(Unit.BaseUnit,
                    GameManager.Instance.Grid.GetTile(path[i])));
            }
        }
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
        DoMove();
    }
}