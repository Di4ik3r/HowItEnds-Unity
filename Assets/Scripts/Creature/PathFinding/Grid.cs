using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid {

	// public LayerMask unwalkableMask;
	// public static Vector2 gridWorldSize = new Vector2();
	public static float nodeRadius = 2;
    public static Node[,] grid = Grid.CreateGrid();

	static float nodeDiameter;
	static int  gridSizeX, gridSizeY;
	public static int MaxSize;


	// void Awake() {
		// gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		// gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		// CreateGrid();
	// }

	static Node[,] CreateGrid() {
		gridSizeX = Creature.digitalMap.GetLength(0);
		gridSizeY = Creature.digitalMap.GetLength(1);
		MaxSize = gridSizeX * gridSizeY;
		nodeDiameter = nodeRadius * 2;
		var result = new Node[gridSizeX, gridSizeY];
		// Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				// Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				// bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
				result[x,y] = new Node(x, y);
			}
		}

		// Debug.Log($"{PathFinding.GetDistance(new Vector2(14,11),	new Vector2(14, 2))}");
		// Debug.Log($"{PathFinding.GetDistance(new Vector2(14,11),	new Vector2(14, 1))}");
		// Debug.Log($"{PathFinding.GetDistance(new Vector2(14,11),	new Vector2(14, 0))}");
		

        return result;
	}

	public static void Refresh() {
		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				grid[x, y].Refresh();
			}
		}
	}

	public static List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					if(grid[checkX, checkY].walkable)
						neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}
	

	public static Node NodeFromWorldPoint(Vector2 worldPosition) {
		// float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		// float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		// percentX = Mathf.Clamp01(percentX);
		// percentY = Mathf.Clamp01(percentY);

		// int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		// int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
        
		// Debug.Log("------------------------");
		// Debug.Log($"{grid.GetLength(0)} : {grid.GetLength(1)}");
        // Debug.Log($"{gridSizeX} : {gridSizeY}");
        // Debug.Log(worldPosition);
		
		return grid[(int)worldPosition.x, (int)worldPosition.y];
	}
}