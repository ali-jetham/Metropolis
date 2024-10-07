using System;
using System.Collections.Generic;
// using Buildings;
using UnityEngine;

[System.Serializable]
public class CommercialBuilding : Building
{

    string name;
    [NonSerialized] public List<Citizen> employees = new();

    public CommercialBuilding(Vector3 pos, Quaternion rotation) : base()
    {
        this.pos = pos;
        this.rotation = rotation;
    }

    public void Upgrade()
    {
        switch (level)
        {
            case 0:
                (capacity, maintenanceCost) = Utility.residentialBuildingLevels[0];
                break;
            case 1:
                (capacity, maintenanceCost) = Utility.residentialBuildingLevels[1];
                break;
            case 2:
                (capacity, maintenanceCost) = Utility.residentialBuildingLevels[2];
                break;
            case 3:
                (capacity, maintenanceCost) = Utility.residentialBuildingLevels[3];
                break;
            case 4:
                (capacity, maintenanceCost) = Utility.residentialBuildingLevels[4];
                break;
        }
    }
}