using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Represents the grid map of the city as collection of GridCells.
/// </summary>
public class GridMap
{
	// Fields
	private int cellSize = 10;
	public GridCell[,] cells = new GridCell[51, 51];
	private List<Vector3> roads = new();
	private string saveFilePath = "C:\\Users\\Ali\\Documents\\gridmap.json";


	public void Initialize()
	{
		if (File.Exists(saveFilePath))
		{
			Debug.Log("GridMap: gridmap.json found");
			string json = File.ReadAllText(saveFilePath);
			Deserialize(json);
			Debug.Log("Called Deserialize");

		}
		else
		{
			Debug.Log("GridMap: No save found");
			InitializeGrid();
			string json = Serialize();
			File.WriteAllText(saveFilePath, json);
			Debug.Log("GridMap: initialized and serialized");
		}
	}

	/// <summary>
	/// Initializes the grid map with empty/default cells.
	/// </summary>
	public void InitializeGrid()
	{
		for (int x = 0; x < 51; x++)
		{
			for (int y = 0; y < 51; y++)
			{
				Vector3Int pos = new Vector3Int(x * cellSize, 0, y * cellSize);
				cells[x, y] = new GridCell(pos);
			}
		}
	}


	/// <summary>
	/// EventHandler for OnCreateZone event defined in City class.
	/// Subscription is handled in GameManager to ensure its subscribed to the correct instance.
	/// </summary>
	/// <param name="city"></param>
	/// <param name="zone"></param>
	public void OnCreateZoneHandler(City city, Zone zone)
	{
		UpdateGridMap(zone);
		Debug.Log("GridMap: Called UpdateGridMap()");
	}


	/// <summary>
	/// Generic method that can be used with any BuildingType to reflect change in the GridMap.cells
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="building"></param>
	/// <remarks>
	/// This method only updates the GridCell.building, GridCell.buildingId and GridCell.isOccupied
	/// </remarks>
	public void OnBuildingPlacedHandler(Vector3 pos, Building building)
	{
		Debug.Log($"GridMap: OnBuildingPlacedHandler() called, placing {building}");
		Debug.Log($"GridMap: OnBuildingPlacedHandler() called, placing {building}");
		int i = Mathf.FloorToInt(pos.x / cellSize);
		int j = Mathf.FloorToInt(pos.z / cellSize);

		cells[i, j].isOccupied = true;
		cells[i, j].building = building;
		cells[i, j].buildingId = building.id;

		if (building is RoadStructure)
		{
			Debug.Log("GridMap: Building is road");
			cells[i, j].isRoad = true;
			// roads.Add(cells[i, j].pos);
		}

		Debug.Log($"GridMap: Placed {building} at {pos}");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="affectedCells"></param>
	public void OnUtilityDistributeHandler(UtilityType type, List<Vector3> affectedCells)
	{
		// TODO: optimise this loop
		foreach (var cell in cells)
		{
			foreach (var pos in affectedCells)
			{
				if (cell.pos == pos)
				{
					switch (type)
					{
						case UtilityType.Power:
							cell.hasPower = true;
							break;
						case UtilityType.Water:
							cell.hasWater = true;
							break;
					}
				}
			}
		}

		Debug.Log("GridMap: Called OnUtilityDistributeHandler()");
	}

	public void OnServiceDistributeHandler(ServiceType serviceType, List<Vector3> affectedCells)
	{
		Debug.Log("GridMap: OnServiceDistributeHandler()");
		foreach (var cell in cells)
		{
			if (affectedCells.Contains(cell.pos))
			{
				switch (serviceType)
				{
					case ServiceType.Hospital:
						cell.hasHospital = true;
						break;
					case ServiceType.School:
						cell.hasSchool = true;
						break;
					case ServiceType.PoliceStation:
						cell.hasPoliceStation = true;
						break;
					case ServiceType.FireStation:
						cell.hasFireStation = true;
						break;
					default:
						Debug.LogWarning($"GridMap: Unknown service type {serviceType}");
						break;
				}
			}
		}
		Debug.Log($"GridMap: Called OnServiceDistributeHandler() for service type {serviceType}");

		// Optional debugging
		// int count = 0;
		// foreach (var cell in cells)
		// {
		// 	if (cell.hasHospital)
		// 	{
		// 		Debug.Log($"GridMap: Cell at {cell.pos} hasHospital");
		// 		count++;
		// 	}
		// 	if (cell.hasSchool)
		// 	{
		// 		Debug.Log($"GridMap: Cell at {cell.pos} hasSchool");
		// 		count++;
		// 	}
		// 	if (cell.hasPoliceStation)
		// 	{
		// 		Debug.Log($"GridMap: Cell at {cell.pos} hasPoliceStation");
		// 		count++;
		// 	}
		// 	if (cell.hasFireStation)
		// 	{
		// 		Debug.Log($"GridMap: Cell at {cell.pos} hasFireStation");
		// 		count++;
		// 	}
		// }
		// Debug.Log($"GridMap: hasHospital count: {count}");
	}

	/// <summary>
	/// Update "cells" array when a new zone gets created.
	/// Convert actual cell.pos to position in the cells[,] and assigns zone.
	/// </summary>
	public void UpdateGridMap(Zone zone)
	{
		Debug.Log("GridMap: Executing UpdateGridMap");
		Debug.Log($"GridMap: zoneId:{zone.zoneId}");
		int startX = (int)zone.data.startPos.x;
		int endX = (int)zone.data.endPos.x;
		int startY = (int)zone.data.startPos.z;
		int endY = (int)zone.data.endPos.z;

		if (startX > endX)
			(endX, startX) = (startX, endX);

		if (startY > endY)
			(endY, startY) = (startY, endY);

		for (int x = startX; x <= endX; x++)
		{
			for (int y = startY; y <= endY; y++)
			{
				cells[x / 10, y / 10].zone = zone;
				cells[x / 10, y / 10].zoneId = zone.zoneId;
			}
		}
	}

	/// <summary>
	/// Updates the cells based on the affected positions and the update action.
	/// This is a generic method that is called by other methods performing the same thing but only
	/// cell.SomeValue is different.
	/// </summary>
	/// <param name="affectedCells">List of affected cell positions.</param>
	/// <param name="updateAction">Action to update the cell properties.</param>
	private void UpdateCells(List<Vector3> affectedCells, Action<GridCell> updateAction)
	{
		foreach (var cell in cells)
		{
			foreach (var pos in affectedCells)
			{
				if (cell.pos == pos)
				{
					updateAction(cell);
				}
			}
		}
	}

	/// <summary>
	/// Handles the event when a road is built. This is only applicable for straight roads.
	/// </summary>
	/// <param name="affectedCells">List of affected cell positions.</param>
	public void OnRoadBuiltHandler(List<Vector3> affectedCells, Quaternion rotation)
	{
		roads.AddRange(affectedCells);

		// foreach (var cell in affectedCells)
		// {
		// 	 Debug.Log($"GridMap affected cell: {cell}");
		// }

		UpdateCells(affectedCells, cell =>
		{
			cell.isOccupied = true;
			cell.isRoad = true;
			cell.roadRotation = rotation;
		});

		InfrastructureManager.InstantiateRoad_Straight(affectedCells, rotation);
		Debug.Log("GridMap: Called OnRoadBuiltHandler()");
	}


	/// <summary>
	/// Serialize the GridMap.cells 2d array using a wrapper class. 
	/// </summary>
	/// <returns>JSON representation of GridMap.cells as a single string.</returns>
	public string Serialize()
	{
		// Convert the 2D array to a serializable list
		List<GridCell> cellsList = new List<GridCell>();
		foreach (var cell in cells)
		{
			if (cell != null)
			{
				cellsList.Add(cell);
			}
		}
		return JsonUtility.ToJson(new SerializationWrapper<GridCell>(cellsList));
	}

	/// <summary>
	/// Deserialize the JSON string into a SerializationWrapper, then loop through all the elements
	/// in the SerializationWrapper.items and assign them back
	/// </summary>
	/// <param name="json">The JSON string to use for deserialization.</param>
	public void Deserialize(string json)
	{
		SerializationWrapper<GridCell> wrapper =
				JsonUtility.FromJson<SerializationWrapper<GridCell>>(json);
		foreach (GridCell cell in wrapper.items)
		{
			int x = (int)cell.pos.x / cellSize;
			int y = (int)cell.pos.z / cellSize;
			cells[x, y] = cell;
		}
	}

	public void SaveGridMap()
	{
		string json = Serialize();
		File.WriteAllText(saveFilePath, json);
		Debug.Log("GridMap: Saved on application quit");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="citizen"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public List<Vector3> FindPath(Vector3 startPos, Vector3 endPos)
	{
		try
		{
			Debug.Log("GridMap: FindPath() called");

			GridCell startCell = GetCellFromPosition(startPos);
			GridCell endCell = GetCellFromPosition(endPos);

			if (startCell == null || endCell == null)
			{
				Debug.LogError("GridMap: startCell or endCell is null");
				return new List<Vector3>();
			}

			Debug.Log($"{startCell.pos}: is road, {startCell.isRoad}, rotation {startCell.roadRotation}");

			Queue<GridCell> queue = new();
			HashSet<GridCell> visited = new();
			Dictionary<GridCell, GridCell> cameFrom = new();

			queue.Enqueue(startCell);
			visited.Add(startCell);

			while (queue.Count > 0)
			{
				GridCell current = queue.Dequeue();
				if (current == endCell)
				{
					return RetracePath(startCell, endCell, cameFrom, RoadDirection.None, startCell.roadRotation);
				}
				foreach (GridCell neighbor in GetNeighbors(current))
				{
					if (!visited.Contains(neighbor) && neighbor.isRoad)
					{
						queue.Enqueue(neighbor);
						visited.Add(neighbor);
						cameFrom[neighbor] = current;
					}
				}
			}
			Debug.LogWarning("GridMap: No path found");
			return new List<Vector3>();
		}
		catch (Exception ex)
		{
			Debug.LogError($"GridMap: Exception in FindPath - {ex.Message}\n{ex.StackTrace}");
			return new List<Vector3>();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="startCell"></param>
	/// <param name="endCell"></param>
	/// <param name="cameFrom"></param>
	/// <returns></returns>
	private List<Vector3> RetracePath(GridCell startCell, GridCell endCell, Dictionary<GridCell, GridCell> cameFrom, RoadDirection position, Quaternion rotation)
	{
		try
		{
			List<Vector3> path = new();
			GridCell currentCell = endCell;
			float[] offsets = { 2.5f, 3f, 3.5f };

			System.Random random = new System.Random();
			float offset = offsets[random.Next(offsets.Length)];// Randomly select an offset
			float sideMultiplier = random.Next(2) == 0 ? -1 : 1;// Randomly decide the side (left or right)
			Debug.Log($"GridMap:  retrace path rotation is {rotation}");
			// Vector3 sidewalkOffset = new(offset * sideMultiplier, 0, 0);
			Vector3 sidewalkOffset = new();

			if (rotation == Quaternion.Euler(0, 90, 0))
			{
				Debug.Log("GridMap: rotation is approximately 90");
				sidewalkOffset = new Vector3(0, 0, offset * sideMultiplier);
			}
			else
			{
				Debug.Log("Rotation is not 90");
				sidewalkOffset = new Vector3(offset * sideMultiplier, 0, 0);
			}

			while (currentCell != startCell)
			{
				path.Add(currentCell.pos + sidewalkOffset);
				currentCell = cameFrom[currentCell];
			}

			path.Add(startCell.pos + sidewalkOffset);
			path.Reverse();
			return path;


			// switch (position)
			// {
			// 	case RoadDirection.Top:
			// 		pathOffset = new Vector3(3, 0, 0);
			// 		break;
			// 	case RoadDirection.Bottom:
			// 		pathOffset = new Vector3(3, 0, 0);
			// 		break;
			// 	case RoadDirection.Left:
			// 		pathOffset = new Vector3(4, 0, 0);
			// 		break;
			// 	case RoadDirection.Right:
			// 		pathOffset = new Vector3(-4, 0, 0);
			// 		break;
			// }
		}
		catch (Exception ex)
		{
			Debug.LogError($"GridMap: Exception in RetracePath - {ex.Message}\n{ex.StackTrace}");
			return new List<Vector3>();
		}
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="cell"></param>
	/// <returns></returns>
	private List<GridCell> GetNeighbors(GridCell cell)
	{
		List<GridCell> neighbors = new();

		// Define the relative positions of the neighboring cells
		Vector3[] directions = {
				new Vector3(cellSize, 0, 0),  // Right
        new Vector3(-cellSize, 0, 0), // Left
        new Vector3(0, 0, cellSize),  // Up
        new Vector3(0, 0, -cellSize)  // Down
    };

		// Iterate over each direction to find the neighboring cells
		foreach (Vector3 direction in directions)
		{
			// Calculate the position of the neighboring cell
			Vector3 neighborPos = cell.pos + direction;

			// Check if the neighbor position is within the bounds of the grid
			if (IsWithinBounds(neighborPos))
			{
				// Convert the position to grid indices
				Vector3Int gridPos = Vector3Int.FloorToInt(neighborPos / cellSize);

				// Add the neighboring cell to the list
				neighbors.Add(cells[gridPos.x, gridPos.z]);
			}
		}

		return neighbors;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	private bool IsWithinBounds(Vector3 position)
	{
		return position.x >= 0 && position.x < cells.GetLength(0) && position.z >= 0 && position.z < cells.GetLength(1);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	private GridCell GetCellFromPosition(Vector3 position)
	{
		int x = Mathf.FloorToInt(position.x / cellSize);
		int z = Mathf.FloorToInt(position.z / cellSize);
		return cells[x, z];
	}

	private (GridCell, RoadDirection) GetAdjacentRoad(GridCell cell)
	{
		Vector3 cellTop = new(cell.pos.x, 0, cell.pos.z + 10);
		Vector3 cellBottom = new(cell.pos.x, 0, cell.pos.z - 10);
		Vector3 cellLeft = new(cell.pos.x - 10, 0, cell.pos.z);
		Vector3 cellRight = new(cell.pos.x + 10, 0, cell.pos.z);

		GridCell topCell = GetCellFromPosition(cellTop);
		GridCell bottomCell = GetCellFromPosition(cellBottom);
		GridCell leftCell = GetCellFromPosition(cellLeft);
		GridCell rightCell = GetCellFromPosition(cellRight);

		if (topCell != null && topCell.isRoad)
		{ return (topCell, RoadDirection.Top); }
		if (bottomCell != null && bottomCell.isRoad)
		{ return (bottomCell, RoadDirection.Bottom); }
		if (leftCell != null && leftCell.isRoad)
		{ return (leftCell, RoadDirection.Left); }
		if (rightCell != null && rightCell.isRoad)
		{ return (rightCell, RoadDirection.Right); }

		return (null, RoadDirection.None);
	}

	public Vector3 GetRandomRoadPos()
	{
		System.Random random = new();
		return roads[random.Next(roads.Count)];
	}
}


[System.Serializable]
public class SerializationWrapper<T>
{
	public List<T> items;

	public SerializationWrapper(List<T> items)
	{
		this.items = items;
	}
}