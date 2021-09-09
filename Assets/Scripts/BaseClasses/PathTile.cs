using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTile
{
    private PathTile cameFrom;
    private int f = 0;
    private int g = 0;
    private int h = 0;

    private Vector3Int position;
}
