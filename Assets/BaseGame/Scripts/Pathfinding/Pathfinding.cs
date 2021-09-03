using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {


    public static Pathfinding Instance {get; private set;}
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_DOUBLE_DIAGONAL_COST = 17;

    private GridMap3D<GridNode3D> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    public Pathfinding(GridMap3D<GridNode3D> grid) {
        Instance = this;
        this.grid = grid;
    }

    public GridMap3D<GridNode3D> GetGrid() {
        return grid;
    }


    public List<Vector3> FindPath_Vectors(int startX, int startY, int startZ, int endX, int endY, int endZ) {
        List<PathNode> nodeList = FindPath(startX, startY, startZ, endX, endY, endZ);
        List<Vector3> vectorList = new List<Vector3>();
        foreach (PathNode node in nodeList) {
            vectorList.Add(new Vector3(node.x, node.y, node.z) * grid.GetCellSize());
        }
        return vectorList;
    }

    public List<PathNode> FindPath(int startX, int startY, int startZ, int endX, int endY, int endZ) {
        PathNode startNode = grid.GetGridObject(startX, startY, startZ).GetPathNode();
        PathNode endNode = grid.GetGridObject(endX, endY, endZ).GetPathNode();

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                for (int z = 0; z < grid.GetDepth(); z++) {
                    PathNode pathNode = grid.GetGridObject(x, y, z).GetPathNode();
                    pathNode.gCost = int.MaxValue;
                    pathNode.CalculateFCost();
                    pathNode.prevNode = null;
                }
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCost(openList);
            if (currentNode == endNode) {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (!neighbourNode.IsWalkable()) {
                    closedList.Add(neighbourNode);
                    continue;
                }
                if (closedList.Contains(neighbourNode)) {
                    continue;
                }
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.prevNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        // Out of nodes on the open list
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.x - 1 >= 0) {
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y, currentNode.z));
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y, currentNode.z - 1));
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y, currentNode.z + 1));
        }
        if (currentNode.x + 1 < grid.GetWidth()) {
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y, currentNode.z));
            if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y, currentNode.z - 1));
            if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y, currentNode.z + 1));
        }
        if (currentNode.z - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y, currentNode.z - 1));
        if (currentNode.z + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y, currentNode.z + 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int y, int z) {
        return grid.GetGridObject(x, y, z).GetPathNode();
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCost(List<PathNode> pathNodeList) {
        PathNode lowestCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestCostNode.fCost) {
                lowestCostNode = pathNodeList[i];
            }
        }
        return lowestCostNode;
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        PathNode currentNode = endNode;
        path.Insert(0, currentNode);
        while (currentNode.prevNode != null) {
            path.Insert(0, currentNode.prevNode);
            currentNode = currentNode.prevNode;
        }
        return path;
    }
}
