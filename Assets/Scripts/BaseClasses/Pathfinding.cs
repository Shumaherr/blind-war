using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utils;

public class Pathfinding
{
    public List<Vector3Int> FindPath(Tilemap walkableGrid, Vector3Int start, Vector3Int goal)
    {
        var closedList = new List<PathTile>();
        var openList = new List<PathTile>();
        var startNode = new PathTile(start);
        startNode.HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal);

        openList.Add(startNode);
        while (openList.Count > 0)
        {
            var currentNode = openList.OrderBy(node =>
                node.EstimateFullPathLength).First();
            if (currentNode.Position == goal)
                return GetPathForNode(currentNode);
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            foreach (var neighbourNode in Neighbors(currentNode.Position))
            {
                if (!walkableGrid.HasTile(neighbourNode) ||
                    (GameManager.Instance.HasUnit(neighbourNode) || GameManager.Instance.HasCity(neighbourNode)) &&
                    neighbourNode != goal)
                    continue;
                var openNeighbourNode = new PathTile(neighbourNode, currentNode);
                openNeighbourNode.HeuristicEstimatePathLength = GetHeuristicPathLength(neighbourNode, goal);
                openNeighbourNode.PathLengthFromStart =
                    currentNode.PathLengthFromStart + GetDistanceBetweenNeighbours();
                if (closedList.Count(node => node.Position == neighbourNode) > 0)
                    continue;
                var openNode = openList.FirstOrDefault(node =>
                    node.Position == neighbourNode);
                if (openNode == null)
                {
                    openList.Add(openNeighbourNode);
                }

                else if (openNode.PathLengthFromStart > openNeighbourNode.PathLengthFromStart)
                {
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = openNeighbourNode.PathLengthFromStart;
                }
            }
        }

        return null;
    }

    private static int GetHeuristicPathLength(Vector3Int from, Vector3Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }

    private static int GetDistanceBetweenNeighbours()
    {
        return 1;
    }

    private static List<Vector3Int> GetPathForNode(PathTile pathNode)
    {
        var result = new List<Vector3Int>();
        var currentNode = pathNode;
        while (currentNode != null)
        {
            result.Add(currentNode.Position);
            currentNode = currentNode.CameFrom;
        }

        result.Reverse();
        return result;
    }
}