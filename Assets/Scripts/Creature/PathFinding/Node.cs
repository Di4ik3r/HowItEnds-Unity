using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Node : IHeapItem<Node> {
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	
	public Node(int _gridX, int _gridY) {
		// walkable = _walkable;
		// worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
        this.walkable = Creature.digitalMap[_gridX, _gridY] == PathFinding.WALKABLE_VALUE ? true : false;
		// this.walkable = true;
	}

	public Vector2 v() {
		return new Vector2(gridX, gridY);
	}

	public void Refresh() {
		this.walkable = Creature.digitalMap[gridX, gridY] == PathFinding.WALKABLE_VALUE ? true : false;
	}

	public int fCost {
		get {
			return gCost + hCost;
		}
	}

	int heapIndex;
	public int HeapIndex {
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare) {
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}
		return -compare;
	}
}

// public class Node {

//         public int g;
//         public int h;
//         public int f {
//             get {
//                 return g + h;
//             }
//         }

//         public Node parent;

//         public Vector2 location;
//         public bool isWalkable;

//         public Node(Vector2 _location) {
//             this.location = _location;
//             // this.isWalkable = true;
//             this.isWalkable = Creature.digitalMap[(int)_location.x, (int)_location.y] == PathFinding.WALKABLE_VALUE ? true : false;
//         }

//         public List<Node> GetNeighbours() {
//             var result = new List<Node>();

//             for(int x = -1; x <= 1; x++) {
//                 for(int y = -1; y <= 1; y++) {
//                     if(x == 0 && y == 0)
//                         continue;
//                     int checkX = (int)this.location.x + x;
//                     int checkY = (int)this.location.y + y;

//                     if(checkX >= 0 && checkX < Creature.digitalMap.GetLength(0)
//                         && checkY >= 0 && checkY < Creature.digitalMap.GetLength(1))
//                     result.Add(PathFinding.map[checkX, checkY]);
//                 }
//             }

//             return result;
//         }

//         public List<Node> getNeighbours_deprecated() {
//             // var indexiesSum = this.location.x + this.location.y;

//             var result = new List<Node>();

//             for(int x = -1; x <= 1; x++) {
//                 for(int y = -1; y <= 1; y++) {
//                     if(x == 0 && y == 0)
//                         continue;
//                     int checkX = (int)this.location.x + x;
//                     int checkY = (int)this.location.y + y;

//                     if(checkX >= 0 && checkX < Creature.digitalMap.GetLength(0)
//                         && checkY >= 0 && checkY < Creature.digitalMap.GetLength(1))
//                     result.Add(new Node(new Vector2(checkX, checkY)));
//                 }
//             }

//             // var x1 = this.location.x - 1;
//             // var x2 = this.location.x + 1;
//             // var y1 = this.location.y - 1;
//             // var y2 = this.location.y + 1;

//             // if(x1 >= 0)
//             //     result[resultIndexer++] = new Node(x1, this.location.y);
//             // if(x2 >= 0)
//             //     result[resultIndexer++] = new Node(x2, this.location.y);
//             // if(y1 >= 0)
//             //     result[resultIndexer++] = new Node(this.location.x, y1);
//             // if(y2 >= 0)
//             //     result[resultIndexer++] = new Node(this.location.y, y2);

//             return result;
//         }
//     }