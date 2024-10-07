using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Used to instantiate buildings of ResidentialZone, CommercialZone and IndustrialZone.
/// Also used to instantiate UtilityBuildings and more in future.
/// </summary>
public class InfrastructureManager : MonoBehaviour
{
	public static List<GameObject> instantiatedPlanes = new List<GameObject>();
	public static List<GameObject> instantiatedPlanesUtilityDist = new List<GameObject>();
	public static List<GameObject> instantiatedPlanesServiceDist = new List<GameObject>();

	// made static so that static methods can access it
	private static ConcurrentDictionary<Vector3, GameObject> instantiatedBuildings = new();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="building"></param>
	/// <param name="pos"></param>
	/// <param name="rotation"></param>
	/// <returns></returns>
	public static string InstantiateResidentialBuilding(ResidentialBuilding building, Vector3 pos, Quaternion rotation)
	{
		Debug.Log($"InfrastructureManager: instantiating at {pos}");
		System.Random random = new();
		GameObject prefab = null;
		string prefabId = string.Empty;
		int randomIndex = 0;

		switch (building.level)
		{
			case 0:
				randomIndex = random.Next(4);
				prefab = Utility.residentialLevelZeroPrefabs[randomIndex];
				prefabId = "res_0" + randomIndex;
				// Debug.Log(prefabId);
				break;

			case 1:
				randomIndex = random.Next(5);
				prefab = Utility.residentialLevelOnePrefabs[randomIndex];
				prefabId = "res_1" + randomIndex;
				break;

			case 2:
				randomIndex = random.Next(2);
				prefab = Utility.residentialLevelTwoPrefabs[randomIndex];
				prefabId = "res_2" + randomIndex;
				break;

			case 3:
				randomIndex = random.Next(4);
				prefab = Utility.residentialLevelThreePrefabs[randomIndex];
				prefabId = "res_3" + randomIndex;
				break;

			case 4:
				randomIndex = random.Next(4);
				prefab = Utility.residentialLevelFourPrefabs[randomIndex];
				prefabId = "res_4" + randomIndex;
				break;

			default:
				Debug.LogError("InfrastructureManager: residential switch failed to enter any case");
				break;
		}

		if (prefab != null)
		{
			try
			{
				// GameObject instantiatedObject = null;
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject instantiatedObject = Instantiate(prefab, pos, rotation);
					bool success = instantiatedBuildings.TryAdd(pos, instantiatedObject);
					// Debug.Log($"InfrastructureManager: success: {success}, add key: {pos} and value: {instantiatedObject}");
				});
				return prefabId;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: residential prefab is null, instantiation aborted");
			return prefabId;
		}

		return prefabId;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="building"></param>
	/// <param name="pos"></param>
	/// <param name="rotation"></param>
	public static string InstantiateCommercialBuilding(CommercialBuilding building, Vector3 pos, Quaternion rotation)
	{
		System.Random random = new();
		GameObject prefab = null;
		string prefabId = string.Empty;
		int randomIndex = 0;

		switch (building.level)
		{
			case 0:
				randomIndex = random.Next(3);
				prefab = Utility.commercialLevelZeroPrefabs[randomIndex];
				prefabId = "com_0" + randomIndex;
				break;
			case 1:
				randomIndex = random.Next(4);
				prefab = Utility.commercialLevelOnePrefabs[random.Next(3)];
				prefabId = "com_1" + randomIndex;
				break;
			case 2:
				randomIndex = random.Next(4);
				prefab = Utility.commercialLevelTwoPrefabs[random.Next(4)];
				prefabId = "com_2" + randomIndex;
				break;
			case 3:
				randomIndex = random.Next(6);
				prefab = Utility.commercialLevelThreePrefabs[random.Next(3)];
				prefabId = "com_3" + randomIndex;
				break;
			default:
				Debug.LogError("InfrastructureManager: commercial switch failed to enter any case");
				break;
		}

		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() =>
					{
						GameObject instantiatedObject = Instantiate(prefab, pos, rotation);
						bool success = instantiatedBuildings.TryAdd(pos, instantiatedObject);
						Debug.Log($"InfrastructureManager: success: {success}, add key: {pos} and value: {instantiatedObject}");
					});
				return prefabId;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: commercial prefab is null, instantiation aborted");
			return prefabId;
		}

		return prefabId;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="building"></param>
	/// <param name="pos"></param>
	/// <param name="rotation"></param>
	public static string InstantiateIndustrialBuilding(IndustrialBuilding building)
	{
		System.Random random = new();
		GameObject prefab = null;
		string prefabId = string.Empty;
		int randomIndex = 0;

		switch (building.level)
		{
			case 0:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelZeroPrefabs[randomIndex];
				prefabId = "ind_0" + randomIndex;
				Debug.Log($"Level 0: randomIndex={randomIndex}, prefabId={prefabId}");
				break;
			case 1:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelOnePrefabs[randomIndex];
				prefabId = "ind_1" + randomIndex;
				break;
			case 2:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelTwoPrefabs[randomIndex];
				prefabId = "ind_2" + randomIndex;
				break;
			case 3:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelThreePrefabs[randomIndex];
				prefabId = "ind_3" + randomIndex;
				break;
			default:
				Debug.LogError("InfrastructureManager: industrial switch default");
				break;
		}
		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject instantiatedObject = Instantiate(prefab, building.pos, building.rotation);
					bool success = instantiatedBuildings.TryAdd(building.pos, instantiatedObject);
					Debug.Log($"InfrastructureManager: success: {success}, add key: {building.pos} and value: {instantiatedObject}");
				});
				Debug.Log($"prefabId: {prefabId}");
				return prefabId;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: industrial prefab is null, instantiation aborted");
			Debug.Log($"prefabId: {prefabId}");
			return prefabId;
		}
		Debug.Log($"prefabId: {prefabId}");
		return prefabId;
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="building"></param>
	/// <param name="pos"></param>
	public static string InstantiateUtilityBuilding(UtilityBuilding building, Vector3 pos)
	{
		if (building.utilityType == UtilityType.Power)
		{
			GameObject prefab = Utility.utilityPowerPrefabs[building.level];
			Instantiate(prefab, pos, Quaternion.identity);
			return "power_" + building.level;
		}

		if (building.utilityType == UtilityType.Water)
		{
			GameObject prefab = Utility.utilityWaterPrefabs[building.level];
			Instantiate(prefab, pos, Quaternion.identity);
			return "water_" + building.level;
		}

		return string.Empty;
	}

	public static string InstantiateUtilityDistribution(UtilityDistribution building, Vector3 pos)
	{
		GameObject prefab;
		Debug.Log($"InfrastructureManager: Instantiating UtilityDistribution at pos: {pos}");

		switch (building.utilityType)
		{
			case UtilityType.Power:
				prefab = Utility.utilityPowerDistPrefabs[building.level];
				Instantiate(prefab, pos, Quaternion.identity);
				return "powerDist_" + building.level;

			case UtilityType.Water:
				prefab = Utility.utilityWaterDistPrefabs[building.level];
				Instantiate(prefab, pos, Quaternion.identity);
				return "waterDist_" + building.level;

			default:
				Debug.Log("InfrastructureManager: Unknown UtilityDistribution type");
				return string.Empty;
		}
	}

	public static void InstantiateServiceBuilding(ServiceBuilding building, Vector3 pos)
	{
		GameObject prefab = null;

		switch (building.serviceType)
		{
			case ServiceType.Hospital:
				prefab = Utility.hospitalPrefab;
				break;
			case ServiceType.FireStation:
				prefab = Utility.fireStationPrefab;
				break;
			case ServiceType.PoliceStation:
				prefab = Utility.policeStationPrefab;
				break;
			case ServiceType.School:
				prefab = Utility.schoolPrefab;
				break;
			default:
				Debug.LogError("InfrastructureManager: Unknown ServiceType");
				return;
		}

		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() => Instantiate(prefab, pos, Quaternion.identity));
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: Service building prefab is null, instantiation aborted");
		}
	}

	public static void InstantiateRoad_Straight(List<Vector3> affectedCells, Quaternion rotation)
	{
		foreach (var pos in affectedCells)
		{
			Instantiate(Utility.roadStraight, pos, rotation);
		}
	}

	public static void InstantiateRoad_Structure(RoadStructureType type, Vector3 pos, Quaternion rotation)
	{
		switch (type)
		{
			case RoadStructureType.Curve:
				Instantiate(Utility.roadCurve, pos, rotation);
				break;
			case RoadStructureType.IntersectionT:
				Instantiate(Utility.roadIntersectionT, pos, rotation);
				break;
			case RoadStructureType.IntersectionCross:
				Instantiate(Utility.roadIntersectionCross, pos, rotation);
				break;
		}
	}

	/// <summary>
	/// Upgrades the ResidentialBuilding. Destroys the existing building and instantiates a new one.
	/// </summary>
	/// <param name="building"></param>
	public static string InstantiateResidentialBuilding_Upgrade(ResidentialBuilding building)
	{
		Vector3 pos = building.pos;
		Quaternion rotation = building.rotation;

		// Log all keys in the dictionary
		Debug.Log("InfrastructureManager: Current keys in instantiatedBuildings:");
		foreach (var key in instantiatedBuildings.Keys)
		{
			Debug.Log($"Key: {key}");
		}

		Debug.Log($"InfrastructureManager: Checking for key {pos}");

		if (instantiatedBuildings.ContainsKey(pos))
		{
			Debug.Log("InfrastructureManager: ContainsKey");
			GameObject existingBuilding = instantiatedBuildings[pos];
			Debug.Log($"InfrastructureManager: existingBuilding assigned");

			if (existingBuilding != null)
			{
				Debug.Log($"InfrastructureManager: existingBuilding is not null");
				try
				{
					MainThreadDispatcher.Enqueue(() =>
					{
						Destroy(existingBuilding);
						bool success = instantiatedBuildings.TryRemove(pos, out GameObject removedObject);
						Debug.Log($"InfrastructureManager: remove success: {success}, removedObject: {removedObject}");
					});
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					Debug.LogError(e.StackTrace);
					return string.Empty;
				}
			}
			else
			{
				Debug.LogError("InfrastructureManager: existingBuilding is null");
				return string.Empty;
			}
		}
		else
		{
			Debug.LogError($"InfrastructureManager: doesn't contain key {pos}");
			return string.Empty;
		}

		GameObject prefab = null;
		string prefabId = string.Empty;
		System.Random random = new();
		int randomIndex = 0;

		switch (building.level)
		{
			case 1:
				randomIndex = random.Next(5);
				prefab = Utility.residentialLevelOnePrefabs[randomIndex];
				prefabId = "res_1" + randomIndex;
				break;
			case 2:
				randomIndex = random.Next(2);
				prefab = Utility.residentialLevelTwoPrefabs[randomIndex];
				prefabId = "res_2" + randomIndex;
				break;
			case 3:
				randomIndex = random.Next(4);
				prefab = Utility.residentialLevelThreePrefabs[randomIndex];
				prefabId = "res_3" + randomIndex;
				break;
			case 4:
				randomIndex = random.Next(4);
				prefab = Utility.residentialLevelFourPrefabs[randomIndex];
				prefabId = "res_4" + randomIndex;
				break;
			default:
				Debug.LogError("InfrastructureManager: residential switch failed to enter any case");
				break;
		}

		// instantiate new prefab
		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject instantiatedObject = Instantiate(prefab, pos, rotation);
					Debug.Log("InfrastructureManager: instantiate upgrade successful");
					instantiatedBuildings.TryAdd(pos, instantiatedObject);
					Debug.Log($"InfrastructureManager: adding upgraded prefab, instantiatedObject:{instantiatedObject}");
				});
				return prefabId;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
				return string.Empty;
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: residential prefab is null, instantiation aborted");
			return string.Empty;
		}
	}

	public static void InstantiateCommercialBuilding_Upgrade(CommercialBuilding building)
	{
		Vector3 pos = building.pos;
		Quaternion rotation = building.rotation;

		// Log all keys in the dictionary
		Debug.Log("InfrastructureManager: Current keys in instantiatedBuildings:");
		foreach (var key in instantiatedBuildings.Keys)
		{
			Debug.Log($"Key: {key}");
		}

		Debug.Log($"InfrastructureManager: Checking for key {pos}");

		if (instantiatedBuildings.ContainsKey(pos))
		{
			Debug.Log("InfrastructureManager: ContainsKey");
			GameObject existingBuilding = instantiatedBuildings[pos];
			Debug.Log($"InfrastructureManager: existingBuilding assigned");

			if (existingBuilding != null)
			{
				Debug.Log($"InfrastructureManager: existingBuilding is not null");
				try
				{
					MainThreadDispatcher.Enqueue(() =>
					{
						Destroy(existingBuilding);
						bool success = instantiatedBuildings.TryRemove(pos, out GameObject removedObject);
						Debug.Log($"InfrastructureManager: remove success: {success}, removedObject: {removedObject}");
					});
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					Debug.LogError(e.StackTrace);
					return;
				}
			}
			else
			{
				Debug.LogError("InfrastructureManager: existingBuilding is null");
				return;
			}
		}
		else
		{
			Debug.LogError($"InfrastructureManager: doesn't contain key {pos}");
			return;
		}

		GameObject prefab = null;
		string prefabId = string.Empty;
		System.Random random = new();
		int randomIndex = 0;

		switch (building.level)
		{
			case 1:
				randomIndex = random.Next(4);
				prefab = Utility.commercialLevelOnePrefabs[randomIndex];
				prefabId = "com_1" + randomIndex;
				break;
			case 2:
				randomIndex = random.Next(4);
				prefab = Utility.commercialLevelTwoPrefabs[randomIndex];
				prefabId = "com_2" + randomIndex;
				break;
			case 3:
				randomIndex = random.Next(6);
				prefab = Utility.commercialLevelThreePrefabs[randomIndex];
				prefabId = "com_3" + randomIndex;
				break;
			default:
				Debug.LogError("InfrastructureManager: residential switch failed to enter any case");
				break;
		}

		// instantiate new prefab
		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject instantiatedObject = Instantiate(prefab, pos, rotation);
					Debug.Log("InfrastructureManager: instantiate upgrade successful");
					instantiatedBuildings.TryAdd(pos, instantiatedObject);
					Debug.Log($"InfrastructureManager: adding upgraded prefab, instantiatedObject:{instantiatedObject}");
				});
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: residential prefab is null, instantiation aborted");
		}
	}

	public static void InstantiateIndustrialBuilding_Upgrade(IndustrialBuilding building)
	{
		Vector3 pos = building.pos;
		Quaternion rotation = building.rotation;

		// Log all keys in the dictionary

		Debug.Log($"InfrastructureManager: Checking for key {pos}");

		if (instantiatedBuildings.ContainsKey(pos))
		{
			Debug.Log("InfrastructureManager: ContainsKey");
			GameObject existingBuilding = instantiatedBuildings[pos];
			Debug.Log($"InfrastructureManager: existingBuilding assigned");

			if (existingBuilding != null)
			{
				Debug.Log($"InfrastructureManager: existingBuilding is not null");
				try
				{
					MainThreadDispatcher.Enqueue(() =>
					{
						Destroy(existingBuilding);
						bool success = instantiatedBuildings.TryRemove(pos, out GameObject removedObject);
						Debug.Log($"InfrastructureManager: remove success: {success}, removedObject: {removedObject}");
					});
				}
				catch (Exception e)
				{
					Debug.LogError(e.Message);
					Debug.LogError(e.StackTrace);
					return;
				}
			}
			else
			{
				Debug.LogError("InfrastructureManager: existingBuilding is null");
				return;
			}
		}
		else
		{
			Debug.LogError($"InfrastructureManager: doesn't contain key {pos}");
			return;
		}

		GameObject prefab = null;
		string prefabId = string.Empty;
		System.Random random = new();
		int randomIndex = 0;

		switch (building.level)
		{
			case 1:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelOnePrefabs[randomIndex];
				prefabId = "res_1" + randomIndex;
				break;
			case 2:
				randomIndex = random.Next(2);
				prefab = Utility.industrialLevelTwoPrefabs[randomIndex];
				prefabId = "res_2" + randomIndex;
				break;
			case 3:
				randomIndex = random.Next(0);
				prefab = Utility.industrialLevelThreePrefabs[randomIndex];
				prefabId = "res_3" + randomIndex;
				break;
			default:
				Debug.LogError("InfrastructureManager: residential switch failed to enter any case");
				break;
		}

		// instantiate new prefab
		if (prefab != null)
		{
			try
			{
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject instantiatedObject = Instantiate(prefab, pos, rotation);
					Debug.Log("InfrastructureManager: instantiate upgrade successful");
					instantiatedBuildings.TryAdd(pos, instantiatedObject);
					Debug.Log($"InfrastructureManager: adding upgraded prefab, instantiatedObject:{instantiatedObject}");
				});
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: residential prefab is null, instantiation aborted");
		}
	}

	public static void InstantiateZoneHighlightPlane(ZoneData.Type type, Vector3 startPos, Vector3 endPos)
	{
		GameObject prefab = null;
		switch (type)
		{
			case ZoneData.Type.Residential:
				prefab = Utility.residentialHighlightPlane;
				break;
			case ZoneData.Type.Commercial:
				prefab = Utility.commercialHighlightPlane;
				break;
			case ZoneData.Type.Industrial:
				prefab = Utility.industrialHighlightPlane;
				break;
		}
		if (prefab != null)
		{
			try
			{
				//Vector3 center = ((startPos.x + endPos.x) / 2,5, + (endPos.z+endPos.z) / 2));
				Vector3 center = (startPos + endPos) / 2;
				center.y = 25;
				// Calculate scale based on distance
				float scaleFactor = 1f;  // Each 10 units of distance adds 1 to scale
				float xScale = 10 + (endPos.x - startPos.x) / scaleFactor;
				float zScale = 10 + (endPos.z - startPos.z) / scaleFactor;
				Vector3 size = new Vector3(xScale, 50, zScale);

				// Instantiate and scale the plane
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject plane = Instantiate(prefab, center, Quaternion.identity);
					plane.transform.localScale = size;
					instantiatedPlanes.Add(plane);  // Store the reference
				});

			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: Highlight plane prefab is null, instantiation aborted");
		}
	}

	public static void DestroyPlanes()
	{
		foreach (GameObject plane in instantiatedPlanes)
		{
			if (plane != null)
			{
				GameObject.Destroy(plane);  // Destroy the instantiated plane
			}
		}
		instantiatedPlanes.Clear();  // Clear the list after destroying all planes
	}

	public static void InstantiateUtilityDistributionHighlight(UtilityDistribution building, Vector3 placementPos)
	{
		GameObject prefab = null;
		switch (building.utilityType)
		{
			case UtilityType.Power:
				Debug.Log("POwer Prefab selected in case");
				prefab = Utility.powerDistributionHighlightPlane; // Ensure you have this prefab
				break;
			case UtilityType.Water:
				prefab = Utility.waterDistributionHighlightPlane; // Ensure you have this prefab
				break;
			default:
				Debug.LogError("InfrastructureManager: Unknown UtilityDistribution type for highlighting");
				return;
		}

		if (prefab != null)
		{
			try
			{
				byte radius = 100;
				Vector3 startPos = new(Mathf.Clamp(placementPos.x - radius, 0, 2000), 0, Mathf.Clamp(placementPos.z - radius, 0, 2000));
				Vector3 endPos = new(Mathf.Clamp(placementPos.x + radius, 0, 2000), 0, Mathf.Clamp(placementPos.z + radius, 0, 2000));
				Vector3 center = (startPos + endPos) / 2;
				center.y = 25;
				// Calculate scale based on distance
				float scaleFactor = 1f;  // Each 10 units of distance adds 1 to scale
				float xScale = 10 + (endPos.x - startPos.x) / scaleFactor;
				float zScale = 10 + (endPos.z - startPos.z) / scaleFactor;
				Vector3 size = new Vector3(xScale, 50, zScale);
				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject plane = Instantiate(prefab, center, Quaternion.identity);
					plane.transform.localScale = size;
					instantiatedPlanesUtilityDist.Add(plane);  // Store the reference
				});
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: Highlight plane prefab is null, instantiation aborted");
		}
	}

	public static void DestroyPlanesDist()
	{
		foreach (GameObject plane in instantiatedPlanesUtilityDist)
		{
			if (plane != null)
			{
				GameObject.Destroy(plane);  // Destroy the instantiated plane
			}
		}
		instantiatedPlanesUtilityDist.Clear();  // Clear the list after destroying all planes
	}


	public static void InstantiateServiceDistributionHighlight(ServiceBuilding building, Vector3 placementPos)
	{
		GameObject prefab = null;
		switch (building.serviceType)
		{
			case ServiceType.Hospital:
				prefab = Utility.hospitalHighlightPlane; // Ensure you have this prefab
				break;
			case ServiceType.FireStation:
				prefab = Utility.fireStationHighlightPlane; // Ensure you have this prefab
				break;
			case ServiceType.PoliceStation:
				prefab = Utility.policeStationHighlightPlane; // Ensure you have this prefab
				break;
			case ServiceType.School:
				prefab = Utility.schoolHighlightPlane; // Ensure you have this prefab
				break;
			default:
				Debug.LogError("InfrastructureManager: Unknown ServiceBuilding type for highlighting");
				return;
		}

		if (prefab != null)
		{
			try
			{
				byte radius = 100;
				Vector3 startPos = new(Mathf.Clamp(placementPos.x - radius, 0, 2000), 0, Mathf.Clamp(placementPos.z - radius, 0, 2000));
				Vector3 endPos = new(Mathf.Clamp(placementPos.x + radius, 0, 2000), 0, Mathf.Clamp(placementPos.z + radius, 0, 2000));
				Vector3 center = (startPos + endPos) / 2;
				center.y = 25;

				// Calculate scale based on distance
				float scaleFactor = 1f;  // Each 10 units of distance adds 1 to scale
				float xScale = 10 + (endPos.x - startPos.x) / scaleFactor;
				float zScale = 10 + (endPos.z - startPos.z) / scaleFactor;
				Vector3 size = new Vector3(xScale, 50, zScale);

				MainThreadDispatcher.Enqueue(() =>
				{
					GameObject plane = Instantiate(prefab, center, Quaternion.identity);
					plane.transform.localScale = size;
					instantiatedPlanesServiceDist.Add(plane);  // Store the reference
				});
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				Debug.LogError(e.StackTrace);
			}
		}
		else
		{
			Debug.LogError("InfrastructureManager: Highlight plane prefab is null, instantiation aborted");
		}

	}

	public static void DestroyServicePlanesDist()
	{
		foreach (GameObject plane in instantiatedPlanesServiceDist)
		{
			if (plane != null)
			{
				GameObject.Destroy(plane);  // Destroy the instantiated plane
			}
		}
		instantiatedPlanesServiceDist.Clear();  // Clear the list after destroying all planes
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="building"></param>
	public static void InstantiateBuilding<T>(T building) where T : Building
	{
		GameObject prefab = null;

		if (building is ResidentialBuilding)
			prefab = Utility.residentialPrefabs[building.prefabId];
		else if (building is CommercialBuilding)
			prefab = Utility.commercialPrefabs[building.prefabId];
		else if (building is IndustrialBuilding)
			prefab = Utility.industrialPrefabs[building.prefabId];
		else if (building is UtilityBuilding)
			prefab = Utility.utilityPrefabs[building.prefabId];
		else if (building is UtilityDistribution)
			prefab = Utility.UtilityDistPrefabs[building.prefabId];

		try
		{
			MainThreadDispatcher.Enqueue(() =>
			{
				GameObject instantiatedObject = Instantiate(prefab, building.pos, building.rotation);
				bool success = instantiatedBuildings.TryAdd(building.pos, instantiatedObject);
			});
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
			Debug.LogError(e.StackTrace);
		}
	}

}