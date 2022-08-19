using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private List<TileData> tileDatas;

    public Dictionary<TileBase, TileData> TilesData { get; private set; }

    private void Awake()
    {
        TilesData = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas) TilesData.Add(tileData.tileBase, tileData);
    }

    public int GetMoveCosts(BaseUnit unit, TileBase tile)
    {
        if (!TilesData.ContainsKey(tile))
            return 1;
        return TilesData[tile].moveCost[unit.UnitType];
    }

    public string GetTileName(TileBase tile)
    {
        return TilesData[tile].tileName;
    }
}