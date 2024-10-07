using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a zone in the city.
/// </summary>
[System.Serializable]
public abstract class Zone
{
	// fields
	public string zoneId;
	public ZoneData data;
	private GridMap gridMap; // reference to the GridMap instance in GameManager
	[NonSerialized] public List<GridCell> cellsInZone = new();
	public Zqi zqi;


	/// <summary>
	/// Constructor for Zone.
	/// </summary>
	/// <param name="zoneData">ZoneData type which has contains data needed to create a zone.</param>
	/// <param name="gridMap">Reference to GridMap instance of GameManager</param>
	public Zone(ZoneData zoneData, GridMap gridMap)
	{
		this.zoneId = Guid.NewGuid().ToString();
		this.data = zoneData;
		this.gridMap = gridMap;
		Debug.Log($"Zone: Constructor called, zoneId{zoneId}");
	}

	public virtual void Initialize()
	{
		GetCellsInZone();
	}

	/// <summary>
	/// Updates Zqi.powerCoverage
	/// </summary>
	/// <returns></returns>
	public void UpdatePowerCoverage()
	{
		int count = 0;

		foreach (var cell in cellsInZone)
		{
			if (cell.hasPower)
			{
				count++;
			}
		}
		// Debug.Log($"Zone: powerCoverage is {count} cells");
		zqi.powerCoverage = count;
	}

	/// <summary>
	/// Updates Zqi.waterCoverage
	/// </summary>
	/// <returns></returns>
	public void UpdateWaterCoverage()
	{
		int count = 0;

		foreach (var cell in cellsInZone)
		{
			if (cell.hasWater)
			{
				count++;
			}
		}
		// Debug.Log($"Zone: waterCoverage is {count} cells");
		zqi.waterCoverage = count;
	}

	public void UpdateSchoolCoverage()
	{
		int count = 0;

		foreach (var cell in cellsInZone)
		{
			if (cell.hasSchool)
			{
				count++;
			}
		}
		// Debug.Log($"Zone: school coverage is {count} cells");
		zqi.schoolCoverage = count;
	}

	public void UpdateHospitalCoverage()
	{
		int count = 0;

		foreach (var cell in cellsInZone)
		{
			if (cell.hasHospital)
			{
				count++;
			}
		}
		// Debug.Log($"Zone: hospital coverage is {count} cells");
		zqi.hospitalCoverage = count;
	}

	public void UpgradeZqi()
	{
		int policeCount = 0;
		int fireCount = 0;


		foreach (var cell in cellsInZone)
		{
			if (cell.hasPoliceStation)
			{
				policeCount++;
			}
			if (cell.hasFireStation)
			{
				fireCount++;
			}
		}
		// Debug.Log($"Zone: police coverage is {policeCount} and fire coverage is {fireCount} cells");
		zqi.policeCoverage = policeCount;
		zqi.fireCoverage = fireCount;
	}

	public virtual void UpdateZqi()
	{
		UpdatePowerCoverage();
		UpdateWaterCoverage();
		UpdateSchoolCoverage();
		UpdateHospitalCoverage();
		UpgradeZqi();
	}

	/// <summary>
	/// Checks if the cell has any road connectivity.
	/// Loops through all cellsInZone to find a match for any position(top, bottom, left, right),
	/// and if there is a match checks if the match is a road.
	/// </summary>
	/// <param name="cell">The cell for which checks need to performed</param>
	/// <returns>Return true if any adjacent cell has GridCell.isRoad true</returns>
	public (bool, Quaternion) HasRoad(GridCell cell)
	{
		Vector3 cellTop = new(cell.pos.x, 0, cell.pos.z + 10);
		Vector3 cellBottom = new(cell.pos.x, 0, cell.pos.z - 10);
		Vector3 cellLeft = new(cell.pos.x - 10, 0, cell.pos.z);
		Vector3 cellRight = new(cell.pos.x + 10, 0, cell.pos.z);

		foreach (GridCell zoneCell in cellsInZone)
		{
			if (zoneCell.pos == cellTop && zoneCell.isRoad)
				return (true, Quaternion.Euler(0, 0, 0));
			if (zoneCell.pos == cellBottom && zoneCell.isRoad)
				return (true, Quaternion.Euler(0, 180, 0));
			if (zoneCell.pos == cellLeft && zoneCell.isRoad)
				return (true, Quaternion.Euler(0, -90, 0));
			if (zoneCell.pos == cellRight && zoneCell.isRoad)
				return (true, Quaternion.Euler(0, 90, 0));
		}

		return (false, Quaternion.identity);
	}

	/// <summary>
	/// Uses the GridMap.cells and checks for any cell instances that belong to
	/// "this" zone. Adds the matching cells to cellsInZone.
	/// Also returns the number of cells added to zone.
	/// </summary>
	/// <returns></returns>
	public int GetCellsInZone()
	{
		Debug.Log("Zone: GetCellsInZone called");
		cellsInZone.Clear();

		foreach (GridCell cell in gridMap.cells)
		{
			if (cell.zoneId == zoneId)
			{
				cellsInZone.Add(cell);
			}
		}
		Debug.Log($"Zone: There are {cellsInZone.Count} cells in zone");
		return cellsInZone.Count;
	}

	// public List<Citizen> GetCitizensInZone()
	// {
	// 	foreach (var building in )
	// }


	/// <summary>
	/// Abstract method that needs to be implemented by child classes.
	/// </summary>
	public abstract void SpawnBuilding();


	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return data.ToString();
	}
}