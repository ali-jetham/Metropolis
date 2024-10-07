using System.Collections.Generic;
// using Buildings;
using UnityEngine;

public enum UtilityType
{
    None,
    Water,
    Power
}

[System.Serializable]
public class UtilityBuilding : Building
{
    public UtilityType utilityType;

    /// <summary>
    /// This tuple holds all the fields/properties of the UtilityBuilding.
    /// </summary>
    public (int capacity, int maintenanceCost, int upgradeCost) properties;
    public int capacityUsed;
    public int availableCapacity;

    public UtilityBuilding(UtilityType type, byte level, Vector3 pos)
    {
        this.utilityType = type;
        this.level = level;
        this.pos = pos;
        Initialize();
        availableCapacity = Utility.powerLevelTable[level].maintenanceCost;
        maintenanceCost = Utility.powerLevelTable[level].maintenanceCost;
    }

    public void Initialize()
    {
        if (utilityType == UtilityType.Power)
            UpdatePowerProperties();

        if (utilityType == UtilityType.Water)
        {
            UpdateWaterProperties();
        }
    }

    /// <summary>
    /// Ensures that capacity is available before adding new UtilityDistribution.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool HasCapacity(int amount)
    {
        if (availableCapacity >= amount)
        {
            capacityUsed += amount;
            availableCapacity -= amount;
            return true;
        }
        else
        {
            Debug.LogError("UtilityBuilding: Not enough capacity available");
            return false;
        }
    }

    private void UpdatePowerProperties()
    {
        switch (level)
        {
            case 0:
                properties = Utility.powerLevelTable[0];
                break;
            case 1:
                properties = Utility.powerLevelTable[1];
                break;
            case 2:
                properties = Utility.powerLevelTable[2];
                break;
            case 3:
                properties = Utility.powerLevelTable[3];
                break;
            default:
                Debug.LogError("UtilityBuilding: Invalid level for UtilityType.Power");
                break;
        }
    }

    private void UpdateWaterProperties()
    {
        switch (level)
        {
            case 0:
                properties = Utility.waterLevelTable[0];
                break;
            case 1:
                properties = Utility.waterLevelTable[1];
                break;
            default:
                Debug.LogError("UtilityBuilding: Invalid level for UtilityType.Water");
                break;
        }
    }
}