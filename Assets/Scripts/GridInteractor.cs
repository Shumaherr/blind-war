using System;
using System.Collections;
using System.Collections.Generic;
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
            if(!_grid.HasTile(clickCellPos))
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
}
