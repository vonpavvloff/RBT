using UnityEngine;
using System.Collections.Generic;

public class HexGridObject : MonoBehaviour {

	public HexObstacle visibility;
	public HexObstacle moveability;

	public HexGridCell cell {
		get {
			Transform cur = this.transform;
			while(cur != null) {
				HexGridCell c = cur.GetComponent<HexGridCell>();
				if(c != null)
					return c;
				else
					cur = cur.parent;
			}
			return null;
		}
	}

	public void move(HexDirection dir) {
		HexGridCell c = cell;
		HexGridCell c2 = c.grid.get(c.coordinates + dir);
		if(c2 != null) {
			Vector3 pos = this.transform.localPosition;
			this.transform.parent = c2.transform;
			this.transform.localPosition = pos;
		}
	}

	public HexCoordinates coordinates {
		get {
			return cell.coordinates;
		}
	}
}
