using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Education
{
  None,
  Primary,
  Secondary,
  Higher,
  Graduate,
  PostGraduate,
  Phd
}

public enum CommuteType
{
  ToWork,
  ToHome
}

[System.Serializable]
public class Citizen
{
  //name
  readonly Guid id;
  public byte age;
  public Education education;
  public byte health;
  public byte happiness;
  public int income;
  public Building residence;
  public CommercialBuilding cWorkplace; // only one is used
  public IndustrialBuilding iWorkplace;
  public string prefab;

  // Events
  public event Func<Vector3, Vector3, List<Vector3>> OnWorkCommute;


  public Citizen()
  {
    id = Guid.NewGuid();
    cWorkplace = null;
    iWorkplace = null;
    income = 1000;
  }

  public void Initialize()
  {
    Debug.Log("Citizen: Initialize() called");
    if (TimeManager.Instance != null)
    {
      TimeManager.Instance.On2Am += CommuteToWork;
    }
    else
    {
      Debug.LogError("Citizen: TimeManager.Instance is null");
    }
  }

  public void CommuteToWork()
  {
    //   Debug.Log("Citizen: CommuteToWork() called");

    //   if (residence.pos != null)
    //   {
    //     List<Vector3> path = OnWorkCommute?.Invoke(residence.pos, new Vector3(40, 0, 40));

    //     if (path != null && path.Count > 0)
    //     {
    //       Debug.Log($"Citizen: path is not null, TransportCitizen() called");
    //       try
    //       {
    //         MainThreadDispatcher.CollectRequest(this, path);
    //       }
    //       catch (Exception e)
    //       {
    //         Debug.LogError(e.Message);
    //       }
    //     }
    //     else
    //     {
    //       Debug.Log($"Citizen: path is Null or path.Count:{path.Count}");
    //     }
    //   }
    //   else
    //   {
    //     Debug.Log("Citizen: residence.pos is null");
    //   }
  }

  public override string ToString()
  {
    return $"age: {age}, cWorkplace: {cWorkplace}, iWorkplace: {iWorkplace}";
  }
}