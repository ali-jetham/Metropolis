using System;
using System.Collections.Generic;
using UnityEngine;

public class CitizenAnimator : MonoBehaviour
{
  public void TransportCitizen(Citizen citizen, List<Vector3> path)
  {
    Debug.Log("CitizenAnimator: TransportCitizen() called");
    GameObject prefab = null;
    System.Random random = new();
    float offsetX = (float)(random.NextDouble() * 2 - 1); // Random value between -1 and 1
    float offsetY = 0; // Assuming you don't want to change the y position
    float offsetZ = (float)(random.NextDouble() * 2 - 1); // Random value between -1 and 1
    // Vector3 randomPosition = path[0] + new Vector3(offsetX, offsetY, offsetZ);

    try
    {
      MainThreadDispatcher.Enqueue(() =>
      {
        prefab = Instantiate(Utility.citizenPrefabs[random.Next(2)], path[0], Quaternion.identity);
        var moveComponent = prefab.AddComponent<Move>();
        moveComponent.Initialize(path, 1);

        // Add a CitizenAnimator component to the prefab
        var citizenAnimator = prefab.AddComponent<CitizenAnimator>();
        citizenAnimator.StartTransport(moveComponent);
      });
    }
    catch (Exception e)
    {
      Debug.LogError(e.Message);
      Debug.LogError(e.StackTrace);
    }
  }

  private void StartTransport(Move moveComponent)
  {
    // Logic to start the transport animation for the citizen
    moveComponent.StartMoving();
  }
}