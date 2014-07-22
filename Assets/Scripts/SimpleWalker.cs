using UnityEngine;
using System.Collections;

public class SimpleWalker : HexGridObject {

	// Update is called once per frame
	void Update () {
		HexGridObject o = GetComponent<HexGridObject>();
		if(Input.GetAxis("Horizontal") > 0) {
			if(Input.GetAxis("Vertical") > 0)
				o.move(HexDirection.TOP_RIGHT);
			else if(Input.GetAxis("Vertical") < 0)
				o.move(HexDirection.BOTTOM_RIGHT);
		} else if(Input.GetAxis("Horizontal") < 0) {
			if(Input.GetAxis("Vertical") > 0)
				o.move(HexDirection.TOP_LEFT);
			else if(Input.GetAxis("Vertical") < 0)
				o.move(HexDirection.BOTTOM_LEFT);
		} else {
			if(Input.GetAxis("Vertical") > 0)
				o.move(HexDirection.TOP);
			else if(Input.GetAxis("Vertical") < 0)
				o.move(HexDirection.BOTTOM);
		}

	}
}
