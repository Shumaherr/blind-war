using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static readonly Vector3Int
        LEFT = new Vector3Int(-1, 0, 0);

    private static readonly Vector3Int
        RIGHT = new Vector3Int(1, 0, 0);

    private static readonly Vector3Int
        DOWN = new Vector3Int(0, -1, 0);

    private static readonly Vector3Int
        DOWNLEFT = new Vector3Int(-1, -1, 0);

    private static readonly Vector3Int
        DOWNRIGHT = new Vector3Int(1, -1, 0);

    private static readonly Vector3Int
        UP = new Vector3Int(0, 1, 0);

    private static readonly Vector3Int
        UPLEFT = new Vector3Int(-1, 1, 0);

    private static readonly Vector3Int
        UPRIGHT = new Vector3Int(1, 1, 0);

    private static readonly Vector3Int[] directions_when_y_is_even =
        {LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT};

    private static readonly Vector3Int[] directions_when_y_is_odd =
        {LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT};

    public static IEnumerable<Vector3Int> Neighbors(Vector3Int node)
    {
        var directions = node.y % 2 == 0 ? directions_when_y_is_even : directions_when_y_is_odd;
        foreach (var direction in directions)
        {
            var neighborPos = node + direction;
            yield return neighborPos;
        }
    }
    
    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = Random.Range(0, n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}

