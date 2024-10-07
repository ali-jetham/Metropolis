using System;
using System.Collections.Generic;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
  private static readonly Queue<Action> _executionQueue = new Queue<Action>();
  private static readonly List<(Citizen, List<Vector3>)> requestBatch = new List<(Citizen, List<Vector3>)>();

  private void Update()
  {
    lock (_executionQueue)
    {
      while (_executionQueue.Count > 0)
      {
        _executionQueue.Dequeue().Invoke();
      }
    }

    ProcessBatchRequests();
  }

  public static void Enqueue(Action action)
  {
    if (action == null)
    {
      throw new ArgumentNullException(nameof(action));
    }

    lock (_executionQueue)
    {
      _executionQueue.Enqueue(action);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="citizen"></param>
  /// <param name="path"></param>
  public static void CollectRequest(Citizen citizen, List<Vector3> path)
  {
    lock (requestBatch)
    {
      requestBatch.Add((citizen, path));
    }
  }

  public static void ProcessBatchRequests()
  {
    lock (requestBatch)
    {
      foreach (var (citizen, path) in requestBatch)
      {
        Enqueue(() =>
        {
          GameObject citizenObject = new("CitizenAnimator");
          CitizenAnimator citizenAnimator = citizenObject.AddComponent<CitizenAnimator>();
          citizenAnimator.TransportCitizen(citizen, path);
        });
      }
      requestBatch.Clear();
    }
  }
}