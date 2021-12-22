using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private List<TileData> tileDatas;
    
    private Dictionary<TileBase, TileData> _tilesData;

    public Dictionary<TileBase, TileData> TilesData => _tilesData;

    private void Awake()
    {
        _tilesData = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            _tilesData.Add(tileData.tileBase, tileData);
        }
    }

    public byte GetTurnPoints(UnitType unitType, TileBase tile)
    {
        return _tilesData[tile].maxMovement[unitType];
    }

    public string GetTileName(TileBase tile)
    {
        return _tilesData[tile].tileName;
    }
    
}
