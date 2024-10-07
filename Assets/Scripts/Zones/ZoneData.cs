using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// struct that is passed by InputManager.OnZoneInput
/// </summary>

[System.Serializable]
public struct ZoneData
{
    public enum Type
    {
        Residential,
        Commercial,
        Industrial
    }

    public Type type;
    public Vector3 startPos;
    public Vector3 endPos;
    // string name;

    public ZoneData(Type type, Vector3 startPos, Vector3 endPos)
    {
        this.type = type;
        this.startPos = startPos;
        this.endPos = endPos;
        // this.name = Utility.GenerateZoneName();
    }

    public override string ToString()
    {
        return $"Type:{type}, startPos: {startPos}, endPos: {endPos}";
    }

}
