using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zones
{
	[System.Serializable]
	public class IndustrialZone : Zone
	{
		public List<IndustrialBuilding> buildings = new();

		/// <summary>
		/// Constructor for IndustrialZone
		/// </summary>
		/// <param name="zoneData"></param>
		/// <param name="gridMap"></param>
		public IndustrialZone(ZoneData zoneData, GridMap gridMap) : base(zoneData, gridMap)
		{
			if (TimeManager.Instance == null)
			{
				Debug.Log("IndustrialZone: TimeManager is null");
				return;
			}

			TimeManager.Instance.OnMinutePassed += HandleMinutePassed;
			TimeManager.Instance.OnHourPassed += HandleHourPassed;
			TimeManager.Instance.OnDayPassed += HandleDayPassed;
			Debug.Log("IndustrialZone: Constructor called");
		}

		public override void Initialize()
		{
			base.Initialize();
			zqi = new(this, cellsInZone.Count);
		}

		public ZoneData GetZoneData()
		{
			return data;
		}

		public List<IndustrialBuilding> GetIndustrialBuildings()
		{
			return buildings;
		}
		private void HandleMinutePassed()
		{
			UpdateZqi();
			Debug.Log($"ZQI of zone is {zqi.GetZqiIndustrial()}");
			SpawnBuilding();
		}

		private void HandleHourPassed()
		{
			UpgradeBuilding();
		}

		private void HandleDayPassed()
		{
		}

		public override void SpawnBuilding()
		{

			// if (zqi.powerCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log($"IndustrialZone: No Power, powerCoverage: {zqi.powerCoverage}");
			// 	return;
			// }

			// if (zqi.waterCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log("IndustrialZone: No Water");
			// 	return;
			// }

			if (cellsInZone == null | cellsInZone.Count == 0)
			{
				Debug.LogWarning("IndustrialZone: No cells available in zone, called GetCellsInZone");
				GetCellsInZone();
				return;
			}

			System.Random random = new();
			GridCell randomCell = cellsInZone[random.Next(cellsInZone.Count)];

			if (randomCell.isOccupied)
				return;

			var (hasRoad, rotation) = HasRoad(randomCell);
			if (!hasRoad)
			{
				Debug.Log("IndustrialZone: randomCell has no road");
				return;
			}


			IndustrialBuilding building = new(randomCell.pos, rotation);
			building.prefabId = InfrastructureManager.InstantiateIndustrialBuilding(building);

			randomCell.isOccupied = true;
			randomCell.building = building;
			randomCell.buildingId = building.id;

			buildings.Add(building);
		}


		/// <summary>
		/// 
		/// </summary>
		public void UpgradeBuilding()
		{

			Debug.Log("IndustrialZone: UpgradeBuilding() called");
			System.Random random = new();
			var building = buildings[random.Next(buildings.Count)];

			float zqiValue = zqi.GetZqiIndustrial();

			switch (zqiValue)
			{
				case var value when value >= 20f && value < 100f && building.level < 1:
					building.level = 1;
					building.Upgrade();
					InfrastructureManager.InstantiateIndustrialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateIndustrialBuilding_Upgrade called");
					break;

				case var value when value >= 40f && value < 100f && building.level < 2:
					building.level = 2;
					building.Upgrade();
					InfrastructureManager.InstantiateIndustrialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateIndustrialBuilding_Upgrade called");
					break;

				case var value when value >= 60f && value < 100f && building.level < 3:
					building.level = 3;
					building.Upgrade();
					InfrastructureManager.InstantiateIndustrialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateIndustrialBuilding_Upgrade called");
					break;

				default:
					Debug.Log("IndustrialZone: UpgradeBuilding() switch default");
					break;
			}
		}
	}
}
