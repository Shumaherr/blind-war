using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utils;

public class GridInteractor : BaseInteractable
{
    public delegate void OnTileSelectedDelegate(Vector3Int tilePos);

    [SerializeField] private Camera mainCamera;

    private Tilemap _grid;

    private List<Vector3Int> _highlightedTiles;


    public GridInteractor(List<Vector3Int> highlightedTiles)
    {
        _highlightedTiles = highlightedTiles;
    }

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
            Debug.Log($"Clicked {clickCellPos}");
            if (!_grid.HasTile(clickCellPos) ||
                !ControllerManager.Instance.SelectedUnit||
                !IsNeighbor(UnitCell(ControllerManager.Instance.SelectedUnit), clickCellPos) ||
                ControllerManager.Instance.AllUnits.ContainsKey(clickCellPos))
                return;
            OnTileSelected?.Invoke(clickCellPos);
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("unitSelected", OnUnitSelected);
        EventManager.StartListening("unitMoved", OnUnitMoved);
    }

    private void OnDisable()
    {
        EventManager.StopListening("unitSelected", OnUnitSelected);
        EventManager.StopListening("unitMoved", OnUnitMoved);
    }

    public event OnTileSelectedDelegate OnTileSelected;

    private void OnUnitMoved(Dictionary<string, object> obj)
    {
        UnhighlightCells();
    }

    private void OnUnitSelected(Dictionary<string, object> obj)
    {
        var unit = (Unit)obj["unit"];
        if (unit.CanMove())
            HighlightNeighbourCells(unit);
    }

    public override void Interact()
    {
    }

    private static bool IsNeighbor(Vector3Int cell1, Vector3Int cell2)
    {
        return Neighbors(cell1).Contains(cell2);
    }

    public void HighlightNeighbourCells(Unit selectedUnit)
    {
        if (_highlightedTiles.Count != 0)
            UnhighlightCells();
        foreach (var cell in Neighbors(UnitCell(selectedUnit)))
        {
            if (ControllerManager.Instance.AllUnits.ContainsKey(cell) ||
                ControllerManager.Instance.AllCitites.ContainsKey(cell) ||
                _grid.GetTile(cell) == null ||
                MapManager.Instance.GetMoveCosts(selectedUnit.BaseUnit, _grid.GetTile(cell)) > selectedUnit.Moves)
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

    public Vector3Int UnitCell(Unit unit)
    {
        return _grid.LocalToCell(unit.transform.position);
    }
    
    public TileBase GetTileBase(Vector3Int cell)
    {
        return _grid.GetTile(_grid.LocalToCell(cell));
    }
}