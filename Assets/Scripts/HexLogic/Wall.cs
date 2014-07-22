using UnityEngine;
using System.Collections;

public class Wall : HexGridObject {

	public int wallType;

	// Use this for initialization
	void Start () {
		visibility.bitMask = wallType;
		moveability.bitMask = wallType;
	}
}
