using System;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public GameObject canvas;
    public GameObject gridOutline;

    public UnityEvent<ZoneData> onZoneInput = new();

    void Update()
    {
        ListenForZone();
        ListenForDashboard();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetWorldPositionOnMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float hitDistance;

        if (plane.Raycast(ray, out hitDistance))
        {
            Vector3 worldPosition = ray.GetPoint(hitDistance);
            worldPosition = Utility.Round(worldPosition);
            return worldPosition;
        }
        return new Vector3(-1, -1, -1);
    }

    /// <summary>
    /// enum for state management for ListenForZone()
    /// </summary>
    private enum ZoneListenState
    {
        WaitingForZone, WaitingForFirstClick, WaitingForSecondClick
    }

    /// <summary>
    /// Variables for ListenForZone(), outside the scope of method because Update() is called too frequently
    /// </summary>
    private ZoneListenState currentState = ZoneListenState.WaitingForZone;
    private ZoneData.Type? zoneType = null;
    private Vector3 firstClickPosition;
    private Vector3 secondClickPosition;

    /// <summary>
    /// Listen for correct input to create zone.
    /// Invoke OnZoneInput on correct input.
    /// </summary>
    private void ListenForZone()
    {
        // if (Utility.isDashboardActive)
        //     return;

        // to make the rotation of some structures possible
        //when a prefab is hovered, it exits, hence 'r' can be used to rotate the prefab
        if (Utility.isHoverBuilding)
            return;

        switch (currentState)
        {
            case ZoneListenState.WaitingForZone:
                if (Input.GetKeyDown(KeyCode.R))
                {
                    gridOutline.SetActive(true);
                    zoneType = ZoneData.Type.Residential;
                    currentState = ZoneListenState.WaitingForFirstClick;
                }
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    gridOutline.SetActive(true);
                    zoneType = ZoneData.Type.Commercial;
                    currentState = ZoneListenState.WaitingForFirstClick;
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    gridOutline.SetActive(true);
                    zoneType = ZoneData.Type.Industrial;
                    currentState = ZoneListenState.WaitingForFirstClick;
                }
                break;

            case ZoneListenState.WaitingForFirstClick:
                if (Input.GetMouseButtonDown(0))
                {
                    firstClickPosition = GetWorldPositionOnMouseClick();
                    currentState = ZoneListenState.WaitingForSecondClick;
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    currentState = ZoneListenState.WaitingForZone;
                    gridOutline.SetActive(false);
                }
                break;

            case ZoneListenState.WaitingForSecondClick:
                if (Input.GetMouseButtonDown(0))
                {
                    secondClickPosition = GetWorldPositionOnMouseClick();

                    ZoneData data = new(zoneType.Value, firstClickPosition, secondClickPosition);
                    onZoneInput.Invoke(data);

                    currentState = ZoneListenState.WaitingForZone;
                    zoneType = null;

                    gridOutline.SetActive(false);
                }
                break;
        }
    }


    public void HandleResidentialZoneCreation()
    {
        StartZoneCreation(ZoneData.Type.Residential);
    }

    public void HandleCommercialZoneCreation()
    {
        StartZoneCreation(ZoneData.Type.Commercial);
    }

    public void HandleIndustrialZoneCreation()
    {
        StartZoneCreation(ZoneData.Type.Industrial);
    }

    private void StartZoneCreation(ZoneData.Type zoneType)
    {
        gridOutline.SetActive(true);
        canvas.SetActive(false);
        this.zoneType = zoneType;
        currentState = ZoneListenState.WaitingForFirstClick;
    }

    /// <summary>
    /// On pressing "space" toggles the dashboard.
    /// </summary>
    public void ListenForDashboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Utility.isDashboardActive)
            {
                Utility.isDashboardActive = false;
                canvas.SetActive(false);
            }
            else
            {
                Utility.isDashboardActive = true;
                canvas.SetActive(true);
            }

        }
    }
}
