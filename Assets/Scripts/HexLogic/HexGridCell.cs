using UnityEngine;
using System.Collections.Generic;

public class HexGridCell : MonoBehaviour {

	public HexGrid grid;
	public HexCoordinates coordinates;

	private bool isVisible(HexGridCell otherCell) {
		// Raycasting from current cell to the other cell
		HexGridCell prev1 = null;
		HexGridCell prev2 = null;
		foreach(HexCoordinates c in HexCoordinates.hexLineCover(coordinates,otherCell.coordinates)) {
			HexGridCell cur = grid.get(c);
			if(cur == null)
				return false;
			if(prev1 != null) {
				if(!prev1.isVisibleAdjacent(cur))
					return false;
				if(prev2 != null && HexCoordinates.distance(prev2.coordinates,cur.coordinates) == 1 && !prev2.isVisibleAdjacent(cur))
					return false;
			}
			prev2 = prev1;
			prev1 = cur;
		}
		return true;
	}

	private bool isVisible(HexDirection dir) {
		HexObstacle obstacle = new HexObstacle(0);
		foreach(HexGridObject obj in this.transform.GetComponentsInChildren<HexGridObject>())
			obstacle = obstacle | obj.visibility;
		return !obstacle.get(dir);
	}

	private bool isVisibleAdjacent(HexGridCell adjCell) {
		HexDirection dirToAdj = HexCoordinates.adjacentDirection(this.coordinates,adjCell.coordinates);
		return this.isVisible(dirToAdj) && adjCell.isVisible(-dirToAdj);
	}

	public IEnumerable<HexGridCell> getVisibleCells(int range) {
		yield return this;
		for(int i = 1; i <= range; ++i) {
			// TODO Refactor! Implemented a simple raycasting algorithm for now
			bool foundVisibleCell = false;
			foreach(HexCoordinates c2 in HexCoordinates.circle(this.coordinates,i)) {
				HexGridCell cell = grid.get(c2);
				if(cell == null)
					continue;
				if(this.isVisible(cell)) {
					yield return cell;
					foundVisibleCell = true;
				}
			}
			if(!foundVisibleCell)
				break;
		}
	}
}
