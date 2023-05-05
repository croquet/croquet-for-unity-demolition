using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MoveAndShoot : MonoBehaviour
{
    private int lastTouchCount = 0;
    private Actions inputActions;
    private InputAction look;
    private InputAction fire;
    Vector2 touchDeltaSinceLastDown = new Vector2();

    private GameObject mainCamera;
    private float yaw = 0;
    private float pitch = 0;

    void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
        
        // input registration
        inputActions = new Actions();
        inputActions.Enable();
        look = inputActions.Player.Look;
        look.Enable();

        fire = inputActions.Player.Fire;
        fire.Enable();

        if (Input.touchSupported)
        {
            //inputActions.Player.Fire.started += DragStart;
            inputActions.Player.Fire.canceled += Fire;
            inputActions.Player.Look.started += DragStart;
            inputActions.Player.Look.canceled += DragEnd;
            inputActions.Player.Look.performed += DragMove;
        }
        else
        {
            inputActions.Player.Fire.started += DragStart;
            inputActions.Player.Fire.canceled += DragEnd;
            inputActions.Player.Look.performed += DragMove;
        }
    }

    void Update()
    {
        LookForSecondaryClick();
    }

    void LookForSecondaryClick()
    {
        // primary click is handled by the drag detection
        bool secondaryClick = false;
        if (Input.touchSupported)
        {
            int countNow = Input.touchCount;
            secondaryClick = countNow <= 1 && lastTouchCount > 1;
            lastTouchCount = countNow;
        }
        else secondaryClick = Input.GetMouseButtonUp(1);
        
        if (secondaryClick) CroquetBridge.SendCroquet("event", "pointerUp", "1");
    }

    bool dragging = false;
    bool dragMoved = false;
    void DragStart(InputAction.CallbackContext callbackContext)
    {
        // Debug.Log("Drag start");

        touchDeltaSinceLastDown = Vector2.zero;
        dragging = true;
        dragMoved = false; // if still false on up, this was just a tap

        // read yaw and pitch from camera's current position
        Quaternion q = mainCamera.transform.localRotation;
        yaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
        pitch = Mathf.Rad2Deg * Mathf.Atan2(2*q.x*q.w - 2*q.y*q.z, 1 - 2*q.x*q.x - 2*q.z*q.z);
        // Debug.Log($"starting yaw: {yaw} pitch: {pitch}");
    }

    void DragMove(InputAction.CallbackContext callbackContext)
    {
        if (!dragging) return;

        //Debug.Log("Drag move");

        Vector2 delta = callbackContext.ReadValue<Vector2>();
        MoveCamera(delta);

        touchDeltaSinceLastDown += delta;
        if (touchDeltaSinceLastDown.magnitude > 2)
        {
            //Debug.Log(touchDeltaSinceLastDown.magnitude.ToString());
            dragMoved = true;
        }
    }

    void DragEnd(InputAction.CallbackContext callbackContext)
    {
        // Debug.Log($"Drag end: dragging={dragging} dragMoved={dragMoved}");

        // on touch device, there is a constant stream of short-lived drag
        // start, move, end.  a tap will come through as Fire.Canceled, which
        // we use to trigger Fire() below directly.
        if (!Input.touchSupported && dragging && !dragMoved)
        {
            Fire(callbackContext);
        }

        dragging = false;
        dragMoved = false;
    }

    void MoveCamera(Vector2 delta)
    {
        /* adapted from Croquet-view code:
            yaw += 0.005 * deltaX;
            yaw %= TAU;
            pitch += -0.005 * deltaY;
            pitch = Math.min(pitch, toRad(80));
            pitch = Math.max(pitch, toRad(10));
         */

        yaw += Mathf.Rad2Deg * 0.005f * delta.x;
        yaw %= 360;
        pitch += Mathf.Rad2Deg * -0.005f * delta.y;
        pitch = Mathf.Clamp(pitch, 10, 80);

        // Debug.Log($"yaw: {yaw} pitch: {pitch}");

        Vector3 camOffset = new Vector3(0, 0, -50);
        Quaternion yawQ = Quaternion.AngleAxis(yaw, new Vector3(0, 1, 0));
        Quaternion pitchQ = Quaternion.AngleAxis(pitch, new Vector3(1, 0, 0));
        Quaternion camQ = yawQ * pitchQ;
        Vector3 pos = camQ * camOffset;
        mainCamera.transform.localPosition = pos;
        mainCamera.transform.localRotation = camQ;
    }

    void Fire(InputAction.CallbackContext callbackContext)
    {
        // Debug.Log("Shoot");

        /*
            const pitchMatrix = m4_rotation([1, 0, 0], pitch);
            const yawMatrix = m4_rotation([0, 1, 0], yaw);
            const both = m4_multiply(pitchMatrix, yawMatrix);
            const shoot = v3_transform(gun, both);
         */
        Quaternion camRot = mainCamera.transform.localRotation;
        Vector3 gunOffset = new Vector3(0, -1, -49);
        Vector3 gun = camRot * gunOffset;
        string positionStr = string.Join<float>(",", new[] { gun.x, gun.y, gun.z });
        CroquetBridge.SendCroquet("event", "shoot", positionStr); // $$$ SendCroquetSync
    }

}
