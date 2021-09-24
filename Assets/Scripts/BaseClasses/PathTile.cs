using UnityEngine;

public class PathTile
{
    public PathTile(Vector3Int _position)
    {
        Position = _position;
    }

    public PathTile(Vector3Int _position, PathTile cameFrom)
    {
        Position = _position;
        CameFrom = cameFrom;
    }

    // Координаты точки на карте.
    public Vector3Int Position { get; set; }

    // Длина пути от старта (G).
    public int PathLengthFromStart { get; set; }

    // Точка, из которой пришли в эту точку.
    public PathTile CameFrom { get; set; }

    // Примерное расстояние до цели (H).
    public int HeuristicEstimatePathLength { get; set; }

    // Ожидаемое полное расстояние до цели (F).
    public int EstimateFullPathLength => PathLengthFromStart + HeuristicEstimatePathLength;
}