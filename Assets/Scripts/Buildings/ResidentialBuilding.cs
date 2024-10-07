using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResidentialBuilding : Building
{
	string name;
	[NonSerialized] public List<Citizen> residents = new();

	public ResidentialBuilding(Vector3 pos, Quaternion rotation) : base()
	{
		this.pos = pos;
		this.rotation = rotation;
		Upgrade();
	}

	public void Upgrade()
	{
		switch (level)
		{
			case 0:
				(capacity, maintenanceCost) = Utility.residentialBuildingLevels[0];
				break;
			case 1:
				(capacity, maintenanceCost) = Utility.residentialBuildingLevels[1];
				break;
			case 2:
				(capacity, maintenanceCost) = Utility.residentialBuildingLevels[2];
				break;
			case 3:
				(capacity, maintenanceCost) = Utility.residentialBuildingLevels[3];
				break;
			case 4:
				(capacity, maintenanceCost) = Utility.residentialBuildingLevels[4];
				break;
		}
	}

	public override string ToString()
	{
		return base.ToString() + $", residents: {residents.Count}";
	}
}