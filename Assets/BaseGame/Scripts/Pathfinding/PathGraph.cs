using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGraph {

    List<GraphNode> nodes;
    Pathfinding pathfinding;
    GridMap3D<GridNode3D> grid;
    private List<GridNode3D> toUpdate;
    private List<GridNode3D> updated;

    public PathGraph() {
        this.pathfinding = Pathfinding.Instance;
        this.grid = pathfinding.GetGrid();
    }

    public void Update() {
        foreach (GridNode3D node in toUpdate) {
            GraphNode newNode = new GraphNode(node);
        }
    }
}

public class GraphNode {
    private List<GraphNode> neighbours;
    private GridNode3D tile;

    public GraphNode(GridNode3D tile) {
        this.tile = tile;
    }

    public void UpdateNeighbours() {

    }
}
