using System;
// using Buildings;
using UnityEngine;

[Serializable]
public class GridCell
{

    public Vector3 pos;
    public Zone zone;
    public string zoneId;
    public Building building;
    public Quaternion roadRotation;
    public string buildingId;
    public bool isOccupied, isRoad, hasPower, hasWater, hasRoad, hasSchool;
    public bool hasPoliceStation;
    public bool hasFireStation;
    public bool hasHospital;

    public GridCell(Vector3 pos)
    {
        this.pos = pos;
        this.zone = null;
        this.isOccupied = false;
        this.hasPower = false;
        this.hasWater = false;
        isRoad = hasRoad = false;
    }


    public override string ToString()
    {
        if (zone == null)
        {
            return $"Position: {pos}";
        }
        return $"Position: {pos.ToString()}\nzone: {zone.ToString()}";
    }
}
