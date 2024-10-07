using System;
using UnityEngine;

[Serializable]
public class Building
{
	public BuildingType type;
	public string id;
	public byte level;
	public int capacity;
	public int maintenanceCost;
	public Vector3 pos;
	public Quaternion rotation;
	public string prefabId;

	public Building()
	{
		this.id = Guid.NewGuid().ToString();
		level = 0;
	}
	public override string ToString()
	{
		return $"ID: {id}, Level: {level}, Capacity: {capacity}, Maintenance Cost: {maintenanceCost}, Position: {pos}, Rotation: {rotation}, Prefab ID: {prefabId}";
	}
}
