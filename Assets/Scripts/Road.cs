using System.Collections.Generic;
using UnityEngine;


public enum RoadDirection
{
	None,
	Top,
	Bottom,
	Left,
	Right
}
[System.Serializable]
public class Road : Building
{
	public Vector3 startPos;
	public Vector3 endPos;

	public Road(Vector3 startPos, Vector3 endPos)
	{
		this.startPos = startPos;
		this.endPos = endPos;
		this.rotation = Quaternion.identity;
	}


	/// <summary>
	/// Uses the startPos and endPos to get the cells that should be counted in the road stretch.
	/// </summary>
	public List<Vector3> GetCells()
	{
		int startX = (int)startPos.x;
		int startY = (int)startPos.z;
		int endX = (int)endPos.x;
		int endY = (int)endPos.z;

		// a list of vector3 positions of cells that should be made road
		List<Vector3> cellsToBeMadeRoad = new();
		Vector3 pos;

		if (startX == endX)
		{
			// Vertical road
			Debug.Log("Road: Vertical road");
			if (startY > endY)
			{
				(endY, startY) = (startY, endY);
				Debug.Log($"startY: {startY}, endY: {endY}");
			}


			for (int i = startY; i <= endY; i += 10)
			{
				pos = new Vector3(startX, 0, i);
				cellsToBeMadeRoad.Add(pos);
			}
		}
		else if (startY == endY)
		{
			// Horizontal road
			Debug.Log("Road: Horizontal road");
			rotation = Quaternion.Euler(0, 90, 0);

			if (startX > endX)
			{
				(endX, startX) = (startX, endX);
			}

			for (int i = startX; i <= endX; i += 10)
			{
				pos = new Vector3(i, 0, startY);
				cellsToBeMadeRoad.Add(pos);
			}
		}

		foreach (var cellPos in cellsToBeMadeRoad)
		{
			Debug.Log($"Road: {cellPos}");
		}
		return cellsToBeMadeRoad;
	}

	public override string ToString()
	{
		return $"startPos is {startPos} and endPos is{endPos}";
	}
}