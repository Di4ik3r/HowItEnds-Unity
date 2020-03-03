using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathFinding {
    public static int WALKABLE_VALUE = 0;

	public Transform seeker, target;

    // public static Node[,] map = MapInit(Creature.digitalMap);

    // private static Node[,] MapInit(int[,] digitalMap) {
    //     var xLength = digitalMap.GetLength(0);
    //     var yLength = digitalMap.GetLength(1);

    //     var map = new Node[
    //         xLength,
    //         yLength
    //     ];

    //     for(var i = 0; i < xLength; i++) {
    //         for(var j = 0; j < yLength; j++) {
    //             map[i, j] = new Node(new Vector2(i, j));
    //         }
    //     }

    //     return map;
    // }

	// void Awake() {
	// 	grid = GetComponent<Grid> ();
	// }

	// void Update() {
	// 	FindPath (seeker.position, target.position);
	// }

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
					else {
						//openSet.UpdateItem(neighbour);
					}
				}
			}
		}

        return new Stack();
	}

	Stack RetracePath(Node startNode, Node endNode) {
		Stack path = new Stack();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			// path.Add(currentNode);
            // path.Push(currentNode);
			path.Push(new Vector2(currentNode.gridX, currentNode.gridY));
			currentNode = currentNode.parent;
		}
		// path.Reverse();

		// grid.path = path;
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


// public class PathFinding {

//     public static int WALKABLE_VALUE = 0;

//     public static int MOVE_COST = 10;
//     public static int MOVE_D_COST = 10;

//     public Vector2 from;
//     public Vector2 to;

//     public List<Node> open;
//     public HashSet<Node> close;

//     public static Node[,] map = MapInit(Creature.digitalMap);


//     private static Node[,] MapInit(int[,] digitalMap) {
//         var xLength = digitalMap.GetLength(0);
//         var yLength = digitalMap.GetLength(1);

//         var map = new Node[
//             xLength,
//             yLength
//         ];

//         for(var i = 0; i < xLength; i++) {
//             for(var j = 0; j < yLength; j++) {
//                 map[i, j] = new Node(new Vector2(i, j));
//             }
//         }

//         return map;
//     }

//     public Stack FindPath(Vector2 _from, Vector2 _to) {
//         from = _from;
//         to = _to;

//         open = new List<Node>();
//         close = new HashSet<Node>();

//         var startingNode = new Node(from);
//         var endNode = new Node(to);

//         open.Add(startingNode);

//         var iteration = 0;
//         while(open.Count > 0) {
//         // while(iteration <= 10) {
//             Node current = open[0];
//             for(int i = 1; i < open.Count; i++) {
//                 if(     open[i].f < current.f ||
//                         open[i].f == current.f) {
//                     if(open[i].h < current.h) 
//                         current = open[i];
//                 }
//             }


//             // Debug.Log($"before open: {open.Count}");
//             open.Remove(current);
//             close.Add(current);
//             // Debug.Log($"after open: {open.Count}");

//             // Debug.Log($"current: {current.location} - {endNode.location}");

//             if(current.location == endNode.location) {
//                 Stack path = new Stack();
//                 Node curr = current.parent;

//                 while(curr.location != startingNode.location) {
//                     path.Push(new Vector2(curr.location.x, curr.location.y));
//                     curr = curr.parent;
//                 }

//                 // path.Reverse();

//                 return path;
//             }

//             var neighbours = current.GetNeighbours();
//             foreach (var neighbour in neighbours) {
//                 if(!neighbour.isWalkable || close.Contains(neighbour)) {
//                 // if(close.Contains(neighbour)) {
//                     continue;
//                 }

//                 int newMovementCostToNeighbour = current.g + PathFinding.GetDistance(current, neighbour);

//                 if( newMovementCostToNeighbour < neighbour.g ||
//                         !open.Contains(neighbour)) {
//                     neighbour.g = newMovementCostToNeighbour;
//                     neighbour.h = PathFinding.GetDistance(neighbour, endNode);
//                     neighbour.parent = current;

//                     if(!open.Contains(neighbour)) {
//                         open.Add(neighbour);
//                     }
//                 }
//             }

//             // Debug.Log($"open: {open.Count}");
//             // Debug.Log($"close: {close.Count}");


//             // foreach (var item in open) {
//             //     Debug.Log($"open: {item.location}");
//             // }

//             // foreach (var item in close) {
//             //     Debug.Log($"close: {item.location}");
//             // }

//             iteration++;
//         }

//         return new Stack();
//     }

//     public static int GetDistance(Node a, Node b) {
//         int distanceX = Mathf.Abs((int)a.location.x - (int)b.location.x);
//         int distanceY = Mathf.Abs((int)a.location.y - (int)b.location.y);


//         var result = 0;
//         if(distanceX > distanceY) {
//             result = PathFinding.MOVE_D_COST * distanceY + PathFinding.MOVE_COST * (distanceX - distanceY);
//         }
//         result = PathFinding.MOVE_D_COST * distanceX + PathFinding.MOVE_COST * (distanceY - distanceX);

//         // Debug.Log($"get Distance: {result}");

//         return result;
//     }

//     public static int GetDistance(Vector2 a, Vector2 b) {
//         int distanceX = Mathf.Abs((int)a.x - (int)b.x);
//         int distanceY = Mathf.Abs((int)a.y - (int)b.y);


//         var result = 0;
//         if(distanceX > distanceY) {
//             result = PathFinding.MOVE_D_COST * distanceY + PathFinding.MOVE_COST * (distanceX - distanceY);
//         }
//         result = PathFinding.MOVE_D_COST * distanceX + PathFinding.MOVE_COST * (distanceY - distanceX);

//         // Debug.Log($"get Distance: {result}");

//         return result;
//     }
// }