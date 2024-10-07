using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zones
{
	[System.Serializable]
	public class ResidentialZone : Zone
	{
		// Fields
		public List<ResidentialBuilding> buildings = new();
		public List<Citizen> citizensInZone;
		public List<CommercialZone> adjacentCommercialZones;
		public List<IndustrialZone> adjacentIndustrialZones;


		// Events
		public event Action<ResidentialZone> AddCitizenRequest;


		/// <summary>
		/// Constructor for ResidentialZone.
		/// </summary>
		/// <param name="zoneData"></param>
		/// <param name="gridMap"></param>
		public ResidentialZone(ZoneData zoneData, GridMap gridMap) : base(zoneData, gridMap)
		{
			if (TimeManager.Instance == null)
			{
				Debug.Log("ResidentialZone: TimeManager is null");
				return;
			}
			SubscribeEvents();
			Debug.Log("ResidentialZone: Constructor called");
		}

		public override void Initialize()
		{
			base.Initialize();
			zqi = new(this, cellsInZone.Count);

		}

		public void SubscribeEvents()
		{
			if (TimeManager.Instance == null)
			{
				Debug.LogError("ResidentialZone: TimeManager instance is null");
				return;
			}
			try
			{
				TimeManager.Instance.OnHourPassed += HandleHourPassed;
				Debug.Log("ResidentialZone: Subscribed to OnHourPassed");
				TimeManager.Instance.OnMinutePassed += HandleMinutePassed;
				Debug.Log("ResidentialZone: Subscribed to OnMinutePassed");
				TimeManager.Instance.OnDayPassed += HandleDayPassed;
				Debug.Log("ResidentialZone: Subscribed to OnDayPassed");
				TimeManager.Instance.OnMonthPassed += HandleMonthPassed;
				Debug.Log("ResidentialZone: Subscribed to OnMonthPassed");
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		public ZoneData GetZoneData()
		{
			return data;
		}

		private void HandleMinutePassed()
		{
			Debug.Log("ResidentialZone: HandleMinutePassed called");
			// UpdateZqi();
			// try
			// {
			// Debug.Log($"ZQI of zone is {zqi.GetZqiResidential()}");
			// Debug.Log($"Employment rating is {zqi.EmploymentRating}");
			// }
			// catch (Exception e)
			// {
			// 	Debug.LogError(e.Message);
			// }
			SpawnBuilding();
			AddCitizenRequest?.Invoke(this);
		}

		private void HandleHourPassed()
		{
			Debug.Log("ResidentialZone: HandleHourPassed() called");
			UpgradeBuilding();
		}

		private void HandleDayPassed()
		{
		}

		public void HandleMonthPassed()
		{
		}

		/// <summary>
		/// Loops through all the buildings in zone and adds the citizens from those buildings into
		/// citizensInZone.
		/// </summary>
		/// <returns></returns>
		public List<Citizen> GetCitizensInZone()
		{
			citizensInZone.Clear();

			foreach (var building in buildings)
			{
				citizensInZone.AddRange(building.residents);
			}

			return citizensInZone;
		}

		public override void SpawnBuilding()
		{
			Debug.Log("ResidentialZone: SpawnBuilding() called");
			// if (zqi.powerCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log($"ResidentialZone: No Power, powerCoverage: {zqi.powerCoverage}");
			// 	return;
			// }

			// if (zqi.waterCoverage < cellsInZone.Count / 2)
			// {
			// 	Debug.Log("ResidentialZone: No Water");
			// 	return;
			// }

			if (cellsInZone == null | cellsInZone.Count == 0)
			{
				Debug.LogWarning("ResidentialZone: No cells available in zone, called GetCellsInZone");
				GetCellsInZone();
				return;
			}

			System.Random random = new();
			GridCell randomCell = cellsInZone[random.Next(cellsInZone.Count)];

			if (randomCell.isOccupied)
			{
				// Debug.Log("ResidentialZone: randomCell is occupied");
				return;
			}

			var (hasRoad, rotation) = HasRoad(randomCell);
			if (!hasRoad)
			{
				// Debug.Log("ResidentialZone: randomCell has no road connectivity");
				return;
			}


			ResidentialBuilding building = new(randomCell.pos, rotation);
			building.prefabId = InfrastructureManager.InstantiateResidentialBuilding(building, randomCell.pos, rotation);

			randomCell.isOccupied = true;
			randomCell.building = building;
			randomCell.buildingId = building.id;

			buildings.Add(building);
			Debug.Log($"ResidentialZone: buildings.Count: {buildings.Count}");
		}

		/// <summary>
		/// Updates the building levels once the zone crosses certain ZQI thresholds.
		/// </summary>
		public void UpgradeBuilding()
		{

			Debug.Log("ResidentialZone: UpgradeBuilding() called");
			System.Random random = new();
			var building = buildings[random.Next(buildings.Count)];

			float zqiValue = zqi.GetZqiResidential();
			// Debug.Log($"UpgradeBuilding: zqi is {zqiValue}");
			// Debug.Log($"UpgradeBuilding: building.level is {building.level}");

			switch (zqiValue)
			{
				case var value when value >= 20f && value < 40f && building.level < 1:
					building.level = 1;
					building.Upgrade();
					building.prefabId = InfrastructureManager.InstantiateResidentialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;

				case var value when value >= 40f && value < 50f && building.level < 2:
					building.level = 2;
					building.Upgrade();
					building.prefabId = InfrastructureManager.InstantiateResidentialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;

				case var value when value >= 50f && value < 80f && building.level < 3:
					building.level = 3;
					building.Upgrade();
					building.prefabId = InfrastructureManager.InstantiateResidentialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;

				case var value when value >= 60f && value < 100f && building.level < 4:
					building.level = 4;
					building.Upgrade();
					building.prefabId = InfrastructureManager.InstantiateResidentialBuilding_Upgrade(building);
					Debug.Log("UpgradeBuilding: InstantiateResidentialBuilding_Upgrade() called");
					break;
				default:
					Debug.Log("ResidentialZone: UpgradeBuilding() switch default");
					break;
			}
		}

		public override string ToString()
		{
			return $", WaterRating: {zqi.WaterRating}, LiteracyRating: {zqi.LiteracyRating}, EmploymentRating:";
		}
	}
}