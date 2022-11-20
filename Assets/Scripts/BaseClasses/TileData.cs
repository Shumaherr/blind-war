using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public string tileName;
    public TileBase tileBase;
    public GenericDictionary<UnitType, int> moveCost;
}