using UnityEngine;

/// <summary>
/// Enum defined to differentiate between RoadStructure
/// </summary>
public enum RoadStructureType
{
  Curve,
  IntersectionT,
  IntersectionCross
}

/// <summary>
/// Base class used for RoadCurve, RoadIntersectionT and RoadIntersectionCross 
/// </summary>
[System.Serializable]
public class RoadStructure : Building
{
  public RoadStructureType roadStructureType;

  public RoadStructure(RoadStructureType type, Vector3 pos, Quaternion rotation) : base()
  {
    this.pos = pos;
    this.roadStructureType = type;
    this.rotation = rotation;
  }
}