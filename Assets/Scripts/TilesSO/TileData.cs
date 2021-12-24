using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public string tileName;
    public TileBase tileBase;
    public GenericDictionary<UnitType, byte> moveCosts;
    
}
