using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
  private List<Vector3> path;
  private float speed;
  private int currentTargetIndex;

  public void Initialize(List<Vector3> path, float speed)
  {
    this.path = path;
    this.speed = speed;
    currentTargetIndex = 0;
  }

  public void StartMoving()
  {
    if (path == null || path.Count == 0)
    {
      Debug.LogError("Move: Path is null or empty.");
      return;
    }

    // Start the movement coroutine
    StartCoroutine(MoveAlongPath());
  }

  private IEnumerator<WaitForSeconds> MoveAlongPath()
  {
    while (currentTargetIndex < path.Count)
    {
      Vector3 targetPosition = path[currentTargetIndex];
      while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
      {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        yield return null;
      }

      currentTargetIndex++;
    }

    // Movement finished
    Debug.Log("Move: Reached the end of the path.");
    Destroy(gameObject);
  }
}