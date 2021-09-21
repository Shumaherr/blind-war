using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utils;

public class GridInteractor : BaseInteractable
{
    [SerializeField] private Camera mainCamera;

    private List<Vector3Int> _highlightedTiles;
    public delegate void OnTileSelectedDelegate(Vector3Int tilePos);
    public event OnTileSelectedDelegate OnTileSelected;
    
    private Tilemap _grid;
    
    // Start is called before the first frame update
    void Start()
    {
        _highlightedTiles = new List<Vector3Int>();
        _grid = GetComponent<Tilemap>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 4;
            Vector2 clickWorldPos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector3Int clickCellPos = _grid.WorldToCell(clickWorldPos);
            if(!_grid.HasTile(clickCellPos) || !IsNeighbor(ControllerManager.Instance.SelectedUnitCell(), clickCellPos) ||
               GameManager.Instance.TakenCells.Contains(clickCellPos))
                return;
            Debug.Log(clickCellPos);
            if (OnTileSelected != null)
                OnTileSelected.Invoke(clickCellPos);
        }
    }

    public override void Interact()
    {
        
    }

    private void OnMouseDown()
    {
        //Create event
    }


    public GridInteractor(List<Vector3Int> highlightedTiles)
    {
        _highlightedTiles = highlightedTiles;
    }

    private bool IsNeighbor(Vector3Int cell1, Vector3Int cell2)
    {
        return Neighbors(cell1).Contains(cell2);
    }

    public void HighlightNeighbourCells(Vector3Int cellToHighlight)
    {
        if(_highlightedTiles.Count != 0)
            UnhighlightCells();
        foreach (var cell in Neighbors(ControllerManager.Instance.SelectedUnitCell()))
        {
            if(GameManager.Instance.TakenCells.Contains(cell))
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
        foreach (var tile in _highlightedTiles)
        {
            _grid.SetColor(tile, Color.white);
        }
    }
}

