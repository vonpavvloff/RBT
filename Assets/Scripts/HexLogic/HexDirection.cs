using UnityEngine;
using System.Collections;

public struct HexDirection {

	internal int direction;

	public int dx {
		get {
			switch (direction) {
			case 0:
			case 5:
				return -1;
			case 2:
			case 3:
				return 1;
			default:
				return 0;
			}
		}
	}

	public int dy {
		get {
			switch (direction) {
			case 1:
			case 2:
				return -1;
			case 4:
			case 5:
				return 1;
			default:
				return 0;
			}
		}
	}

	public int dz {
		get {
			switch (direction) {
			case 3:
			case 4:
				return -1;
			case 0:
			case 1:
				return 1;
			default:
				return 0;
			}
		}
	}

	public HexDirection(int dir) {
		direction = dir;
	}
	public static HexDirection BOTTOM_LEFT = new HexDirection(0);
	public static HexDirection BOTTOM = new HexDirection(1);
	public static HexDirection BOTTOM_RIGHT = new HexDirection(2);
	public static HexDirection TOP_RIGHT = new HexDirection(3);
	public static HexDirection TOP = new HexDirection(4);
	public static HexDirection TOP_LEFT = new HexDirection(5);

	public static HexDirection operator -(HexDirection d) {
		return new HexDirection((d.direction + 3) % 6);
	}

	public override string ToString ()
	{
		return string.Format ("[HexDirection: dx={0}, dy={1}, dz={2}]", dx, dy, dz);
	}
}
