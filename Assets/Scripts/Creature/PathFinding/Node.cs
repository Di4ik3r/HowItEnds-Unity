using System;
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
		gridX = _gridX;
		gridY = _gridY;
        this.walkable = Creature.digitalMap[_gridX, _gridY] == PathFinding.WALKABLE_VALUE ? true : false;
	}

	public Vector2 v() {
		return new Vector2(gridX, gridY);
	}

	public void Refresh() {
		try {
			this.walkable = Creature.digitalMap[gridX, gridY] == PathFinding.WALKABLE_VALUE ? true : false;
		} catch(Exception ex) {
			Debug.Log($"{gridX}, {gridY}");
		}
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