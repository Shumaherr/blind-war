using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utils
{
    private static readonly Vector3Int
        LEFT = new(-1, 0, 0);

    private static readonly Vector3Int
        RIGHT = new(1, 0, 0);

    private static readonly Vector3Int
        DOWN = new(0, -1, 0);

    private static readonly Vector3Int
        DOWNLEFT = new(-1, -1, 0);

    private static readonly Vector3Int
        DOWNRIGHT = new(1, -1, 0);

    private static readonly Vector3Int
        UP = new(0, 1, 0);

    private static readonly Vector3Int
        UPLEFT = new(-1, 1, 0);

    private static readonly Vector3Int
        UPRIGHT = new(1, 1, 0);

    private static readonly Vector3Int[] directions_when_y_is_even =
        { LEFT, RIGHT, DOWN, DOWNLEFT, UP, UPLEFT };

    private static readonly Vector3Int[] directions_when_y_is_odd =
        { LEFT, RIGHT, DOWN, DOWNRIGHT, UP, UPRIGHT };

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
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /*public static Vector3Int GetRandomCell(Tilemap tilemap)
    {
        var cellBounds = tilemap.cellBounds;
        return new Vector3Int(Random.Range(cellBounds.xMin, cellBounds.xMax),
            Random.Range(tilemap.cellBounds.yMin, cellBounds.yMax),
            Random.Range(tilemap.cellBounds.zMin, cellBounds.zMax));
    }*/
    public static Vector3Int GetRandomCell(Tilemap tilemap)
    {
        var cellBounds = tilemap.cellBounds;
        return new Vector3Int(Random.Range(-3, 10),
            Random.Range(-3, 10),
            Random.Range(tilemap.cellBounds.zMin, cellBounds.zMax));
    }
}