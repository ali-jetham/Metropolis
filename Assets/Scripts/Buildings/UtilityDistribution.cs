using System;
using System.Collections.Generic;
// using Buildings;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// "Connects" to a UtilityBuilding.
/// </summary
[System.Serializable]
public class UtilityDistribution : Building
{
  UtilityBuilding connectedUtilityBuilding;
  public UtilityType utilityType;
  byte radius = 50; // this radius sets the end bounds, using which cells inside the bounds are calculated

  // events
  public event Action<UtilityType, List<Vector3>> OnUtilityDistribute;


  /// <summary>
  /// Constructor for UtilityDistribution.
  /// </summary>
  /// <param name="utilityBuilding"></param>
  /// <exception cref="InvalidOperationException"></exception>
  public UtilityDistribution(UtilityBuilding utilityBuilding, Vector3 pos)
  {
    connectedUtilityBuilding = utilityBuilding;
    this.pos = pos;
    capacity = 400;
    utilityType = utilityBuilding.utilityType;
    Debug.Log($"UtilityDistribution: Constructor success, UtilityDistribution added at pos {pos}");


    // Subscribe to the event in GameManager
    GameManager gameManager = GameObject.FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
      gameManager.SubscribeUtilityDistribution(this);
      Debug.Log("UtilityDistribution: called gameManager to subscribe");
    }

    Distribute();
  }

  /// <summary>
  /// Makes a list of the cells that have access to a specific utility, this is then raised by
  /// events that sends the list to gridmap which finally updates it.
  /// </summary>
  /// <remarks>
  /// The way this algorithm works is by finding the two corners that will be covered by the radius
  /// and them simply loops over all the cells that ..
  /// </remarks>
  public void Distribute()
  {
    Vector3 startPos = new(Mathf.Clamp(pos.x - radius, 0, 2000), 0, Mathf.Clamp(pos.z - radius, 0, 2000));
    Vector3 endPos = new(Mathf.Clamp(pos.x + radius, 0, 2000), 0, Mathf.Clamp(pos.z + radius, 0, 2000));
    Vector3 cellPos;
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
        cellPos = new(x, 0, y);
        cellsAffected.Add(cellPos);
      }
    }

    Debug.Log("UtilityDistribution: loops completed, printing values now");
    foreach (var pos in cellsAffected)
    {
      Debug.Log(pos);
    }
    Debug.Log($"UtilityDistribution: cellsAffected length: {cellsAffected.Count}");

    OnUtilityDistribute?.Invoke(utilityType, cellsAffected);
  }
}