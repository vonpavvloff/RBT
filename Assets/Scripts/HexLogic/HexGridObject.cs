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

	public HexCoordinates coordinates {
		get {
			return cell.coordinates;
		}
	}
}
