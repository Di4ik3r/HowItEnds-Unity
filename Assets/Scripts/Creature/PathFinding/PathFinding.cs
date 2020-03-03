using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinding {
    public static int WALKABLE_VALUE = 0;

	public Stack FindPath(Vector2 startPos, Vector2 targetPos) {
		Grid.Refresh();
		Node startNode = Grid.NodeFromWorldPoint(startPos);
		Node targetNode = Grid.NodeFromWorldPoint(targetPos);

		Heap<Node> openSet = new Heap<Node>(Grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (currentNode == targetNode) {
				Debug.Log("retr");
				return RetracePath(startNode,targetNode);
			}

			foreach (Node neighbour in Grid.GetNeighbours(currentNode)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}

        return new Stack();
	}

	Stack RetracePath(Node startNode, Node endNode) {
		Stack path = new Stack();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Push(new Vector2(currentNode.gridX, currentNode.gridY));
			currentNode = currentNode.parent;
		}

        return path;
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

    public static int GetDistance(Vector2 a, Vector2 b) {
        int dstX = (int)Mathf.Abs(a.x - b.x);
		int dstY = (int)Mathf.Abs(a.y - b.y);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
    }
}