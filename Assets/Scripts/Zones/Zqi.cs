using System.Collections.Generic;
using Zones;
using UnityEngine;

public enum ZqiType
{
  Water,
  Power,
  School,
  Hospital,
  Police,
  Fire,
  Literacy,
  Pollution,
  Jobs
}

/// <summary>
/// Zone Quality Index is used to handle the progression of Zone.
/// The higher the indexes the faster the zone progresses.
/// </summary>
// [System.Serializable]
public class Zqi
{
  public int maximumCellsCount;
  // public List<Citizen> citizensInZone;

  private ResidentialZone zone;
  private CommercialZone commercialZone;
  private IndustrialZone industrialZone;
  public int waterCoverage;
  public int powerCoverage;
  public float schoolCoverage;
  public int hospitalCoverage;
  public int policeCoverage;
  public int fireCoverage;
  public float recreationalCoverage;
  public float literacyRate;
  public float pollutionLevel;
  public float jobAvailability;

  public Zqi(ResidentialZone zone, int maxCells)
  {
    this.zone = zone;
    maximumCellsCount = maxCells;
    if (maximumCellsCount == 0)
    {
      Debug.LogError("ZQI: maximumCellsCount is zero");
      maximumCellsCount = 1;
    }
    waterCoverage = 0;
    powerCoverage = 0;
    schoolCoverage = 0;
    hospitalCoverage = 0;
    policeCoverage = 0;
    fireCoverage = 0;
    literacyRate = 0;
    jobAvailability = 0; // number of available jobs in the adjacent commercial and industrial zones
    pollutionLevel = 0; // 
    Debug.Log("ZQI: Constructor called");
  }

  public Zqi(CommercialZone commercialZone, int maxCells)
  {
    this.commercialZone = commercialZone;
    maximumCellsCount = maxCells;
    if (maximumCellsCount == 0)
    {
      Debug.LogError("ZQI: maximumCellsCount is zero");
      maximumCellsCount = 1;
    }
    waterCoverage = 0;
    powerCoverage = 0;
    schoolCoverage = 0;
    hospitalCoverage = 0;
    policeCoverage = 0;
    fireCoverage = 0;
    literacyRate = 0;
    jobAvailability = 0; // number of available jobs in the adjacent commercial and industrial zones
    pollutionLevel = 0;
    Debug.Log("ZQI: Commercial Constructor called");
  }

  public Zqi(IndustrialZone industrialZone, int maxCells)
  {
    this.industrialZone = industrialZone;
    maximumCellsCount = maxCells;
    if (maximumCellsCount == 0)
    {
      Debug.LogError("ZQI: maximumCellsCount is zero");
      maximumCellsCount = 1;
    }
    waterCoverage = 0;
    powerCoverage = 0;
    schoolCoverage = 0;
    hospitalCoverage = 0;
    policeCoverage = 0;
    fireCoverage = 0;
    literacyRate = 0;
    jobAvailability = 0; // number of available jobs in the adjacent commercial and industrial zones
    pollutionLevel = 0; // 
    Debug.Log("ZQI: Commercial Constructor called");
  }

  public float MaxCells => maximumCellsCount;

  /*
  Properties to calculate ratings on demand.
  Coverage field ratings are calculated using the following formula:
  Rating = (Current coverage / Maximum Coverage) * 10
  */
  public float WaterRating => waterCoverage / (float)maximumCellsCount * 10;
  public float PowerRating => powerCoverage / (float)maximumCellsCount * 10;
  public float SchoolRating => schoolCoverage / maximumCellsCount * 10;
  public float HospitalRating => hospitalCoverage / (float)maximumCellsCount * 10;
  public float PoliceRating => policeCoverage / (float)maximumCellsCount * 10;
  public float FireRating => fireCoverage / (float)maximumCellsCount * 10;
  public float RecreationalRating => recreationalCoverage / (float)maximumCellsCount * 10;

  public float LiteracyRating => NormalizeLiteracyLevel(CalculateAverageLiteracy());
  public float EmploymentRating =>
    GetJobsInAdjacentZones(zone.adjacentCommercialZones, zone.adjacentIndustrialZones) /
    CalculateJobSeekers(zone) * 10;
  // public float PollutionRating => 


  /// <summary>
  /// 
  /// </summary>
  /// <param name="commercialZones">List of adjacent commercial zones.</param>
  /// <param name="industrialZones">List of adjacent industrial zones.</param>
  /// <returns></returns>
  public int GetJobsInAdjacentZones(List<CommercialZone> commercialZones, List<IndustrialZone> industrialZones)
  {
    int jobs = 0;

    foreach (CommercialZone zone in commercialZones)
    {
      foreach (CommercialBuilding building in zone.buildings)
      {
        jobs += building.capacity - building.employees.Count;
      }
    }

    foreach (var zone in industrialZones)
    {
      foreach (var building in zone.buildings)
      {
        jobs += building.capacity - building.employees.Count;
      }
    }

    return jobs;
  }


  /// <summary>
  /// 
  /// </summary>
  /// <param name="zone"></param>
  /// <returns></returns>
  public int CalculateJobSeekers(ResidentialZone zone)
  {
    int jobsRequirement = 0;

    foreach (var building in zone.buildings)
    {
      if (building.residents == null)
        continue;

      foreach (Citizen citizen in building.residents)
      {
        if (citizen.cWorkplace == null && citizen.iWorkplace == null & citizen.age > 17)
          jobsRequirement++;
      }
    }
    return jobsRequirement;
  }



  public float CalculateAverageLiteracy()
  {
    float totalWeight = 0;
    foreach (var citizen in zone.citizensInZone)
    {
      totalWeight += (int)citizen.education;
    }
    return totalWeight / zone.citizensInZone.Count;
  }

  public float NormalizeLiteracyLevel(float averageLiteracyLevel)
  {
    return (averageLiteracyLevel / 6) * 10;
  }


  /// <summary>
  /// Calculate ZQI of a zone according to indexes related to Residential.
  /// Each field is rated on a scale of 10. Residential ZQI uses all of the 10 fields.
  /// </summary>
  /// <returns>ZQI of a zone</returns>
  public float GetZqiResidential()
  {
    return WaterRating + PowerRating + SchoolRating + HospitalRating + PoliceRating + FireRating;
    // + RecreationalRating;
  }

  /// <summary>
  /// Calculate ZQI of a zone according to indexes related to Commercial.
  /// </summary>
  /// <returns></returns>
  public float GetZqiCommercial()
  {
    // return 30f;
    return (WaterRating * 1.5f) + (PowerRating * 1.5f) + SchoolRating + PoliceRating + FireRating;
  }

  /// <summary>
  /// Calculate ZQI of a zone according to indexes related to Industrial.
  /// </summary>
  /// <returns></returns>
  public float GetZqiIndustrial()
  {
    return WaterRating * 2 + PowerRating * 2 + HospitalRating + PoliceRating + FireRating;
  }
}