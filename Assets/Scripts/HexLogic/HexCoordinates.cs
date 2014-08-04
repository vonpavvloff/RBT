using UnityEngine;
using System.Collections.Generic;
using System; 
using UnityEditor;

[System.Serializable]
public struct HexCoordinates {

	public int x;
	public int y;

	public int z {
		get {
			return - x - y;
		}
	}

	public int q {
		get {
			return x;
		}
	}
	public int r {
		get {
			return z + (x - (x & 1)) / 2;
		}
	}
	
	public HexCoordinates(int xx, int yy) {
		x = xx;
		y = yy;
	}

	public override string ToString ()
	{
		return string.Format ("[HexCoordinates: x={0}, y={1}, z={2}, q={3}, r={4}]", x, y, z, q, r);
	}

	public static bool operator == (HexCoordinates c1, HexCoordinates c2) {
		return c1.x == c2.x && c1.y == c2.y;
	}

	public static bool operator != (HexCoordinates c1, HexCoordinates c2) {
		return !(c1 == c2);
	}

	public static HexCoordinates operator - (HexCoordinates c1, HexCoordinates c2) {
		return new HexCoordinates(c1.x - c2.x, c1.y - c2.y);
	}

	public static HexCoordinates operator + (HexCoordinates c1, HexDirection d2) {
		return new HexCoordinates(c1.x + d2.dx, c1.y + d2.dy);
	}

	public static HexCoordinates operator + (HexCoordinates c1, HexCoordinates c2) {
		return new HexCoordinates(c1.x + c2.x, c1.y + c2.y);
	}

	public static int distance(HexCoordinates c1, HexCoordinates c2) {
		return (Mathf.Abs(c1.x - c2.x) + Mathf.Abs(c1.y - c2.y) + Mathf.Abs(c1.z - c2.z)) / 2;
	}

	public static HexDirection adjacentDirection(HexCoordinates c1, HexCoordinates c2) {
		HexCoordinates delta = c2 - c1;
		if(delta.x > 0) {
			if(delta.y < 0)
				return HexDirection.BOTTOM_RIGHT;
			else
				return HexDirection.TOP_RIGHT;
		} else if(delta.y > 0) {
			if(delta.x < 0)
				return HexDirection.TOP_LEFT;
			else
				return HexDirection.TOP;
		} else {
			if(delta.x < 0)
				return HexDirection.BOTTOM_LEFT;
			else
				return HexDirection.BOTTOM;
		}
	}

	public HexCoordinates adjacent(HexDirection dir) {
		return this + dir;
	}

	private Vector3 toVec() {
		return new Vector3(x,y,z);
	}

	public static HexCoordinates hexRound(Vector3 v) {
		int rx = Mathf.RoundToInt(v.x);
		int ry = Mathf.RoundToInt(v.y);
		int rz = Mathf.RoundToInt(v.z);
		float dx = Mathf.Abs(rx - v.x);
		float dy = Mathf.Abs(ry - v.y);
		float dz = Mathf.Abs(rz - v.z);
		if(dx > dy && dx > dz)
			rx = -ry-rz;
		else if(dy > dz)
			ry = -rx-rz;
		else
			rz = -rx-ry;
		return new HexCoordinates(rx,ry);
	}

	public static IEnumerable<HexCoordinates> hexLine(HexCoordinates c1, HexCoordinates c2) {
		int length = distance(c1,c2);
		float l = length;
		Vector3 v1 = c1.toVec() + new Vector3(1e-6f, 1e-6f, -2e-6f);
		Vector3 v2 = c2.toVec();
		for(int i = 0; i <= length; ++i) {
			Vector3 v3 = v1 * (1.0f - i / l) + v2 * i / l;
			yield return hexRound(v3);
		}
	}

	private static IEnumerable<HexCoordinates> circle(int radius) {
		HexCoordinates current = new HexCoordinates(0,radius);
		if(radius == 0)
			yield return current;
		else {
			for(int i = 0; i < radius * 6; ++i) {
				yield return current;
				current = current + new HexDirection(i / radius);
			}
		}
	}

	public static IEnumerable<HexCoordinates> circle(HexCoordinates origin, int radius) {
		foreach(HexCoordinates c in circle(radius)) {
			yield return c + origin;
		}
	}

	private static IEnumerable<HexCoordinates> line(HexCoordinates origin, HexDirection dir) {
		while(true) {
			yield return origin;
			origin = origin + dir;
		}  
	}

	public static IEnumerable<HexCoordinates> hexLineCover(HexCoordinates c1, Vector3 c2approx) {
		return hexLineCover(c1, hexRound(c2approx * 10000000));
	}

	public static IEnumerable<HexCoordinates> hexLineCover(HexCoordinates c1, HexCoordinates c2) {
		if(c1 == c2) {
			yield return c1;
			yield break;
		}
		// Does guarantee that the cells would be yielded in the order of increasing distance from c1
		int dx = c2.x - c1.x;
		int dy = c2.y - c1.y;
		int dz = c2.z - c1.z;
		int maxCoordinatesDelta;
		int secondCoordinatesDelta;
		HexDirection mainDir;
		HexDirection nextLineDir;
		if(Mathf.Abs(dx) > Mathf.Abs(dy) && Mathf.Abs(dx) > Mathf.Abs(dz)) {
			maxCoordinatesDelta = dx;
			if(Mathf.Abs(dy) > Mathf.Abs(dz)) {
				secondCoordinatesDelta = dy;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.BOTTOM_RIGHT;
					nextLineDir = HexDirection.TOP_RIGHT;
				} else {
					mainDir = HexDirection.TOP_LEFT;
					nextLineDir = HexDirection.BOTTOM_LEFT;
				}
			} else {
				secondCoordinatesDelta = dz;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.TOP_RIGHT;
					nextLineDir = HexDirection.BOTTOM_RIGHT;
				} else {
					mainDir = HexDirection.BOTTOM_LEFT;
					nextLineDir = HexDirection.TOP_LEFT;
				}
			}
		} else if (Mathf.Abs(dy) > Mathf.Abs(dz)) {
			maxCoordinatesDelta = dy;
			if(Mathf.Abs(dx) > Mathf.Abs(dz)) {
				secondCoordinatesDelta = dx;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.TOP_LEFT;
					nextLineDir = HexDirection.TOP;
				} else {
					mainDir = HexDirection.BOTTOM_RIGHT;
					nextLineDir = HexDirection.BOTTOM;
				}
			} else {
				secondCoordinatesDelta = dz;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.TOP;
					nextLineDir = HexDirection.TOP_LEFT;
				} else {
					mainDir = HexDirection.BOTTOM;
					nextLineDir = HexDirection.BOTTOM_RIGHT;
				}
			}
		} else {
			maxCoordinatesDelta = dz;
			if(Mathf.Abs(dx) > Mathf.Abs(dy)) {
				secondCoordinatesDelta = dx;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.BOTTOM_LEFT;
					nextLineDir = HexDirection.BOTTOM;
				} else {
					mainDir = HexDirection.TOP_RIGHT;
					nextLineDir = HexDirection.TOP;
				}
			} else {
				secondCoordinatesDelta = dy;
				if(maxCoordinatesDelta > 0) {
					mainDir = HexDirection.BOTTOM;
					nextLineDir = HexDirection.BOTTOM_LEFT;
				} else {
					mainDir = HexDirection.TOP;
					nextLineDir = HexDirection.TOP_RIGHT;
				}
			}
		}

		IEnumerator<HexCoordinates> curLine = line(c1,mainDir).GetEnumerator();
		curLine.MoveNext();
		IEnumerator<HexCoordinates> nextLine = line(c1 + nextLineDir,mainDir).GetEnumerator();
		nextLine.MoveNext();

		int dmain = Mathf.Abs(maxCoordinatesDelta) * 2 - Mathf.Abs(maxCoordinatesDelta + secondCoordinatesDelta);
		int dsec = Mathf.Abs(maxCoordinatesDelta + secondCoordinatesDelta) * 3;
		int linesSwitched = 0;


		for(int i = 0; i <= dmain; ++i) {
			int curdelta = i * dsec - linesSwitched * 3 * dmain;
			if(dmain > dsec) {
				if((i + linesSwitched) % 2 == 1) {
					// Odd steps, compare with 1 and check next line
					if(curdelta > dmain) {
						// Paint next line
						yield return nextLine.Current;
						if(curdelta >= dmain * 2) {
							curLine = nextLine;
							nextLine = line(curLine.Current + nextLineDir,mainDir).GetEnumerator();
							linesSwitched += 1;
							curLine.MoveNext();
						}
					}
					nextLine.MoveNext();
				}
				else {
					// Even steps compare with 2 and check current line
					if(curdelta < 2 * dmain) {
						// Paint current line
						yield return curLine.Current;
					}
					curLine.MoveNext();
				}
			} else {
				// The case when we are moving along diagonal is special
				if((i + linesSwitched) % 2 == 1) {
					// Odd steps, compare with 1 and check next line
					if(curdelta >= dmain) {
						// Paint next line
						yield return nextLine.Current;
						if(curdelta >= dmain * 2) {
							curLine = nextLine;
							nextLine = line(curLine.Current + nextLineDir,mainDir).GetEnumerator();
							linesSwitched += 1;
							curLine.MoveNext();
						}
					}
					nextLine.MoveNext();
				}
				else {
					// Even steps compare with 2 and check current line
					if(curdelta <= 2 * dmain) {
						// Paint current line
						yield return curLine.Current;
					}
					curLine.MoveNext();
				}
			}
		}
	}

	public override bool Equals (object obj) {
		if(obj is HexCoordinates)
			return ((HexCoordinates) obj) == this;
		else
			return base.Equals (obj);
	}

	public override int GetHashCode ()
	{
		// TODO Check the correctness of the given hash.
		return x.GetHashCode() + y.GetHashCode() + z.GetHashCode();
	}

	public Vector3 to2D() {
		float xx = x - (y + z) / 2.0f;
		float zz = (y - z) * Mathf.Sqrt(3f) / 2.0f;
		return new Vector3(xx,0,zz);
	}

	public static HexCoordinates from2D(Vector3 pp) {
		float x = pp.x * 2 / 3;
		float y = pp.z / Mathf.Sqrt(3f) - pp.x / 3f;
		return hexRound(new Vector3(x,y,-x-y));
	}
}

