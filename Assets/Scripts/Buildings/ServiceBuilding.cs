using System;
using System.Collections.Generic;
using UnityEngine;
// using Buildings;

public enum ServiceType
{
  None,
  Hospital,
  PoliceStation,
  FireStation,
  School
}

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class ServiceBuilding : Building
{
  public ServiceType serviceType;
  public readonly byte radius = 100;

  // Events
  public event Action<ServiceType, List<Vector3>> OnServiceDistribute;

  // Constructor
  public ServiceBuilding(ServiceType type, Vector3 pos)
  {
    this.pos = pos;
    this.serviceType = type;
    this.maintenanceCost = 500;

    // Subscribe to the event in GameManager
    GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
      gameManager.SubscribeServiceDistribution(this);
      Debug.Log("ServiceDistribution: Subscribed to GameManager");
    }
    else
    { Debug.LogError("ServiceDistribution: GameManager not found, subscription failed"); }

    Distribute();
  }

  /// <summary>
  /// Calculates and distributes service coverage to cells within the radius.
  /// </summary>
  public void Distribute()
  {
    const int MAX_X = 2000;
    const int MAX_Z = 2000;

    Vector3 startPos = new(Mathf.Clamp(pos.x - radius, 0, MAX_X), 0, Mathf.Clamp(pos.z - radius, 0, MAX_Z));
    Vector3 endPos = new(Mathf.Clamp(pos.x + radius, 0, MAX_X), 0, Mathf.Clamp(pos.z + radius, 0, MAX_Z));
    List<Vector3> cellsAffected = new();

    int startX = (int)startPos.x;
    int startY = (int)startPos.z;
    int endX = (int)endPos.x;
    int endY = (int)endPos.z;

    Debug.Log($"startX: {startX}, endX: {endX}");
    Debug.Log($"startY: {startY}, endY: {endY}");

    if (startX > endX)
      (endX, startX) = (startX, endX);

    if (startY > endY)
      (endY, startY) = (startY, endY);

    for (int x = startX; x <= endX; x += 10)
    {
      for (int y = startY; y <= endY; y += 10)
      {
        Vector3 cellPos = new(x, 0, y);
        if (Vector3.Distance(new Vector3(pos.x, 0, pos.z), cellPos) <= radius)
        {
          cellsAffected.Add(cellPos);
        }
      }
    }
    Debug.Log($"ServiceDistribution: Affected cells count: {cellsAffected.Count}");
    OnServiceDistribute?.Invoke(serviceType, cellsAffected);
  }
}