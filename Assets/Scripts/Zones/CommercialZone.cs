using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zones
{
	[System.Serializable]
	public class CommercialZone : Zone
	{
		public List<CommercialBuilding> buildings = new();

		/// <summary>
		/// Constructor for CommercialZone
		/// </summary>
		/// <param name="zoneData"></param>
		/// <param name="gridMap"></param>
		public CommercialZone(ZoneData zoneData, GridMap gridMap) : base(zoneData, gridMap)
		{
			if (TimeManager.Instance == null)
			{
				Debug.Log("CommercialZone: TimeManager is null");
				return;
			}

			TimeManager.Instance.OnMinutePassed += HandleMinutePassed;
			TimeManager.Instance.OnHourPassed += HandleHourPassed;
			TimeManager.Instance.OnDayPassed += HandleDayPassed;
			Debug.Log("CommercialZone: Constructor called");
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

		public List<CommercialBuilding> GetCommercialBuildings()
		{
			return buildings;
		}
		private void HandleMinutePassed()
		{
			UpdateZqi();
			Debug.Log($"ZQI of zone is {zqi.GetZqiCommercial()}");
			SpawnBuilding();
		}

		private void HandleHourPassed()
		{
			Debug.Log("CommercialZone: HandleHourPassed() called");
			UpgradeBuilding();
		}


		private void HandleDayPassed()
		{
		}

		public override void SpawnBuilding()
		{
			// if (zqi.powerCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log($"CommercialZone: No Power, powerCoverage: {zqi.powerCoverage}");
			// 	return;
			// }

			// if (zqi.waterCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log("CommercialZone: No Water");
			// 	return;
			// }


			if (cellsInZone == null | cellsInZone.Count == 0)
			{
				Debug.LogWarning("CommercialZone: No cells available in zone, called GetCellsInZone");
				GetCellsInZone();
				return;
			}

			System.Random random = new();
			GridCell randomCell = cellsInZone[random.Next(cellsInZone.Count)];

			if (randomCell.isOccupied)
			{ return; }

			var (hasRoad, rotation) = HasRoad(randomCell);
			if (!hasRoad)
			{
				// Debug.Log("CommercialZone: randomCell has no road");
				return;
			}

			CommercialBuilding building = new(randomCell.pos, rotation);
			building.prefabId = InfrastructureManager.InstantiateCommercialBuilding(building, randomCell.pos, rotation);

			randomCell.isOccupied = true;
			randomCell.building = building;
			randomCell.buildingId = building.id;
			buildings.Add(building);

			Debug.Log("CommercialZone: InstantiateCommercialBuilding called");
		}

		public void UpgradeBuilding()
		{

			Debug.Log("CommercialZone: UpgradeBuilding() called");
			System.Random random = new();
			var building = buildings[random.Next(buildings.Count)];

			float zqiValue = zqi.GetZqiCommercial();
			Debug.Log($"CommercialZone: UpgradeBuilding zqi is {zqiValue}");
			// Debug.Log($"UpgradeBuilding: building.level is {building.level}");

			switch (zqiValue)
			{
				case var value when value >= 20f && value < 100f && building.level < 1:
					building.level = 1;
					Debug.Log($"CommercialZone: building.level: {building.level}");
					building.Upgrade();
					InfrastructureManager.InstantiateCommercialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateCommercialBuilding_Upgrade() called");
					break;

				case var value when value >= 40f && value < 100f && building.level < 2:
					building.level = 2;
					building.Upgrade();
					InfrastructureManager.InstantiateCommercialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;

				case var value when value >= 50f && value < 100f && building.level < 3:
					building.level = 3;
					building.Upgrade();
					InfrastructureManager.InstantiateCommercialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;
				// case var value when value >= 8f && value < 10f && building.level < 4:
				// 	building.level = 4;
				// 	building.Upgrade();
				// 	InfrastructureManager.InstantiateCommercialBuilding_Upgrade(building);
				// 	Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
				// 	break;
				default:
					Debug.Log("CommercialZone: UpgradeBuilding() switch default");
					break;
			}
		}
	}
}
