using UnityEngine;
using System.Collections;

[System.Serializable]
public struct HexObstacle {
	public int bitMask;

	public bool get(HexDirection dir) {
		return (bitMask & (1 << dir.direction)) != 0;
	}

	public void set(HexDirection dir, bool val) {
		if(val) {
			bitMask |= (1 << dir.direction);
		} else {
			bitMask &= ~(1 << dir.direction);
		}
	}

	public bool topLeft {
		get {
			return get (HexDirection.TOP_LEFT);
		}
		set {
			set (HexDirection.TOP_LEFT,value);
		}
	}
	public bool topRight {
		get {
			return get (HexDirection.TOP_RIGHT);
		}
		set {
			set (HexDirection.TOP_RIGHT,value);
		}
	}
	public bool top {
		get {
			return get (HexDirection.TOP);
		}
		set {
			set (HexDirection.TOP,value);
		}
	}
	public bool bottomLeft {
		get {
			return get (HexDirection.BOTTOM_LEFT);
		}
		set {
			set (HexDirection.BOTTOM_LEFT,value);
		}
	}
	public bool bottomRight {
		get {
			return get (HexDirection.BOTTOM_RIGHT);
		}
		set {
			set (HexDirection.BOTTOM_RIGHT,value);
		}
	}
	public bool bottom {
		get {
			return get (HexDirection.BOTTOM);
		}
		set {
			set (HexDirection.BOTTOM,value);
		}
	}

	public HexObstacle(int val) {
		bitMask = val;
	}

	public static HexObstacle operator | (HexObstacle o1, HexObstacle o2) {
		return new HexObstacle(o1.bitMask | o2.bitMask);
	}
}
