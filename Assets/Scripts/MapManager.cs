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

    public int GetTurnPoints(BaseUnit unit, TileBase tile)
    {
        if (!_tilesData.ContainsKey(tile))
            return unit.Moves;
        return unit.Moves * (_tilesData[tile].movePenalty[unit.UnitType] / 100);
    }

    public string GetTileName(TileBase tile)
    {
        return _tilesData[tile].tileName;
    }
    
    
    
}
