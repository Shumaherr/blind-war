using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridInteractor : BaseInteractable
{
    [SerializeField] private Camera mainCamera;
    public delegate void OnTileSelectedDelegate(Vector3Int tilePos);
    
    private Tilemap _grid;

    public event OnTileSelectedDelegate OnTileSelected;
    // Start is called before the first frame update
    void Start()
    {
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
            if(!_grid.HasTile(clickCellPos) || !IsNeighbor(ControllerManager.Instance.SelectedUnitCell(), clickCellPos))
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
    
    static Vector3Int
        LEFT = new Vector3Int(-1, 0, 0),
        RIGHT = new Vector3Int(1, 0, 0),
        DOWN = new Vector3Int(0, -1, 0),
        DOWNLEFT = new Vector3Int(-1, -1, 0),
        DOWNRIGHT = new Vector3Int(1, -1, 0),
        UP = new Vector3Int(0, 1, 0),
        UPLEFT = new Vector3Int(-1, 1, 0),
        UPRIGHT = new Vector3Int(1, 1, 0);

    static Vector3Int[] directions_when_y_is_even = 
        { LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT };
    static Vector3Int[] directions_when_y_is_odd = 
        { LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT };

    public IEnumerable<Vector3Int> Neighbors(Vector3Int node) {
        Vector3Int[] directions = (node.y % 2) == 0? 
            directions_when_y_is_even: 
            directions_when_y_is_odd;
        foreach (var direction in directions) {
            Vector3Int neighborPos = node + direction;
            yield return neighborPos;
        }
    }

    private bool IsNeighbor(Vector3Int cell1, Vector3Int cell2)
    {
        return Neighbors(cell1).Contains(cell2);
    }
}
