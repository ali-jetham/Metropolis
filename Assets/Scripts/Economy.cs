using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Economy
{
    // Events
    public event Action<float> OnTreasuryUpdated;

    // Fields
    public float treasury;
    private float totalIncome;
    private float totalExpenses;
    // private List<float> monthlyIncome;
    // private List<float> monthlyExpenses;

    public float TotalIncome => totalIncome; // Read-only property for total income
    public float TotalExpenses => totalExpenses; // Read-only property for total expenses
    public float Treasury => treasury;

    // public void HandleMonthPassed()
    // {
    //     // calculation
    //     float income = 0;
    //     monthlyIncome.Add(income);
    // }

    public void AddIncome(float amount)
    {
        Debug.Log($"Economy: AddIncome() called, adding {amount} to treasury");
        treasury += amount;
        Debug.Log($"Economy: treasury = {treasury}");
        totalIncome += amount;
        NotifyTreasuryUpdate();
    }

    public void DeductExpenses(float amount)
    {
        Debug.Log($"Economy: Deduct Expenses called: {amount}");
        treasury -= amount;
        totalExpenses += amount;
        NotifyTreasuryUpdate();
    }

    // public float CityTreasury()
    // {
    //     float treasury = TotalIncome - TotalExpenses;
    //     Debug.Log($"City Treasury Amount: {treasury}");
    //     return treasury;
    // }

    private void NotifyTreasuryUpdate()
    {
        // float treasury = CityTreasury();
        OnTreasuryUpdated?.Invoke(treasury);
    }

    public float CalculateResidentialTaxes(List<Citizen> citizens)
    {
        float totalTax = 0;
        if (citizens == null)
        {
            Debug.LogWarning("No citizens found for tax calculation.");
            return totalTax;
        }

        foreach (var citizen in citizens)
        {
            totalTax += citizen.income * 0.1f; // Example: 10% tax
        }
        AddIncome(totalTax);
        Debug.Log($"Residential Taxes calculated: {totalTax}");
        return totalTax;
    }

    public float CalculateCommercialTaxes(List<CommercialBuilding> commercialBuildings)
    {
        float totalTax = 0;
        if (commercialBuildings == null)
        {
            Debug.LogWarning("No commercial buildings found for tax calculation.");
            return totalTax;
        }

        foreach (var building in commercialBuildings)
        {
            if (building.level.Equals(0))
            {
                totalTax += 50;
            }
            else
            {
                totalTax += building.level * 50; // Example: $50 per building level
            }
        }
        AddIncome(totalTax);
        Debug.Log($"Commercial Taxes calculated: {totalTax}");
        return totalTax;
    }

    public float CalculateIndustrialTaxes(List<IndustrialBuilding> industrialBuildings)
    {
        float totalTax = 0;
        if (industrialBuildings == null)
        {
            Debug.LogWarning("No industrial buildings found for tax calculation.");
            return totalTax;
        }

        foreach (var building in industrialBuildings)
        {
            if (building.level.Equals(0))
            {
                totalTax += 75;
            }
            else
            {
                totalTax += building.level * 75; // Example: $50 per building level
            }
        }
        AddIncome(totalTax);
        Debug.Log($"Industrial Taxes calculated: {totalTax}");
        return totalTax;
    }

    public void CalculateMaintenanceCost(List<UtilityBuilding> utilityBuildings, List<ServiceBuilding> serviceBuildings)
    {
        Debug.Log("Economy: CalculateMaintenanceCost() called");
        float totalMaintenanceCost = 0;
        float serviceCost = 0;
        float utilityCost = 0;

        // Iterate through utility buildings
        if (utilityBuildings != null)
        {
            foreach (UtilityBuilding building in utilityBuildings)
            {
                utilityCost += building.maintenanceCost;
                Debug.Log($"Utility Building Level: {building.level}, Maintenance Cost: {utilityCost}");
            }

        }

        // Iterate through service buildings
        if (serviceBuildings != null)
        {
            foreach (var building in serviceBuildings)
            {
                float maintenanceCostMultiplier = 1.0f;
                switch (building.serviceType)
                {
                    case ServiceType.Hospital:
                        maintenanceCostMultiplier = 1.5f; // Higher cost for hospitals
                        break;
                    case ServiceType.PoliceStation:
                        maintenanceCostMultiplier = 1.2f; // Moderate cost for police stations
                        break;
                    case ServiceType.FireStation:
                        maintenanceCostMultiplier = 1.3f; // Moderate cost for fire stations
                        break;
                    case ServiceType.School:
                        maintenanceCostMultiplier = 1.1f; // Lower cost for schools
                        break;
                    default:
                        break;
                }
                serviceCost += building.maintenanceCost * maintenanceCostMultiplier;
            }
        }
        totalMaintenanceCost = utilityCost + serviceCost;
        Debug.Log($"Maintenance costs deducted: {totalMaintenanceCost}");
        DeductExpenses(totalMaintenanceCost);
    }

}
