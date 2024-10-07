using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zones;

public static class Utility
{
    //
    public static bool isDashboardActive = false;
    public static bool isHoverBuilding = false;

    public static Vector3 Round(Vector3 vector)
    {
        return new Vector3(
            RoundToNearestTenOrZero(vector.x),
            RoundToNearestTenOrZero(vector.y),
            RoundToNearestTenOrZero(vector.z));
    }

    // NOTE: This method does not work with negative values.
    private static float RoundToNearestTenOrZero(float value)
    {
        if (value < 5)
            return 0;
        return Mathf.Round(value / 10) * 10;
    }


    /// <summary>
    /// 
    /// </summary>
    private static ushort entityIdCounter = 0;
    public static ushort GenerateEntityId()
    {
        ushort entityId = entityIdCounter;
        entityIdCounter++;
        return entityId;
    }


    private static string[] prefixes = {"San","Las","Los","New","Fort","Santa","Maha","Amster","Bora","Rio",
    "Port",
    "Mount",
    "Lake",
    "El",
    "Saint"
};

    private static string[] suffixes = {
    "Diego",
    "Vegas",
    "Angeles",
    "York",
    "Worth",
    "Antonio",
    "Rashtra",
    "Dam",
    "Bora",
    "Janeiro",
    "Harbor",
    "Hill",
    "Ville",
    "Creek",
    "Bay"
};

    public static string GenerateZoneName()
    {
        string randomPrefix = prefixes[Random.Range(0, prefixes.Length)];
        string randomSuffix = suffixes[Random.Range(0, suffixes.Length)];

        return randomPrefix + " " + randomSuffix;
    }


    /// <summary>
    /// static arrays for loading prefabs
    /// </summary>
    public static Dictionary<string, GameObject> residentialPrefabs = new();
    public static Dictionary<string, GameObject> commercialPrefabs = new();
    public static Dictionary<string, GameObject> industrialPrefabs = new();
    public static Dictionary<string, GameObject> utilityPrefabs = new();
    public static Dictionary<string, GameObject> UtilityDistPrefabs = new();
    // public static Dictionary<string, GameObject> servicePrefabs = new();
    public static Dictionary<string, GameObject> roadPrefabs = new();

    public static GameObject[] residentialLevelZeroPrefabs;
    public static GameObject[] residentialLevelOnePrefabs;
    public static GameObject[] residentialLevelTwoPrefabs;
    public static GameObject[] residentialLevelThreePrefabs;
    public static GameObject[] residentialLevelFourPrefabs;

    public static GameObject[] commercialLevelZeroPrefabs;
    public static GameObject[] commercialLevelOnePrefabs;
    public static GameObject[] commercialLevelTwoPrefabs;
    public static GameObject[] commercialLevelThreePrefabs;
    public static GameObject[] commercialLevelFourPrefabs;

    public static GameObject[] industrialLevelZeroPrefabs;
    public static GameObject[] industrialLevelOnePrefabs;
    public static GameObject[] industrialLevelTwoPrefabs;
    public static GameObject[] industrialLevelThreePrefabs;
    // public static GameObject[] industrialLevelFourPrefabs;

    public static GameObject[] utilityPowerPrefabs = new GameObject[4];
    public static GameObject[] utilityWaterPrefabs;

    public static GameObject[] utilityPowerDistPrefabs;
    public static GameObject[] utilityWaterDistPrefabs;

    public static GameObject roadStraight;
    public static GameObject roadIntersectionCross;
    public static GameObject roadIntersectionT;
    public static GameObject roadCurve;

    public static GameObject hospitalPrefab;
    public static GameObject fireStationPrefab;
    public static GameObject policeStationPrefab;
    public static GameObject schoolPrefab;

    public static GameObject[] citizenPrefabs;
    public static GameObject residentialHighlightPlane;
    public static GameObject commercialHighlightPlane;
    public static GameObject industrialHighlightPlane;

    public static GameObject powerDistributionHighlightPlane;
    public static GameObject waterDistributionHighlightPlane;
    public static GameObject hospitalHighlightPlane;
    public static GameObject fireStationHighlightPlane;
    public static GameObject policeStationHighlightPlane;
    public static GameObject schoolHighlightPlane;

    /// <summary>
    /// Static Lists for storing level data of different buildings. Think of this like a lookup table
    /// from where the values for the levels of buildings will be retrieved.
    /// </summary>

    public static List<(int capacity, int maintenanceCost, int upgradeCost)> powerLevelTable = new()
    {
        (700, 1000, 15000),
        (400000, 10, 50),
        (15000, 10, 10),
        (800000, 10, 10)
    };

    public static List<(int capacity, int maintenanceCost, int upgradeCost)> waterLevelTable = new()
    {
        (30000, 1000, 15000),
        (60000, 10, 50)
    };

    public static List<(int capacity, int maintenanceCost)> residentialBuildingLevels = new()
    {
        (4, 100),
        (6, 250),
        (12, 350),
        (160, 400),
        (1000, 1000),
    };

    static Utility()
    {
        residentialLevelZeroPrefabs = Resources.LoadAll<GameObject>("Residential/levelZero");
        residentialLevelOnePrefabs = Resources.LoadAll<GameObject>("Residential/levelOne");
        residentialLevelTwoPrefabs = Resources.LoadAll<GameObject>("Residential/levelTwo");
        residentialLevelThreePrefabs = Resources.LoadAll<GameObject>("Residential/levelThree");
        residentialLevelFourPrefabs = Resources.LoadAll<GameObject>("Residential/levelFour");

        commercialLevelZeroPrefabs = Resources.LoadAll<GameObject>("Commercial/levelZero");
        commercialLevelOnePrefabs = Resources.LoadAll<GameObject>("Commercial/levelOne");
        commercialLevelTwoPrefabs = Resources.LoadAll<GameObject>("Commercial/levelTwo");
        commercialLevelThreePrefabs = Resources.LoadAll<GameObject>("Commercial/levelThree");

        industrialLevelZeroPrefabs = Resources.LoadAll<GameObject>("Industrial/levelZero");
        industrialLevelOnePrefabs = Resources.LoadAll<GameObject>("Industrial/levelOne");
        industrialLevelTwoPrefabs = Resources.LoadAll<GameObject>("Industrial/levelTwo");
        industrialLevelThreePrefabs = Resources.LoadAll<GameObject>("Industrial/levelThree");

        utilityPowerPrefabs = Resources.LoadAll<GameObject>("Utilities/Power");
        utilityWaterPrefabs = Resources.LoadAll<GameObject>("Utilities/Water");

        utilityPowerDistPrefabs = Resources.LoadAll<GameObject>("UtilityDistribution/Power");
        utilityWaterDistPrefabs = Resources.LoadAll<GameObject>("UtilityDistribution/Water");

        roadStraight = Resources.Load<GameObject>("Roads/road-straight-m");
        roadIntersectionCross = Resources.Load<GameObject>("Roads/intersection-m");
        roadIntersectionT = Resources.Load<GameObject>("Roads/t-intersection-m");
        roadCurve = Resources.Load<GameObject>("Roads/curve-m");

        hospitalPrefab = Resources.Load<GameObject>("Services/Hospital");
        fireStationPrefab = Resources.Load<GameObject>("Services/FireStation");
        policeStationPrefab = Resources.Load<GameObject>("Services/PoliceStation");
        schoolPrefab = Resources.Load<GameObject>("Services/School");

        citizenPrefabs = Resources.LoadAll<GameObject>("Citizens");
        residentialHighlightPlane = Resources.Load<GameObject>("HighlightCubes/resHighlightCube");
        commercialHighlightPlane = Resources.Load<GameObject>("HighlightCubes/comHighlightCube");
        industrialHighlightPlane = Resources.Load<GameObject>("HighLightCubes/indHighlightCube");

        powerDistributionHighlightPlane = Resources.Load<GameObject>("HighlightCubes/utilityPowerHighlightCube");
        waterDistributionHighlightPlane = Resources.Load<GameObject>("HighlightCubes/resHighlightCube");

        hospitalHighlightPlane = Resources.Load<GameObject>("HighlightCubes/comHighlightCube");
        fireStationHighlightPlane = Resources.Load<GameObject>("HighlightCubes/comHighlightCube");
        policeStationHighlightPlane = Resources.Load<GameObject>("HighlightCubes/comHighlightCube");
        schoolHighlightPlane = Resources.Load<GameObject>("HighlightCubes/comHighlightCube");

        PopulatePrefabDictionary(residentialPrefabs, residentialLevelZeroPrefabs);
        PopulatePrefabDictionary(residentialPrefabs, residentialLevelOnePrefabs);
        PopulatePrefabDictionary(residentialPrefabs, residentialLevelTwoPrefabs);
        PopulatePrefabDictionary(residentialPrefabs, residentialLevelThreePrefabs);
        PopulatePrefabDictionary(residentialPrefabs, residentialLevelFourPrefabs);

        PopulatePrefabDictionary(commercialPrefabs, commercialLevelZeroPrefabs);
        PopulatePrefabDictionary(commercialPrefabs, commercialLevelOnePrefabs);
        PopulatePrefabDictionary(commercialPrefabs, commercialLevelTwoPrefabs);
        PopulatePrefabDictionary(commercialPrefabs, commercialLevelThreePrefabs);

        PopulatePrefabDictionary(industrialPrefabs, industrialLevelZeroPrefabs);
        PopulatePrefabDictionary(industrialPrefabs, industrialLevelOnePrefabs);
        PopulatePrefabDictionary(industrialPrefabs, industrialLevelTwoPrefabs);
        PopulatePrefabDictionary(industrialPrefabs, industrialLevelThreePrefabs);

        PopulatePrefabDictionary(utilityPrefabs, utilityPowerPrefabs);
        PopulatePrefabDictionary(utilityPrefabs, utilityWaterPrefabs);

        PopulatePrefabDictionary(UtilityDistPrefabs, utilityPowerDistPrefabs);
        PopulatePrefabDictionary(UtilityDistPrefabs, utilityWaterDistPrefabs);
    }

    /// <summary>
    /// Populates the dictionary with the GameObjects of the list passed, 
    /// mapped to the GameObject.name 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="prefabs"></param>
    private static void PopulatePrefabDictionary(Dictionary<string, GameObject> dictionary, GameObject[] prefabs)
    {
        foreach (var prefab in prefabs)
        {
            dictionary[prefab.name] = prefab;
            Debug.Log($"Added key: {prefab.name} to {prefab}");
        }
    }
}

