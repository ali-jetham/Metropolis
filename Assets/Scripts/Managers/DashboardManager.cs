using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handle all the dashboard inputs and raise events accordingly. 
/// </summary>
public class DashboardManager : MonoBehaviour
{
	// UI canvas
	public GameObject canvas;

	// references to panels
	public GameObject dashboardPanel;
	public GameObject cityManagementPanel;
	public GameObject cityMonitorPanel;

	// panels within city management
	public GameObject utilityPanel;
	public GameObject zoningPanel;
	public GameObject servicePanel;
	public GameObject roadsPanel;
	public GameObject utilityDistributionPanel;
	public GameObject powerUtilityPanel;
	public GameObject waterUtilityPanel;

	// panels within city monitor
	public GameObject monitorZonePanel;
	public GameObject monitorUtilityDistPanel;
	public GameObject monitorServicePanel;

	// public GameObject prefab;
	public GameObject hoverBuilding;
	public GameObject gridOutline;

	public City city;

	private Vector3 pos;
	private RaycastHit hit;
	public LayerMask layerMask;


	// events
	public event Action<UtilityType, byte, Vector3> OnUtilityBuildingPlaced;
	public event Action<UtilityType, Vector3> OnUtilityDistributionPlaced;
	public event Action<ServiceType, Vector3> OnServicePlaced;
	public event Action<Vector3, Vector3> OnRoadPlaced;
	public event Action<RoadStructureType, Vector3, Quaternion> OnRoadStructurePlaced;
	public event Action<ZoneData.Type> OnRequestZoneData;
	public event Action<List<ZoneData>> OnReceiveZoneData;
	public event Action<ZoneData.Type, Vector3, Vector3> OnHighlightZone;
	public event Action<UtilityType> OnUtilityDistMonitorButtonClicked;
	public event Action<ServiceType> OnServiceDistMonitorButtonClicked;

	private void FixedUpdate()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 1000, layerMask))
		{
			pos = hit.point;
		}
	}

	private void Update()
	{
		if (listenForRoad)
		{ ListenForPlaceRoad(); }

		if (currentState == PlacementState.Hovering)
		{
			if (hoverBuilding != null)
			{
				hoverBuilding.transform.position = Utility.Round(pos);
			}

			if (hoverBuilding != null)
			{
				if (Input.GetKeyDown(KeyCode.R))
				{
					hoverBuilding.transform.rotation = hoverBuilding.transform.rotation * Quaternion.Euler(0, 90, 0);
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (currentBuildingType == BuildingType.Service)
				{
					PlaceServiceBuilding(currentServiceType);
					return;
				}
				PlaceBuilding(currentBuildingType);
			}
		}
	}
	public event Action OnResidentialZoneClicked;
	public event Action OnCommercialZoneClicked;
	public event Action OnIndustrialZoneClicked;

	/// <summary>
	/// Disable currently active panel and show dashboard panel.
	/// </summary>
	public void OnDashboardPanelButtonClick()
	{
		Debug.Log("InputManager: OnDashboardPanelButtonClick() called");
		dashboardPanel.SetActive(true);
		cityManagementPanel.SetActive(false);
	}

	public void OnHomeButtonClick()
	{
		Debug.Log("InputManager: Home Button clicked");
		dashboardPanel.SetActive(true);
		cityManagementPanel.SetActive(false);
		cityMonitorPanel.SetActive(false);
	}


	/// <summary>
	/// Open the CityManagementPanel
	/// </summary>
	public void OnCityManagementButtonClick()
	{
		Debug.Log("InputManager: OnCityManagementButtonClick() called");
		dashboardPanel.SetActive(false);
		cityManagementPanel.SetActive(true);
	}

	public void OnResidentialButtonClick()
	{
		canvas.SetActive(false);
		OnResidentialZoneClicked?.Invoke();
	}

	public void OnCommercialButtonClick()
	{
		canvas.SetActive(false);
		OnCommercialZoneClicked?.Invoke();
	}


	// Used to manage state and necessary data for HoverBuilding() and PlaceBuilding()  
	private enum PlacementState
	{
		None,
		Hovering,
		Placed
	}
	private enum HoveringType
	{
	}
	private PlacementState currentState = PlacementState.None;

	public void OnUtilitiesButtonClick()
	{
		zoningPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(false);
		utilityPanel.SetActive(true);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(false);
	}

	public void OnServiceButtonClick()
	{
		utilityPanel.SetActive(false);
		zoningPanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(false);
		servicePanel.SetActive(true);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(false);
	}

	public void OnZoningButtonClick()
	{
		utilityPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(false);
		zoningPanel.SetActive(true);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(false);
	}

	public void OnRoadsButtonClick()
	{
		utilityPanel.SetActive(false);
		zoningPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(true);
		utilityDistributionPanel.SetActive(false);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(false);
	}

	public void OnUtilityDistributionClick()
	{
		utilityPanel.SetActive(false);
		zoningPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(true);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(false);
	}

	/// <summary>
	/// On click of the "Power Supply" button that appears in the utilityPanel
	/// </summary>
	public void OnPowerSupplyButtonClick()
	{
		utilityPanel.SetActive(false);
		zoningPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(false);

		powerUtilityPanel.SetActive(true);
		waterUtilityPanel.SetActive(false);
	}

	/// <summary>
	/// On click of the "Water Supply" button that appears in the utilityPanel
	/// </summary>
	public void OnWaterSupplyButtonClick()
	{
		utilityPanel.SetActive(false);
		zoningPanel.SetActive(false);
		servicePanel.SetActive(false);
		roadsPanel.SetActive(false);
		utilityDistributionPanel.SetActive(false);

		powerUtilityPanel.SetActive(false);
		waterUtilityPanel.SetActive(true);
	}

	public void OnIndustrialButtonClick()
	{
		canvas.SetActive(false);
		OnIndustrialZoneClicked?.Invoke();
	}

	/// <summary>
	/// Open the CityMonitorPanel
	/// </summary>
	public void OnCityMonitorPanelButtonClick()
	{
		Debug.Log("DashboardManager: OnCityMonitorPanelButtonClick() called");
		dashboardPanel.SetActive(false);  // Hide the dashboard panel
		cityMonitorPanel.SetActive(true);  // Show the city monitor panel
	}

	public void OnMonitorZoneClick()
	{
		monitorZonePanel.SetActive(true);
		monitorUtilityDistPanel.SetActive(false);
		monitorServicePanel.SetActive(false);
	}

	public void OnMonitorUtilityDistClick()
	{
		monitorZonePanel.SetActive(false);
		monitorUtilityDistPanel.SetActive(true);
		monitorServicePanel.SetActive(false);
	}

	public void OnMonitorServiceClick()
	{
		monitorZonePanel.SetActive(false);
		monitorUtilityDistPanel.SetActive(false);
		monitorServicePanel.SetActive(true);
	}


	private bool IsZoneHighlighted(ZoneData.Type type, Vector3 startPos, Vector3 endPos)
	{
		// Implement logic to determine if the zone is already highlighted
		// For example, check against a list or map of currently highlighted zones
		return false; // Placeholder implementation
	}

	public void OnMonitorResButtonClicked()
	{
		Debug.Log("Monitor Residential button clicked");

		OnReceiveZoneData += HandleReceiveZoneData;
		OnRequestZoneData?.Invoke(ZoneData.Type.Residential);
	}


	public void OnMonitorComButtonClick()
	{
		Debug.Log("Monitor Residential button clicked");

		OnReceiveZoneData += HandleReceiveZoneData;
		OnRequestZoneData?.Invoke(ZoneData.Type.Commercial);
	}

	public void OnMonitorIndButtonClick()
	{
		Debug.Log("Monitor Residential button clicked");

		OnReceiveZoneData += HandleReceiveZoneData;
		OnRequestZoneData?.Invoke(ZoneData.Type.Industrial);
	}

	public void HandleReceiveZoneData(List<ZoneData> zoneDataList)
	{
		Debug.Log("DashboardManager: HandleReceiveZoneData called");

		foreach (var zoneData in zoneDataList)
		{
			switch (zoneData.type)
			{
				case ZoneData.Type.Residential:
					Vector3 minPosRes = zoneData.startPos;
					Vector3 maxPosRes = zoneData.endPos;

					// Log bounding box positions
					Debug.Log($"Zone Type: {zoneData.type}");
					Debug.Log("Bounding Box Min Position: " + minPosRes);
					Debug.Log("Bounding Box Max Position: " + maxPosRes);
					HighlightZone(ZoneData.Type.Residential, minPosRes, maxPosRes);
					break;

				case ZoneData.Type.Commercial:
					Vector3 minPosCom = zoneData.startPos;
					Vector3 maxPosCom = zoneData.endPos;

					// Log bounding box positions
					Debug.Log($"Zone Type: {zoneData.type}");
					Debug.Log("Bounding Box Min Position: " + minPosCom);
					Debug.Log("Bounding Box Max Position: " + maxPosCom);
					HighlightZone(ZoneData.Type.Commercial, minPosCom, maxPosCom);
					break;

				case ZoneData.Type.Industrial:
					Vector3 minPosInd = zoneData.startPos;
					Vector3 maxPosInd = zoneData.endPos;

					// Log bounding box positions
					Debug.Log($"Zone Type: {zoneData.type}");
					Debug.Log("Bounding Box Min Position: " + minPosInd);
					Debug.Log("Bounding Box Max Position: " + maxPosInd);
					HighlightZone(ZoneData.Type.Industrial, minPosInd, maxPosInd);
					break;

				default:
					Debug.LogWarning("Unknown zone type: " + zoneData.type);
					break;
			}
		}

		// Unsubscribe from the event after receiving the data
		OnReceiveZoneData -= HandleReceiveZoneData;
	}

	public void HighlightZone(ZoneData.Type type, Vector3 startPos, Vector3 endPos)
	{
		if (IsZoneHighlighted(type, startPos, endPos))
		{
			Debug.Log($"Zone of type {type} from {startPos} to {endPos} is already highlighted.");
			return;
		}
		switch (type)
		{
			case ZoneData.Type.Residential:
				OnHighlightZone?.Invoke(ZoneData.Type.Residential, startPos, endPos);
				break;
			case ZoneData.Type.Commercial:
				OnHighlightZone?.Invoke(ZoneData.Type.Commercial, startPos, endPos);
				break;
			case ZoneData.Type.Industrial:
				OnHighlightZone?.Invoke(ZoneData.Type.Industrial, startPos, endPos);
				break;
		}
	}
	public void OnStopMonitorButtonClicked()
	{
		InfrastructureManager.DestroyPlanes();
	}

	public void OnPowerDistMonitorButtonCLick()
	{
		Debug.Log("Monitor Utility Distribution Button Clicked");
		OnUtilityDistMonitorButtonClicked?.Invoke(UtilityType.Power);
	}
	public void OnWaterDistMonitorButtonCLick()
	{
		Debug.Log("Monitor Utility Distribution Button Clicked");
		OnUtilityDistMonitorButtonClicked?.Invoke(UtilityType.Water);
	}
	public void OnUtilityDistStopMonitorButtonClicked()
	{
		InfrastructureManager.DestroyPlanesDist();
	}

	public void OnHospitalMonitorButtonCLick()
	{
		Debug.Log("Monitor Service Distribution Button Clicked");
		OnServiceDistMonitorButtonClicked?.Invoke(ServiceType.Hospital);

	}

	public void OnFireStationMonitorButtonCLick()
	{
		Debug.Log("Monitor Service Distribution Button Clicked");
		OnServiceDistMonitorButtonClicked?.Invoke(ServiceType.FireStation);

	}

	public void OnPoliceStationMonitorButtonCLick()
	{
		Debug.Log("Monitor Service Distribution Button Clicked");
		OnServiceDistMonitorButtonClicked?.Invoke(ServiceType.PoliceStation);

	}

	public void OnSchoolMonitorButtonCLick()
	{
		Debug.Log("Monitor Service Distribution Button Clicked");
		OnServiceDistMonitorButtonClicked?.Invoke(ServiceType.School);

	}

	public void OnServiceDistStopMonitorButtonClicked()
	{
		InfrastructureManager.DestroyServicePlanesDist();
	}


	private byte currentLevel;
	private BuildingType currentBuildingType;
	private UtilityType currentUtilityType;
	private ServiceType currentServiceType;
	// Used to manage state and necessary data for HoverBuilding() and PlaceBuilding()  

	/// <summary>
	/// Hovers a building prefab.
	/// </summary>
	/// <param name="utilityType">UtilityType of the building to be hovered.</param>
	/// <param name="level">Level of the building to be hovered</param>
	public void HoverBuilding(BuildingType buildingType, byte level, UtilityType utilityType = UtilityType.None)
	{
		Debug.Log("DashboardManager: HoverUtilityBuilding() called");
		if (hoverBuilding != null)
		{
			Debug.Log("DashboardManager: Destroying existing hoverBuilding");
			Destroy(hoverBuilding);
		}

		currentBuildingType = buildingType;
		currentLevel = level;
		currentUtilityType = utilityType;
		Utility.isHoverBuilding = true;

		Debug.Log($"DashboardManager: BuildingType is {currentBuildingType}");
		Debug.Log($"DashboardManager: UtilityType is {utilityType}");

		switch (buildingType)
		{
			case BuildingType.Utility:
				if (utilityType == UtilityType.Power)
					hoverBuilding = Instantiate(Utility.utilityPowerPrefabs[level], pos, Quaternion.identity);

				if (utilityType == UtilityType.Water)
					hoverBuilding = Instantiate(Utility.utilityWaterPrefabs[level], pos, Quaternion.identity);
				break;

			case BuildingType.UtilityDistribution:
				if (utilityType == UtilityType.Power)
					hoverBuilding = Instantiate(Utility.utilityPowerDistPrefabs[level], pos, Quaternion.identity);

				if (utilityType == UtilityType.Water)
					hoverBuilding = Instantiate(Utility.utilityWaterDistPrefabs[level], pos, Quaternion.identity);
				break;

			case BuildingType.RoadCurve:
				hoverBuilding = Instantiate(Utility.roadCurve, pos, Quaternion.identity);
				break;

			case BuildingType.RoadIntersectionT:
				hoverBuilding = Instantiate(Utility.roadIntersectionT, pos, Quaternion.identity);
				break;

			case BuildingType.RoadIntersectionCross:
				hoverBuilding = Instantiate(Utility.roadIntersectionCross, pos, Quaternion.identity);
				break;
		}

		currentState = PlacementState.Hovering;
	}

	public void HoverServiceBuilding(ServiceType type)
	{
		Debug.Log("DashboardManager: HoverServiceBuilding() called");

		if (hoverBuilding != null)
		{
			Debug.Log("DashboardManager: Destroying existing hoverBuilding");
			Destroy(hoverBuilding);
		}
		currentBuildingType = BuildingType.Service;
		currentServiceType = type;
		switch (type)
		{
			case ServiceType.Hospital:
				hoverBuilding = Instantiate(Utility.hospitalPrefab, pos, Quaternion.identity);
				break;
			case ServiceType.FireStation:
				hoverBuilding = Instantiate(Utility.fireStationPrefab, pos, Quaternion.identity);
				break;
			case ServiceType.PoliceStation:
				hoverBuilding = Instantiate(Utility.policeStationPrefab, pos, Quaternion.identity);
				break;
			case ServiceType.School:
				hoverBuilding = Instantiate(Utility.schoolPrefab, pos, Quaternion.identity);
				break;
			default:
				Debug.LogError("DashboardManager: Invalid ServiceType");
				break;
		}
		currentState = PlacementState.Hovering;
	}

	private void PlaceServiceBuilding(ServiceType type)
	{
		if (hoverBuilding != null)
		{
			Vector3 placementPos = hoverBuilding.transform.position;
			Debug.Log($"DashboardManager: Placing {type} at {placementPos}");

			hoverBuilding = null;
			currentState = PlacementState.Placed;

			switch (type)
			{
				case ServiceType.Hospital:
					OnServicePlaced?.Invoke(ServiceType.Hospital, placementPos);
					break;
				case ServiceType.FireStation:
					OnServicePlaced?.Invoke(ServiceType.FireStation, placementPos);
					break;
				case ServiceType.PoliceStation:
					OnServicePlaced?.Invoke(ServiceType.PoliceStation, placementPos);
					break;
				case ServiceType.School:
					OnServicePlaced?.Invoke(ServiceType.School, placementPos);
					break;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	public void PlaceBuilding(BuildingType type)
	{
		if (hoverBuilding != null)
		{
			Vector3 placementPos = hoverBuilding.transform.position;
			Quaternion rotation = hoverBuilding.transform.rotation;
			Debug.Log($"DashboardManager placementPos: {placementPos}");
			Destroy(hoverBuilding);
			hoverBuilding = null;
			currentState = PlacementState.Placed;
			Utility.isHoverBuilding = false;

			switch (type)
			{
				case BuildingType.Utility:
					OnUtilityBuildingPlaced?.Invoke(currentUtilityType, currentLevel, placementPos);
					break;
				case BuildingType.UtilityDistribution:
					OnUtilityDistributionPlaced?.Invoke(currentUtilityType, placementPos);
					break;
				case BuildingType.RoadCurve:
					OnRoadStructurePlaced?.Invoke(RoadStructureType.Curve, placementPos, rotation);
					Debug.Log($"DashboardManager: roadCurve placePos is {placementPos}");
					break;
				case BuildingType.RoadIntersectionT:
					OnRoadStructurePlaced?.Invoke(RoadStructureType.IntersectionT, placementPos, rotation);
					Debug.Log($"DashboardManager: roadIntersectionT placePos is {placementPos}");
					break;
				case BuildingType.RoadIntersectionCross:
					OnRoadStructurePlaced?.Invoke(RoadStructureType.IntersectionCross, placementPos, rotation);
					Debug.Log($"DashboardManager: roadIntersectionCross placePos is {placementPos}");
					break;
			}
		}
	}

	///*** state variables, global data variables and more to be used with PlaceRoad() ***///
	private enum RoadPlacementState
	{
		WaitingForFirstClick,
		WaitingForSecondClick
	}
	private RoadPlacementState roadPlacementState = RoadPlacementState.WaitingForFirstClick;
	Vector3 roadStartPos;
	Vector3 roadEndPos;
	private bool listenForRoad = false;
	///*** state variables, global data variables and more to be used with PlaceRoad() ***///

	/// <summary>
	/// 
	/// </summary>
	public void ListenForPlaceRoad()
	{
		switch (roadPlacementState)
		{
			case RoadPlacementState.WaitingForFirstClick:
				if (Input.GetMouseButtonDown(0))
				{
					roadStartPos = InputManager.GetWorldPositionOnMouseClick();
					Debug.Log($"roadStartPos: {roadStartPos}");
					roadPlacementState = RoadPlacementState.WaitingForSecondClick;
				}
				break;

			case RoadPlacementState.WaitingForSecondClick:
				if (Input.GetMouseButtonDown(0))
				{
					roadEndPos = InputManager.GetWorldPositionOnMouseClick();
					Debug.Log($"roadEndPos: {roadEndPos}");
					roadPlacementState = RoadPlacementState.WaitingForFirstClick;
					listenForRoad = false;

					OnRoadPlaced?.Invoke(roadStartPos, roadEndPos);
					gridOutline.SetActive(false);
				}
				break;
		}
	}


	// These methods will be attached to the onClick on various buttons

	public void PlaceRoad()
	{
		listenForRoad = true;
		gridOutline.SetActive(true);
	}

	public void PlaceCurveRoad()
	{
		HoverBuilding(BuildingType.RoadCurve, 0);
	}

	public void PlaceIntersectionT()
	{
		HoverBuilding(BuildingType.RoadIntersectionT, 0);
	}

	public void PlaceIntersectionCross()
	{
		HoverBuilding(BuildingType.RoadIntersectionCross, 0);
	}

	public void PlacePowerBuildingLevelOne()
	{
		Debug.Log("DashboardManager: PlacePowerLevelOne() called");
		HoverBuilding(BuildingType.Utility, 0, UtilityType.Power);
		canvas.SetActive(false);
	}

	public void PlacePowerBuildingLevelTwo()
	{
		HoverBuilding(BuildingType.Utility, 1, UtilityType.Power);
	}

	public void PlacePowerBuildingLevelThree()
	{
		HoverBuilding(BuildingType.Utility, 2, UtilityType.Power);
	}

	public void PlacePowerBuildingLevelFour()
	{
		HoverBuilding(BuildingType.Utility, 3, UtilityType.Power);
	}

	public void PlaceWaterBuildingLevelOne()
	{
		HoverBuilding(BuildingType.Utility, 0, UtilityType.Water);
	}

	public void PlaceWaterBuildingLevelTwo()
	{
		HoverBuilding(BuildingType.Utility, 2, UtilityType.Water);
	}

	public void PlacePowerDistribution()
	{
		HoverBuilding(BuildingType.UtilityDistribution, 0, UtilityType.Power);
	}

	public void PlaceWaterDistribution()
	{
		HoverBuilding(BuildingType.UtilityDistribution, 0, UtilityType.Water);
	}

	public void PlaceSchool()
	{
		HoverServiceBuilding(ServiceType.School);
	}

	public void PlaceHospital()
	{
		HoverServiceBuilding(ServiceType.Hospital);
	}

	public void PlaceFireStation()
	{
		HoverServiceBuilding(ServiceType.FireStation);
	}

	public void PlacePoliceStation()
	{
		HoverServiceBuilding(ServiceType.PoliceStation);
	}
}