using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utils;

public class Pathfinding
{
    // Шаг 1.
    List<PathTile> closedList = new List<PathTile>();

    List<PathTile> openList = new List<PathTile>();

    // Шаг 2.
    public  List<Vector3Int> FindPath(Tilemap walkableGrid, Vector3Int start, Vector3Int goal)
    {
        PathTile startNode = new PathTile(start);
        startNode.HeuristicEstimatePathLength = GetHeuristicPathLength(start, goal);

        openList.Add(startNode);
        while (openList.Count > 0)
        {
            // Шаг 3.
            var currentNode = openList.OrderBy(node =>
                node.EstimateFullPathLength).First();
            // Шаг 4.
            if (currentNode.Position == goal)
                return GetPathForNode(currentNode);
            // Шаг 5.
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            // Шаг 6.
            foreach (var neighbourNode in Neighbors(currentNode.Position))
            {
                if(!walkableGrid.HasTile(neighbourNode) || GameManager.Instance.HasUnit(neighbourNode)
                                                        || GameManager.Instance.HasCity(neighbourNode))
                    continue;
                PathTile openNeighbourNode = new PathTile(neighbourNode, currentNode);
                openNeighbourNode.HeuristicEstimatePathLength = GetHeuristicPathLength(neighbourNode, goal);
                openNeighbourNode.PathLengthFromStart =
                    currentNode.PathLengthFromStart + GetDistanceBetweenNeighbours();
                // Шаг 7.
                if (closedList.Count(node => node.Position == neighbourNode) > 0)
                    continue;
                var openNode = openList.FirstOrDefault(node =>
                    node.Position == neighbourNode);
                // Шаг 8.
                if (openNode == null)
                {
                    openList.Add(openNeighbourNode);
                    
                }
                    
                else if (openNode.PathLengthFromStart > openNeighbourNode.PathLengthFromStart)
                {
                    // Шаг 9.
                    openNode.CameFrom = currentNode;
                    openNode.PathLengthFromStart = openNeighbourNode.PathLengthFromStart;
                }
            }
        }

        // Шаг 10.
        return null;
    }
    
    private int GetHeuristicPathLength(Vector3Int from, Vector3Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
    
    private int GetDistanceBetweenNeighbours()
    {
        return 1;
    }
    
    private List<Vector3Int> GetPathForNode(PathTile pathNode)
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