using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ControllerManager : Singleton<ControllerManager>
{
    [SerializeField] private Grid grid;
    private UnitInteractable _selectedUnit;
    private GridInteractor _gridInteractor;

    public UnitInteractable SelectedUnit
    {
        get => _selectedUnit;
        set => _selectedUnit = value;
    }

    private void Start()
    {
        _gridInteractor = grid.GetComponentInChildren<GridInteractor>();
        _gridInteractor.OnTileSelected += TileSelected;
        //Subscribe to event click on tile
    }

    private void TileSelected(Vector3Int tilePos)
    {
        Debug.Log("Tile" + tilePos + " clicked");
        if (!TurnManager.Instance.isPlayerTurn())
            return;
        if (_selectedUnit)
        {
            MoveUnitToTile(tilePos);
        }
    }

    private void MoveUnitToTile(Vector3Int tilePos)
    {
        if(!_selectedUnit.CanMove())
            return;
        Transform var1 = _selectedUnit.transform;
        Vector3 var3 = grid.GetCellCenterWorld(tilePos);
        Vector3Int unitCell = grid.WorldToCell(_selectedUnit.transform.position);
        Debug.Log("Move for " + Mathf.Round(Vector3.Distance(tilePos,unitCell)) + " cells");
        StartCoroutine(MoveFromTo(var1, var1.position, var3, 3));
        //StartCoroutine(waitForMove());
        Debug.Log("End");
        _selectedUnit.ChangeMoves(1);
        ClearSelected();
    }

    private void ClearSelected()
    {
        _selectedUnit = null;
    }

    private IEnumerator MoveFromTo(Transform objectToMove, Vector3 pos1, Vector3 pos2, float speed)
    {
        float step = (speed / (pos1 - pos2).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(pos1, pos2, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }

        objectToMove.position = pos2;
    }

    public Vector3Int SelectedUnitCell()
    {
        return grid.LocalToCell(_selectedUnit.transform.position);
    }
}