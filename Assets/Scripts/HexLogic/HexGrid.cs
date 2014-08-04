using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class HexGrid : MonoBehaviour {

	public Dictionary<HexCoordinates,HexGridCell> cells;
	public HexGridCell baseCell;
	public HexGridObject gridItem;
	public HexGridObject cursor;
	public float coordinatesScale = 1.0f;
	public int dimX = 20;
	public int dimY = 20;
	public bool showGrid;
	private HexCoordinates curCoord;

	public void attachObject(HexGridObject obj, HexCoordinates coordinates) {
		HexGridCell cell = null;
		Vector3 pos = obj.transform.localPosition;
		cell = get(coordinates);
		obj.transform.parent = cell.transform;
		obj.transform.localPosition = pos;
	}

	public HexGridCell get(HexCoordinates c) {
		HexGridCell cell = null;
		cells.TryGetValue(c,out cell);
		return cell;
	}

	public HexGridCell get(Vector3 c) {
		return get (HexCoordinates.from2D(c / coordinatesScale));
	}

	public static Vector3 MouseToPlanePoint(Camera camera) {
		Vector3 origin = camera.ScreenToWorldPoint(Input.mousePosition);
		Vector3 direction = camera.transform.forward;
		return origin - direction * (origin.y / direction.y);
	}
	[MenuItem("HexControl/GenerateGrid")]
	public static void GenerateGrid() {

		HexGrid grid = GameObject.Find("Grid").GetComponent<HexGrid>();
		grid.cells = new Dictionary<HexCoordinates, HexGridCell>();

		foreach(HexGridCell hgc in grid.GetComponentsInChildren<HexGridCell>()) {
			HexCoordinates c = hgc.coordinates;
			if(c.x < 0 || c.x >= grid.dimX || c.y < -c.x / 2 || c.y >= grid.dimY - c.x / 2 || grid.cells.ContainsKey(c))
				DestroyImmediate(hgc.gameObject);
			else {
				grid.cells.Add(c,hgc);
				hgc.transform.localPosition = c.to2D() * grid.coordinatesScale;
			}
		}

		for(int x = 0; x < grid.dimX; ++x) {
			for(int y = - x / 2; y + x / 2 < grid.dimY; ++y) {
				HexCoordinates c = new HexCoordinates(x,y);
				if(grid.get (c) == null) {
					HexGridCell newCell = Instantiate(grid.baseCell) as HexGridCell;
					newCell.transform.parent = grid.transform;
					newCell.coordinates = c;

					newCell.transform.localPosition = c.to2D() * grid.coordinatesScale;
					grid.cells.Add(c,newCell);
				}
				grid.get(c).name = "Cell " + c.ToString();
			}
		}
		if(grid.gridItem != null) {
			Vector3 scale = grid.gridItem.transform.localScale;
			foreach(HexGridCell hgc in grid.GetComponentsInChildren<HexGridCell>()) {
				Transform trns = hgc.transform.FindChild("GridItem");
				HexGridObject gridItem = null;
				if(trns != null)
					gridItem = trns.GetComponent<HexGridObject>();
				if(gridItem == null) {
					gridItem = Instantiate(grid.gridItem) as HexGridObject;
					gridItem.transform.parent = hgc.transform;
					gridItem.name = "GridItem";
				}
				gridItem.transform.localPosition = new Vector3(0,0,0);
				gridItem.transform.localScale = scale * grid.coordinatesScale;
				if(grid.showGrid)
					gridItem.gameObject.SetActive(true);
				else
					gridItem.gameObject.SetActive(false);
			}
		}
	}

	void Start () {
		cells = new Dictionary<HexCoordinates, HexGridCell>();
		foreach(HexGridCell c in GetComponentsInChildren<HexGridCell>()) {
			Debug.Log(c.coordinates);
			if(!cells.ContainsKey(c.coordinates))
				cells.Add(c.coordinates,c);
			else
				Destroy(c.gameObject);
		}
	}
	
	void Update() {
		if(cursor != null) {
			Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
			if(camera != null) {
				HexGridCell nextCell = get (MouseToPlanePoint(camera));
				if(nextCell != null) {
					Vector3 lp = cursor.transform.localPosition;
					cursor.transform.parent = nextCell.transform;
					cursor.transform.localPosition = lp;
				}
			}
		}
	}
}

