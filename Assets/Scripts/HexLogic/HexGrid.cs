using UnityEngine;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour {

	public Dictionary<HexCoordinates,HexGridCell> cells;
	public HexGridCell baseCell;
	public HexGridObject testObject;
	public SimpleWalker walker;
	public HexGridObject wall;
	private HexCoordinates curCoord;

	public void attachObject(HexGridObject obj, HexCoordinates coordinates) {
		if(!cells.ContainsKey(coordinates)) {
			HexGridCell newCell = Instantiate(baseCell) as HexGridCell;
			newCell.transform.parent = this.transform;
			newCell.coordinates = coordinates;
			newCell.transform.localPosition = coordinates.to2D();
			cells.Add(coordinates,newCell);
		}
		HexGridCell cell = null;
		Vector3 pos = obj.transform.localPosition;
		cells.TryGetValue(coordinates,out cell);
		obj.transform.parent = cell.transform;
		obj.transform.localPosition = pos;
	}

	public HexGridCell get(HexCoordinates c) {
		HexGridCell cell = null;
		cells.TryGetValue(c,out cell);
		return cell;
	}

	void Start() {
		cells = new Dictionary<HexCoordinates, HexGridCell>();
		for(int x = 0; x < 20; ++x) {
			for(int y = - x / 2; y + x / 2 < 20; ++y) {
				HexGridObject obj = Instantiate(testObject) as HexGridObject;
				attachObject(obj,new HexCoordinates(x,y));
			}
		}
		attachObject(walker,new HexCoordinates(3,3));
		curCoord = walker.coordinates;
		attachObject(wall,new HexCoordinates(4,4));
	}

	void Update() {
		if(walker.coordinates != curCoord) {
			foreach(HexGridCell cell in cells.Values)
				foreach(Renderer r in cell.GetComponentsInChildren<Renderer>())
					r.enabled = false;
			//foreach(HexCoordinates c in HexCoordinates.hexLine(new HexCoordinates(10,10),walker.coordinates)) {
			//	HexGridCell cell = get(c);
			//	if(cell != null)
			//		foreach(Renderer r in cell.GetComponentsInChildren<Renderer>())
			//			r.enabled = true;
			//}
			foreach(HexGridCell cell in walker.cell.getVisibleCells(10))
				foreach(Renderer r in cell.GetComponentsInChildren<Renderer>())
					r.enabled = true;
			curCoord = walker.coordinates;
		}
	}
}
