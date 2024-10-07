// using Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Zones;

[System.Serializable]
public class City
{
	// Collections
	public List<ResidentialZone> residentialZones = new();
	public List<CommercialZone> commercialZones = new();
	public List<IndustrialZone> industrialZones = new();
	public List<UtilityBuilding> utilityBuildings = new();
	public List<UtilityDistribution> utilityDistributions = new();
	public List<ServiceBuilding> serviceBuildings = new();
	public List<Road> roads = new();
	public List<RoadStructure> roadStructures = new();
	private readonly string saveFilePath = @"C:\Users\Ali\Documents\city.json";

	// Instances
	public Population population;
	public Economy economy;

	// Events
	public delegate void CreateZoneEventHandler(City sender, Zone createdZone);
	public event CreateZoneEventHandler OnCreateZone;
	public event Action<Vector3, UtilityBuilding> OnUtilityBuildingPlaced;
	public event Action<Vector3, Building> OnUtilityDistributionPlaced;
	public event Action<List<Vector3>, Quaternion> OnRoadPlaced;
	public event Action<Vector3, Building> OnRoadStructurePlaced;
	public event Action<Vector3, ServiceBuilding> OnServiceBuildingPlacement;
	public event Action<List<ZoneData>> OnReceiveZoneData;
	public event Action<ZoneData.Type, Vector3, Vector3> ZoneHighlighted;

	public event Action<UtilityType> OnUtilityDistHighlight;
	public event Action<ServiceType> OnServiceDistHighlight;

	public City(GridMap map)
	{
		Debug.Log("City: Constructor called");
		population = new(map);
		economy = new Economy();

		// Debug.Log($"City: residentialZones count = {residentialZones.Count}");
		// Debug.Log($"City: commercialZones count = {commercialZones.Count}");
		// Debug.Log($"City: industrialZones count = {industrialZones.Count}");
	}

	public void Initialize()
	{
		Debug.Log("City: Initialize() called");
		if (File.Exists(saveFilePath))
		{
			Debug.Log("City: city.json found");
			try
			{
				LoadCity();
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.Log("City: No save found, calling SaveCity()");
			// SaveCity();
		}
	}

	public void SubscribeToEvents()
	{
		TimeManager.Instance.OnHourPassed += HandleHourPassed;
		Debug.Log("City: SubscribeToEvents called");
	}


	public void UnsubscribeFromEvents()
	{
		TimeManager.Instance.OnHourPassed -= HandleHourPassed;
		Debug.Log("UnsubscribeFromEvents called");
	}


	private void HandleHourPassed()
	{
		Debug.Log("City: HandleHourPassed called");
		// SaveCity();
		Debug.Log($"City: Population is {population.citizens.Count}");
	}

	public void UpdateEconomy()
	{
		// Trigger calculations related to taxes and maintenance costs
		economy.CalculateResidentialTaxes(population.citizens);

		foreach (var zone in commercialZones)
		{
			economy.CalculateCommercialTaxes(zone.GetCommercialBuildings());
		}
		foreach (var zone in industrialZones)
		{
			economy.CalculateIndustrialTaxes(zone.GetIndustrialBuildings());
		}
		Debug.Log($"Utility Buildings Count: {utilityBuildings.Count}");
		Debug.Log($"Service Buildings Count: {serviceBuildings.Count}");
		economy.CalculateMaintenanceCost(utilityBuildings, serviceBuildings);

		// economy.CityTreasury();
	}


	public void CreateZone(ZoneData zoneData, GridMap gridMap)
	{
		Zone createdZone = null;

		switch (zoneData.type)
		{
			case ZoneData.Type.Residential:
				ResidentialZone residentialZone = new(zoneData, gridMap);
				residentialZones.Add(residentialZone);
				createdZone = residentialZone;
				Debug.Log($"City: zone created {zoneData.ToString()}");

				residentialZone.AddCitizenRequest += population.HandleAddCitizenRequest;
				break;

			case ZoneData.Type.Commercial:
				CommercialZone commercialZone = new(zoneData, gridMap);
				commercialZones.Add(commercialZone);
				createdZone = commercialZone;
				Debug.Log($"City: zone created {zoneData.ToString()}");
				break;

			case ZoneData.Type.Industrial:
				IndustrialZone industrialZone = new(zoneData, gridMap);
				industrialZones.Add(industrialZone);
				createdZone = industrialZone;
				Debug.Log($"City: zone created {zoneData.ToString()}");
				break;
		}

		if (createdZone != null)
		{
			Debug.Log("City: createdZone is not null");
		}

		// check event is not null, used by GridMap
		if (OnCreateZone != null && createdZone != null)
		{
			OnCreateZone(this, createdZone);
			Debug.Log("City: OnCreateZone event raised");
		}

		createdZone.Initialize();
		UpdateAdjacentZonesForResidentialZones(); // updates every time a new zone is added
	}

	/// <summary>
	/// Call GetAdjacentZones<T>() passing CommercialZone as T.
	/// </summary>
	/// <param name="zone">The zone for which adjacent zones needs to be found.</param>
	/// <returns>List of adjacent CommercialZone.</returns>
	public List<CommercialZone> GetAdjacentCommercialZones(ResidentialZone zone)
	{
		return GetAdjacentZones<CommercialZone>(zone);
	}

	/// <summary>
	/// Call GetAdjacentZone<T>() passing IndustrialZone as T.
	/// </summary>
	/// <param name="zone">The zone for which adjacent zones needs to be found.</param>
	/// <returns>List of adjacent CommercialZone.</returns>
	public List<IndustrialZone> GetAdjacentIndustrialZones(ResidentialZone zone)
	{
		return GetAdjacentZones<IndustrialZone>(zone);
	}

	/// <summary>
	/// Generic method that takes either CommercialZone or IndustrialZone as type T.
	/// Finds adjacent zones belonging to type T.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="zone"></param>
	/// <returns></returns>
	public List<T> GetAdjacentZones<T>(ResidentialZone zone) where T : Zone
	{
		List<T> adjacentZones = new();

		int startX = (int)zone.data.startPos.x;
		int endX = (int)zone.data.endPos.x;
		int startZ = (int)zone.data.startPos.z;
		int endZ = (int)zone.data.endPos.z;

		// Applicable adjacent positions
		List<Vector3> leftPositions = new();
		List<Vector3> bottomPositions = new();
		List<Vector3> rightPositions = new();
		List<Vector3> topPositions = new();

		// Fill the lists with possible adjacent positions
		for (int z = startZ; z <= endZ; z += 10)
		{
			leftPositions.Add(new Vector3(startX - 10, 0, z));
			leftPositions.Add(new Vector3(startX - 20, 0, z));
		}

		for (int x = startX; x <= endX; x += 10)
		{
			bottomPositions.Add(new Vector3(x, 0, startZ - 10));
			bottomPositions.Add(new Vector3(x, 0, startZ - 20));
		}

		for (int z = startZ; z <= endZ; z += 10)
		{
			rightPositions.Add(new Vector3(endX + 10, 0, z));
			rightPositions.Add(new Vector3(endX + 20, 0, z));
		}

		for (int x = startX; x <= endX; x += 10)
		{
			topPositions.Add(new Vector3(x, 0, endZ + 10));
			topPositions.Add(new Vector3(x, 0, endZ + 20));
		}

		// Check for matching zones
		if (typeof(T) == typeof(CommercialZone))
		{
			foreach (var otherZone in commercialZones)
			{
				if (leftPositions.Contains(otherZone.data.startPos) || bottomPositions.Contains(otherZone.data.startPos) || rightPositions.Contains(otherZone.data.startPos) || topPositions.Contains(otherZone.data.startPos))
				{
					adjacentZones.Add(otherZone as T);
				}
			}
		}

		if (typeof(T) == typeof(IndustrialZone))
		{
			foreach (var otherZone in industrialZones)
			{
				if (leftPositions.Contains(otherZone.data.startPos) || bottomPositions.Contains(otherZone.data.startPos) || rightPositions.Contains(otherZone.data.startPos) || topPositions.Contains(otherZone.data.startPos))
				{
					adjacentZones.Add(otherZone as T);
				}
			}
		}

		return adjacentZones;
	}

	private void UpdateAdjacentZonesForResidentialZones()
	{
		foreach (var residentialZone in residentialZones)
		{
			residentialZone.adjacentCommercialZones = GetAdjacentCommercialZones(residentialZone);
			residentialZone.adjacentIndustrialZones = GetAdjacentIndustrialZones(residentialZone);
		}
	}

	public void SaveCity()
	{
		string json = "";
		Debug.Log("City: SaveCity called");
		Debug.Log($"City: roads.Count before saving is {roads.Count}");
		Debug.Log($"City: residentialZones.Count before saving is {residentialZones.Count}");
		try
		{
			json = JsonUtility.ToJson(this);
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
		File.WriteAllText(saveFilePath, json);
	}

	public void LoadCity()
	{
		Debug.Log("City: called LoadCity()");
		string json = File.ReadAllText(saveFilePath);
		City loadedCity = JsonUtility.FromJson<City>(json);

		Debug.Log($"LoadedCity: residentialZones.count: {loadedCity.residentialZones.Count}");
		Debug.Log($"City: loadedCity commercialZones.count: {loadedCity.commercialZones.Count}");
		Debug.Log($"City: loadedCity.roads is {loadedCity.roads.Count} ");
		Debug.Log($"City: loadedCity.utilityBuilding.Count is {loadedCity.utilityBuildings.Count}");

		this.residentialZones = loadedCity.residentialZones;
		this.commercialZones = loadedCity.commercialZones;
		this.industrialZones = loadedCity.industrialZones;
		this.utilityBuildings = loadedCity.utilityBuildings;
		this.utilityDistributions = loadedCity.utilityDistributions;
		this.serviceBuildings = loadedCity.serviceBuildings;
		this.roads = loadedCity.roads;
		this.roadStructures = loadedCity.roadStructures;
		this.population = loadedCity.population;
		this.economy = loadedCity.economy;

		if (TimeManager.Instance != null)
			TimeManager.Instance.On10MinutePassed += population.SimulatePedestrianTraffic;

		foreach (ResidentialZone zone in residentialZones)
		{
			zone.SubscribeEvents();
			foreach (ResidentialBuilding building in zone.buildings)
			{
				InfrastructureManager.InstantiateBuilding<ResidentialBuilding>(building);
			}
		}

		foreach (CommercialZone zone in commercialZones)
		{
			foreach (CommercialBuilding building in zone.buildings)
			{
				InfrastructureManager.InstantiateBuilding<CommercialBuilding>(building);
			}
		}

		foreach (IndustrialZone zone in industrialZones)
		{
			foreach (IndustrialBuilding building in zone.buildings)
			{
				InfrastructureManager.InstantiateBuilding<IndustrialBuilding>(building);
			}
		}

		foreach (UtilityBuilding building in utilityBuildings)
		{
			InfrastructureManager.InstantiateBuilding<UtilityBuilding>(building);
		}

		foreach (UtilityDistribution building in utilityDistributions)
		{
			InfrastructureManager.InstantiateBuilding<UtilityDistribution>(building);
		}

		foreach (ServiceBuilding building in serviceBuildings)
		{
			InfrastructureManager.InstantiateServiceBuilding(building, building.pos);
		}

		foreach (RoadStructure structure in roadStructures)
		{
			InfrastructureManager.InstantiateRoad_Structure(structure.roadStructureType, structure.pos, structure.rotation);
		}

		foreach (Road road in roads)
		{
			Debug.Log(road);
			InfrastructureManager.InstantiateRoad_Straight(road.GetCells(), road.rotation);
		}
		Debug.Log("City: LoadCity completed");
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="level"></param>
	/// <param name="pos"></param>
	public void HandleUtilityPlacement(UtilityType type, byte level, Vector3 pos)
	{
		Debug.Log("City: called HandleUtilityPlacement()");

		UtilityBuilding utilityBuilding = new(type, level, pos);
		utilityBuildings.Add(utilityBuilding);

		OnUtilityBuildingPlaced?.Invoke(pos, utilityBuilding);
		utilityBuilding.prefabId = InfrastructureManager.InstantiateUtilityBuilding(utilityBuilding, pos);

		Debug.Log($"City: Placed utility {utilityBuilding.prefabId}");
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="building"></param>
	/// <param name="pos"></param>
	public void HandleUtilityDistributionPlaced(UtilityType type, Vector3 pos)
	{
		UtilityBuilding utilityBuilding = FindUtilityBuildingForDistribution(type);
		if (utilityBuilding != null)
		{
			UtilityDistribution utilityDistribution = new(utilityBuilding, pos);
			utilityDistributions.Add(utilityDistribution);

			OnUtilityDistributionPlaced?.Invoke(pos, utilityDistribution);
			utilityDistribution.prefabId = InfrastructureManager.InstantiateUtilityDistribution(utilityDistribution, pos);
			Debug.Log($"City: UtilityDistribution named {utilityDistribution.prefabId} placed successfully");
		}
		else
		{
			Debug.Log("City: Cannot place UtilityDistribution");
		}
	}

	public UtilityBuilding FindUtilityBuildingForDistribution(UtilityType type)
	{
		foreach (var building in utilityBuildings)
		{
			if (building.utilityType == type && building.HasCapacity(400))
			{
				return building;
			}
		}
		return null;
	}

	public void HandleServicePlacement(ServiceType type, Vector3 pos)
	{
		ServiceBuilding serviceBuilding = new(type, pos);
		serviceBuildings.Add(serviceBuilding);
		OnServiceBuildingPlacement?.Invoke(pos, serviceBuilding);
		InfrastructureManager.InstantiateServiceBuilding(serviceBuilding, pos);

		Debug.Log($"City: ServiceBuilding of type {type} placed successfully at {pos}");
		Debug.Log($"City: serviceBuildings count: {serviceBuildings.Count}");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	public void HandleRoadPlaced(Vector3 startPos, Vector3 endPos)
	{
		Debug.Log($"City: HandleRoadPlaced() called");
		Road road = new(startPos, endPos);
		var positions = road.GetCells();
		roads.Add(road);
		Debug.Log($"City: road.GetCells count: {road.GetCells().Count}");

		OnRoadPlaced?.Invoke(positions, road.rotation);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="pos"></param>
	public void HandleRoadStructurePlaced(RoadStructureType type, Vector3 pos, Quaternion rotation)
	{
		Debug.Log($"City: HandleRoadStructurePlaced() with {pos}");
		RoadStructure roadStructure = new(type, pos, rotation);
		roadStructures.Add(roadStructure);

		switch (type)
		{
			case RoadStructureType.Curve:
				InfrastructureManager.InstantiateRoad_Structure(RoadStructureType.Curve, pos, rotation);
				break;
			case RoadStructureType.IntersectionT:
				InfrastructureManager.InstantiateRoad_Structure(RoadStructureType.IntersectionT, pos, rotation);
				break;
			case RoadStructureType.IntersectionCross:
				InfrastructureManager.InstantiateRoad_Structure(RoadStructureType.IntersectionCross, pos, rotation);
				break;
		}
		OnRoadStructurePlaced?.Invoke(pos, roadStructure);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public List<ZoneData> GetZoneDataByType(ZoneData.Type type)
	{
		List<ZoneData> zoneDataList = new List<ZoneData>();

		foreach (var zone in residentialZones)
		{
			if (zone != null && zone.data.type == type)
			{
				var zoneData = zone.GetZoneData();
				Debug.Log($"Found zone data: {zoneData}");
				zoneDataList.Add(zoneData);
			}
		}


		foreach (var zone in commercialZones)
		{
			if (zone != null && zone.data.type == type)
			{
				var zoneData = zone.GetZoneData();
				Debug.Log($"Found zone data: {zoneData}");
				zoneDataList.Add(zoneData);

			}
		}

		foreach (var zone in industrialZones)
		{
			if (zone != null && zone.data.type == type)
			{
				var zoneData = zone.GetZoneData();
				Debug.Log($"Found zone data: {zoneData}");
				zoneDataList.Add(zoneData);
			}
		}

		return zoneDataList;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	public void HandleRequestZoneData(ZoneData.Type type)
	{
		Debug.Log("Handle Zone data called");
		List<ZoneData> zoneDataList = GetZoneDataByType(type);

		if (zoneDataList.Count > 0)
		{
			// Trigger event with the list of zone data
			OnReceiveZoneData?.Invoke(zoneDataList);
		}
		else
		{
			Debug.LogWarning($"No Zone data available for type {type}.");
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="startPos"></param>
	/// <param name="endPos"></param>
	public void HandleHighlightZone(ZoneData.Type type, Vector3 startPos, Vector3 endPos)
	{
		Debug.Log($"City: Handling highlight for {type} zone from {startPos} to {endPos}");
		switch (type)
		{
			case ZoneData.Type.Residential:
				ZoneHighlighted?.Invoke(ZoneData.Type.Residential, startPos, endPos);
				InfrastructureManager.InstantiateZoneHighlightPlane(ZoneData.Type.Residential, startPos, endPos);

				break;
			case ZoneData.Type.Commercial:
				ZoneHighlighted?.Invoke(ZoneData.Type.Commercial, startPos, endPos);
				InfrastructureManager.InstantiateZoneHighlightPlane(ZoneData.Type.Commercial, startPos, endPos);
				break;
			case ZoneData.Type.Industrial:
				ZoneHighlighted?.Invoke(ZoneData.Type.Industrial, startPos, endPos);
				InfrastructureManager.InstantiateZoneHighlightPlane(ZoneData.Type.Industrial, startPos, endPos);
				break;
		}
	}

	public void HighlightUtilityDistributions(UtilityType type)
	{
		Debug.Log($"HighlightUtilityDistributions called for {type}");

		// List to track positions of highlighted utility distributions
		List<Vector3> highlightPositions = new List<Vector3>();

		// Iterate over utility buildings and instantiate highlights for the specific type
		foreach (var building in utilityDistributions)
		{
			if (building.utilityType == type) // Only highlight if it matches the passed type
			{
				Vector3 position = building.pos; // Access the position of the building
				InfrastructureManager.InstantiateUtilityDistributionHighlight(building, position);

				// Add the position to the highlight list
				highlightPositions.Add(position);
			}
		}

		// Log the result of the highlighting operation
		if (highlightPositions.Count > 0)
		{
			Debug.Log($"City: Highlighted {highlightPositions.Count} {type} utility distributions.");
		}
		else
		{
			Debug.LogWarning($"City: No {type} utility distributions found to highlight.");
		}

		// Trigger the event to notify DashboardManager
		OnUtilityDistHighlight?.Invoke(type);
	}

	public void HighlightServiceDistributions(ServiceType type)
	{
		Debug.Log("HighlightServiceDistributions called");
		// List to track positions of highlighted service distributions
		List<Vector3> highlightPositions = new List<Vector3>();
		ServiceType? firstType = null; // Nullable type to store the type of the first building

		// Iterate over service buildings and instantiate highlights
		foreach (var building in serviceBuildings) // Assume you have a list of service buildings
		{
			if (building.serviceType == type) // Check if the type matches
			{
				Vector3 position = building.pos; // Access the position of the building
				InfrastructureManager.InstantiateServiceDistributionHighlight(building, position); // You'll implement this method next

				// Add the position to the highlight list
				highlightPositions.Add(position);

				// Store the type of the first building
				if (firstType == null)
				{
					firstType = building.serviceType;
				}
			}
		}

		// Log the result of the highlighting operation
		if (highlightPositions.Count > 0)
		{
			Debug.Log($"City: Highlighted {highlightPositions.Count} {firstType} service distributions.");
		}
		else
		{
			Debug.LogWarning("City: No service distributions found to highlight.");
		}
		OnServiceDistHighlight?.Invoke(type);
	}


}