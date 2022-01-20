using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utils;

public class GridInteractor : BaseInteractable
{
    public delegate void OnTileSelectedDelegate(Vector3Int tilePos);

    private Tilemap _grid;

    private List<Vector3Int> _highlightedTiles;
    [SerializeField] private Camera mainCamera;


    public GridInteractor(List<Vector3Int> highlightedTiles)
    {
        _highlightedTiles = highlightedTiles;
    }

    public event OnTileSelectedDelegate OnTileSelected;

    // Start is called before the first frame update
    private void Start()
    {
        _highlightedTiles = new List<Vector3Int>();
        _grid = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Input.mousePosition;
            mousePos.z = 4;
            Vector2 clickWorldPos = mainCamera.ScreenToWorldPoint(mousePos);
            var clickCellPos = _grid.WorldToCell(clickWorldPos);
            if (!_grid.HasTile(clickCellPos) ||
                !IsNeighbor(ControllerManager.Instance.SelectedUnitCell(), clickCellPos) ||
                GameManager.Instance.TakenCells.Contains(clickCellPos))
                return;
            OnTileSelected?.Invoke(clickCellPos);
        }
    }

    public override void Interact()
    {
    }

    private static bool IsNeighbor(Vector3Int cell1, Vector3Int cell2)
    {
        return Neighbors(cell1).Contains(cell2);
    }

    public void HighlightNeighbourCells(BaseUnit selectedUnit)
    {
        if (_highlightedTiles.Count != 0)
            UnhighlightCells();
        foreach (var cell in Neighbors(ControllerManager.Instance.SelectedUnitCell()))
        {
            if (GameManager.Instance.TakenCells.Contains(cell) || _grid.GetTile(cell) == null || MapManager.Instance.GetTurnPoints(selectedUnit, _grid.GetTile(cell)) <= 0)
                continue;
            _grid.SetTileFlags(cell, TileFlags.None);
            _grid.SetColor(cell, Color.gray);
            _highlightedTiles.Add(cell);
        }
    }

    public void HighlightCell(Vector3Int cell, Color highlightColor)
    {
        _grid.SetTileFlags(cell, TileFlags.None);
        _grid.SetColor(cell, highlightColor);
        _highlightedTiles.Add(cell);
    }

    public void UnhighlightCells()
    {
        foreach (var tile in _highlightedTiles) _grid.SetColor(tile, Color.white);
    }
}