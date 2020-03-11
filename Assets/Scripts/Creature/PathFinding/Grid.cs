using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid {

	public static float nodeRadius = 2;
    	public static Node[,] grid = Grid.CreateGrid();

	static float nodeDiameter;
	static int  gridSizeX, gridSizeY;
	public static int MaxSize;

	public static void RefreshGrid()
	{
		grid = Grid.CreateGrid();
	}

	static Node[,] CreateGrid() {
		gridSizeX = Creature.digitalMap.GetLength(0);
		gridSizeY = Creature.digitalMap.GetLength(1);
		MaxSize = gridSizeX * gridSizeY;
		nodeDiameter = nodeRadius * 2;
		var result = new Node[gridSizeX, gridSizeY];

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				result[x,y] = new Node(x, y);
			}
		}

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
		Node result = grid[0, 0];
		//try {
			result = grid[(int)worldPosition.x, (int)worldPosition.y];
		//} catch(Exception ex) {	
			//Debug.Log($"{worldPosition}");
			//Debug.Log(ex);
		//}

		return result;
	}
}
