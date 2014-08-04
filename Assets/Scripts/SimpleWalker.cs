using UnityEngine;
using System.Collections;

public class SimpleWalker : HexGridObject {
	public float speed;
	float ratio;
	bool canMove;
	HexGridCell prevCell = null;
	HexGridCell nextCell = null;

	void move(HexDirection dir) {
		prevCell = cell;
		nextCell = cell.grid.get(cell.coordinates + dir);
		if(nextCell == null) {
			canMove = true;
		} else {
			ratio = 0.0f;
			canMove = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if(canMove) {
			if(Input.GetAxis("Horizontal") > 0) {
				if(Input.GetAxis("Vertical") > 0)
					move(HexDirection.TOP_RIGHT);
				else if(Input.GetAxis("Vertical") < 0)
					move(HexDirection.BOTTOM_RIGHT);
			} else if(Input.GetAxis("Horizontal") < 0) {
				if(Input.GetAxis("Vertical") > 0)
					move(HexDirection.TOP_LEFT);
				else if(Input.GetAxis("Vertical") < 0)
					move(HexDirection.BOTTOM_LEFT);
			} else {
				if(Input.GetAxis("Vertical") > 0)
					move(HexDirection.TOP);
				else if(Input.GetAxis("Vertical") < 0)
					move(HexDirection.BOTTOM);
			}
		} else {
			if(nextCell != null) {
				bool wasLess05 = ratio < 0.5;
				ratio += speed * Time.deltaTime;
				if(ratio > 1.0f) {
					ratio = 1.0f;
					canMove = true;
				}
				Vector3 dy = new Vector3(0,transform.position.y,0);
				transform.position = prevCell.transform.position * (1.0f - ratio) + nextCell.transform.position * ratio + dy;
				if(wasLess05 && ratio >= 0.5)
					transform.parent = nextCell.transform;
			} else {
				canMove = true;
			}
		}
	}
}
