using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zones;

/// <summary>
/// Handle tasks related to population like, new citizens moving in, citizens jobs, 
/// citizens being born, citizens dying, etc.
/// </summary>
[System.Serializable]
public class Population
{
  // Fields
  public List<Citizen> citizens = new();
  private GridMap gridMap;

  // Events
  public event Action<List<Citizen>, ResidentialBuilding> OnAddCitizenToBuilding;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="gridMap"></param>
  public Population(GridMap gridMap)
  {
    this.gridMap = gridMap;
    TimeManager.Instance.On10MinutePassed += SimulatePedestrianTraffic;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="zone"></param>
  public void AddCitizens(ResidentialZone zone)
  {
    System.Random random = new();
    float zqi = zone.zqi.GetZqiResidential();

    ResidentialBuilding building = zone.buildings[random.Next(zone.buildings.Count)];
    if (building.residents.Count == building.capacity)
    {
      return;
    }

    List<Citizen> newCitizens = GenerateCitizens(building.capacity, zqi);
    foreach (var citizen in newCitizens)
    {
      citizen.residence = building;
    }
    building.residents = newCitizens;
    citizens.AddRange(newCitizens);
    OnAddCitizenToBuilding?.Invoke(newCitizens, building);
  }

  public void HandleHourPassed()
  {
    Debug.Log("Population: HandleHourPassed() called");
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="totalCount"></param>
  /// <returns></returns>
  private List<Citizen> GenerateCitizens(int totalCount, float zqi)
  {
    Debug.Log($"Population: Generating {totalCount} citizens");
    System.Random random = new();
    List<Citizen> newCitizens = new();
    int childrenCount = 0;
    int adultsCount = 0;
    int seniorsCount = 0;
    switch (totalCount)
    {
      case 4:
        childrenCount = 2;
        adultsCount = 2;
        break;
      case 6:
        childrenCount = 2;
        adultsCount = 2;
        seniorsCount = 2;
        break;
      case 12:
        adultsCount = 4;
        childrenCount = 6;
        seniorsCount = 2;
        break;
      case 160:
        adultsCount = 100;
        childrenCount = 40;
        seniorsCount = 20;
        break;
      case 1000:
        adultsCount = 800;
        childrenCount = 150;
        seniorsCount = 50;
        break;
    }
    newCitizens.AddRange(GenerateChildren(childrenCount, zqi));
    newCitizens.AddRange(GenerateAdults(adultsCount, zqi));
    newCitizens.AddRange(GenerateSeniors(seniorsCount, zqi));

    if (newCitizens.Count == totalCount)
    {
      Debug.Log("Population: newCitizens.count is equal to totalCount");
      return newCitizens;
    }
    Debug.LogError($"Population: newCitizens.Count: {newCitizens.Count}, totalCount: {totalCount}");
    return newCitizens;
  }

  private List<Citizen> GenerateChildren(int count, float zqi)
  {
    System.Random random = new();
    List<Citizen> children = new();

    for (int i = 0; i < count; i++)
    {
      Citizen citizen = new()
      {
        age = (byte)random.Next(0, 18),
        education = DetermineEducation(zqi, random),
        health = (byte)random.Next(70, 100)
      };
      citizen.Initialize();
      citizen.OnWorkCommute += gridMap.FindPath;
      children.Add(citizen);
    }

    Debug.Log($"Population: Generated children: {children.Count}");
    return children;
  }

  private List<Citizen> GenerateAdults(int count, float zqi)
  {
    System.Random random = new();
    List<Citizen> adults = new();

    for (int i = 0; i < count; i++)
    {
      Citizen citizen = new()
      {
        age = (byte)random.Next(18, 65),
        education = DetermineEducation(zqi, random),
        health = (byte)random.Next(50, 100)
      };
      citizen.Initialize();
      citizen.OnWorkCommute += gridMap.FindPath;
      adults.Add(citizen);
    }
    Debug.Log($"Population: Generated adults: {adults.Count}");
    return adults;
  }

  private List<Citizen> GenerateSeniors(int count, float zqi)
  {
    System.Random random = new();
    List<Citizen> seniors = new();

    for (int i = 0; i < count; i++)
    {
      Citizen citizen = new()
      {
        age = (byte)random.Next(65, 100),
        education = DetermineEducation(zqi, random),
        health = (byte)random.Next(50, 100)
      };
      citizen.Initialize();
      citizen.OnWorkCommute += gridMap.FindPath;
      seniors.Add(citizen);
    }

    return seniors;
  }


  /// <summary>
  /// 
  /// </summary>
  /// <param name="zqi"></param>
  /// <param name="random"></param>
  /// <returns></returns>
  private Education DetermineEducation(float zqi, System.Random random)
  {
    if (zqi >= 80)
    {
      double rand = random.NextDouble();
      if (rand < 0.5)
        return Education.Phd;
      else if (rand < 0.8)
        return Education.PostGraduate;
      else
        return Education.Graduate;
    }
    else if (zqi >= 60)
    {
      double rand = random.NextDouble();
      if (rand < 0.5)
        return Education.Higher;
      else if (rand < 0.8)
        return Education.Graduate;
      else
        return Education.Secondary;
    }
    else if (zqi >= 40)
    {
      return random.NextDouble() < 0.7 ? Education.Secondary : Education.Primary;
    }
    else if (zqi >= 20)
    {
      return random.NextDouble() < 0.5 ? Education.Secondary : Education.Primary;
    }
    else
    {
      return random.NextDouble() < 0.5 ? Education.Primary : Education.None;
    }
  }


  /// <summary>
  /// 
  /// </summary>
  /// <param name="zone"></param>
  public void HandleAddCitizenRequest(ResidentialZone zone)
  {
    AddCitizens(zone);
  }

  public void SimulatePedestrianTraffic()
  {
    Debug.Log("Population: SimulatePedestrianTraffic() called");
    System.Random random = new();
    Vector3 startPos = gridMap.GetRandomRoadPos();
    if (startPos == null)
      return;

    Vector3 endPos = gridMap.GetRandomRoadPos();
    if (endPos == startPos || endPos == null)
    {
      return;
    }

    Debug.Log($"Population: startPos is {startPos} and endPos is {endPos}");

    Citizen citizen = citizens[random.Next(citizens.Count)];
    List<Vector3> path = gridMap.FindPath(startPos, endPos);

    if (path != null && path.Count > 0)
    {
      Debug.Log($"Citizen: path is not null, TransportCitizen() called");
      try
      {
        MainThreadDispatcher.CollectRequest(citizen, path);
      }
      catch (Exception e)
      {
        Debug.LogError(e.Message);
      }
    }
    else
    {
      Debug.Log($"Citizen: path is Null or path.Count:{path.Count}");
    }
  }
}