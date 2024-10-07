using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    CinemachineVirtualCamera cinemachineVirtualCamera;

    // used with HandleCameraMovementDragPan()
    private bool dragPanActive = false;
    Vector2 lastMousePos;

    // used with HandleCameraZoom_Fov()
    private float targetFov;
    private readonly float minFov = 20;
    private readonly float maxFov = 90;

    // used with HandleMouseZoom_Forward() and HandleCameraZoom_LowerY()
    private Vector3 followOffset;

    private void Awake()
    {
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            new Vector3(0, 50, -40);
        followOffset = cinemachineVirtualCamera
            .GetCinemachineComponent<CinemachineTransposer>()
            .m_FollowOffset;
    }

    void Start() { }

    void Update()
    {
        if (Utility.isDashboardActive)
            return;
            
        HandleCameraMovement();
        HandleCameraRotation();
        HandleCameraMovementDragPan();
        HandleCameraZoom_Fov();
        HandleCameraZoom_LowerY();
    }

    public void HandleCameraMovement()
    {
        Vector3 inputDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            inputDirection.z = +1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputDirection.x = -1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            inputDirection.z = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputDirection.x = +1f;

        Vector3 moveDirection =
            transform.forward * inputDirection.z + transform.right * inputDirection.x;
        float moveSpeed = 50f;
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void HandleCameraRotation()
    {
        float rotateDirection = 0f;
        const float rotateSpeed = 100f;

        // TODO
        // using mouse

        // using keys
        if (Input.GetKey(KeyCode.Q))
            rotateDirection = 1f;
        if (Input.GetKey(KeyCode.E))
            rotateDirection = -1f;

        transform.eulerAngles += new Vector3(0, rotateDirection * rotateSpeed * Time.deltaTime, 0);
    }

    /// <summary>
    ///
    /// </summary>
    private void HandleCameraMovementDragPan()
    {
        Vector3 inputDirection = new Vector3(0, 0, 0);

        if (Input.GetMouseButtonDown(0))
        {
            dragPanActive = true;
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragPanActive = false;
        }

        if (dragPanActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePos;

            float dragPanSpeed = .2f;
            inputDirection.x = -mouseMovementDelta.x * dragPanSpeed;
            inputDirection.z = -mouseMovementDelta.y * dragPanSpeed;

            lastMousePos = Input.mousePosition;

            Vector3 moveDirection =
                transform.forward * inputDirection.z + transform.right * inputDirection.x;
            float moveSpeed = 50f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }

    private void HandleCameraZoom_Fov()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.mouseScrollDelta.y > 0)
                targetFov -= 5;
            if (Input.mouseScrollDelta.y < 0)
                targetFov += 5;

            targetFov = Mathf.Clamp(targetFov, minFov, maxFov);
            cinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
                cinemachineVirtualCamera.m_Lens.FieldOfView,
                targetFov,
                Time.deltaTime * 10f
            );
        }
    }

    private void HandleCameraZoom_LowerY()
    {
        float zoomAmount = 10f;

        if (Input.mouseScrollDelta.y > 0)
            followOffset.y -= zoomAmount;
        if (Input.mouseScrollDelta.y < 0)
            followOffset.y += zoomAmount;

        followOffset.y = Mathf.Clamp(followOffset.y, 50, 1100);

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(
                cinemachineVirtualCamera
                    .GetCinemachineComponent<CinemachineTransposer>()
                    .m_FollowOffset,
                followOffset,
                Time.deltaTime * 10f
            );
    }

    // private void HandleCameraZoom_MoveForward()
    // {
    //     Vector3 zoomDirection = followOffset.normalized;

    //     float zoomAmount = 3f;
    //     if (Input.mouseScrollDelta.y > 0)
    //         followOffset += zoomDirection * zoomAmount;
    //     if (Input.mouseScrollDelta.y < 0)
    //         followOffset -= zoomDirection * zoomAmount;

    //     if (followOffset.magnitude < followOffsetMin)
    //         followOffset = zoomDirection * followOffsetMin;
    //     if (followOffset.magnitude > followOffsetMax)
    //         followOffset = zoomDirection * followOffsetMax;

    //     cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
    //         Vector3.Lerp(
    //             cinemachineVirtualCamera
    //                 .GetCinemachineComponent<CinemachineTransposer>()
    //                 .m_FollowOffset,
    //             followOffset,
    //             Time.deltaTime * 10f
    //         );
    // }

    //     private static Vector3 HandlecameraMovementEdgeScroll(Vector3 inputDirection)
    //     {
    //         int edgeScrollSize = 20; // range for screen corners

    //         if (Input.mousePosition.x < edgeScrollSize)
    //         {
    //             inputDirection.x = -1f;
    //         }

    //         if (Input.mousePosition.y < edgeScrollSize)
    //         {
    //             inputDirection.z = -1f;
    //         }

    //         if (inputDirection.x > (Screen.width - edgeScrollSize))
    //         {
    //             inputDirection.x = 1f;
    //         }

    //         if (inputDirection.y > (Screen.height - edgeScrollSize))
    //         {
    //             inputDirection.x = 1f;
    //         }

    //         return inputDirection;
    //     }
}
