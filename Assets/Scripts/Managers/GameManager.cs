using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control the flow of the game and the interaction between the different components.
/// Also acts a "singleton" and makes sure that there is only one instance of GridMap, City, etc.
/// </summary>
public class GameManager : MonoBehaviour
{
    GridMap gridMap;
    City city;

    [SerializeField] InputManager inputManager;
    [SerializeField] DashboardManager dashboardManager;
    [SerializeField] TMP_Text treasuryText;
    [SerializeField] TMP_Text populationText;


    void Start()
    {
        gridMap = new GridMap();
        city = new(gridMap);

        SetupEventSubscriptions();
        gridMap.Initialize();
        city.Initialize();

        TimeManager.Instance.OnHourPassed += HandleHourPassed;
        if (gridMap.cells.Length == 2601)
            Debug.Log("GameManager: gridMap.cells length is 2601");

    }

    void Update() { }

    /// <summary>
    /// Actions to be performed when closing the game.
    /// </summary>
    private void OnApplicationQuit()
    {
        // gridMap.SaveGridMap();
        // city.SaveCity();
        city.UnsubscribeFromEvents();
    }


    /// <summary>
    /// Set up various event subscription.
    /// </summary>
    private void SetupEventSubscriptions()
    {
        // Reference to InputManager
        inputManager = GetComponent<InputManager>();
        if (inputManager != null)
        {
            inputManager.onZoneInput.AddListener(HandleZoneInput);
        }

        // city event subscription
        city.OnCreateZone += gridMap.OnCreateZoneHandler;
        city.OnUtilityBuildingPlaced += gridMap.OnBuildingPlacedHandler;
        city.OnUtilityDistributionPlaced += gridMap.OnBuildingPlacedHandler;
        city.OnServiceBuildingPlacement += gridMap.OnBuildingPlacedHandler;
        city.OnRoadPlaced += gridMap.OnRoadBuiltHandler;
        city.OnRoadStructurePlaced += gridMap.OnBuildingPlacedHandler;
        city.OnReceiveZoneData += dashboardManager.HandleReceiveZoneData;
        city.SubscribeToEvents();

        // dashboard event subscription
        if (dashboardManager != null)
        {
            dashboardManager.OnUtilityBuildingPlaced += city.HandleUtilityPlacement;
            dashboardManager.OnUtilityDistributionPlaced += city.HandleUtilityDistributionPlaced;
            dashboardManager.OnServicePlaced += city.HandleServicePlacement;
            dashboardManager.OnRoadPlaced += city.HandleRoadPlaced;
            dashboardManager.OnRoadStructurePlaced += city.HandleRoadStructurePlaced;
            dashboardManager.OnResidentialZoneClicked += inputManager.HandleResidentialZoneCreation;
            dashboardManager.OnCommercialZoneClicked += inputManager.HandleCommercialZoneCreation;
            dashboardManager.OnIndustrialZoneClicked += inputManager.HandleIndustrialZoneCreation;
            dashboardManager.OnRequestZoneData += city.HandleRequestZoneData;
            dashboardManager.OnHighlightZone += city.HandleHighlightZone;
            dashboardManager.OnUtilityDistMonitorButtonClicked += city.HighlightUtilityDistributions;
            dashboardManager.OnServiceDistMonitorButtonClicked += city.HighlightServiceDistributions;
        }
    }

    public void SubscribeUtilityDistribution(UtilityDistribution utilityDistribution)
    {
        utilityDistribution.OnUtilityDistribute += gridMap.OnUtilityDistributeHandler;
        Debug.Log("GameManager: called SubscribeUtilityDistribution()");
    }

    public void SubscribeServiceDistribution(ServiceBuilding serviceBuilding)
    {
        serviceBuilding.OnServiceDistribute += gridMap.OnServiceDistributeHandler;
        Debug.Log("GameManager: SubscribeServiceDistribution() called");
    }

    void HandleZoneInput(ZoneData zoneData)
    {
        city.CreateZone(zoneData, gridMap);
    }


    private void UpdateCityTreasuryDisplay(float treasury)
    {
        Debug.Log($"GameManager: Updated City Treasury: {treasury}");
        try
        {
            MainThreadDispatcher.Enqueue(() =>
            {
                treasuryText.SetText($"${treasury}");
                populationText.SetText($"Population: {city.population.citizens.Count}");
            });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private void HandleEndOfMonth()
    {
        Debug.Log("GameManager: Month has passed, economy updated.");
    }

    private void HandleHourPassed()
    {
        Debug.Log("GameManager: Hour has passed, economy updated.");
        city.UpdateEconomy();
        UpdateCityTreasuryDisplay(city.economy.treasury);
    }

    private void HandleDayPassed()
    {
        city.UpdateEconomy();
        UpdateCityTreasuryDisplay(city.economy.Treasury);
        Debug.Log("GameManager: Day has passed, economy updated.");
    }
}
