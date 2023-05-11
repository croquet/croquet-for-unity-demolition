using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MoveAndShoot : MonoBehaviour
{
    private GameObject mainCamera;
    private float yaw = 0;
    private float pitch = 0;

    private bool dragging = false;
    private bool dragMoved = false;
    Vector2 touchDeltaSinceLastDown = new Vector2();

    private int lastTouchCount = 0;

    void Awake()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        LookForSecondaryClick();
        ProcessPointer();
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

    void ProcessPointer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Down");
            touchDeltaSinceLastDown = Vector2.zero;
            dragging = true;
            dragMoved = false; // if still false on up, this was just a tap

            // read yaw and pitch from camera's current position
            Quaternion q = mainCamera.transform.localRotation;
            yaw = Mathf.Rad2Deg * Mathf.Atan2(2 * q.y * q.w - 2 * q.x * q.z, 1 - 2 * q.y * q.y - 2 * q.z * q.z);
            pitch = Mathf.Rad2Deg * Mathf.Atan2(2*q.x*q.w - 2*q.y*q.z, 1 - 2*q.x*q.x - 2*q.z*q.z);
        }

        if (Input.GetMouseButton(0))
        {
            // Debug.Log(Pointer.current.delta.ReadValue());
            Vector2 xyDelta = Pointer.current.delta.ReadValue();
            if (xyDelta.x == 0 && xyDelta.y == 0) return;
            
            MoveCamera(xyDelta);
            
            touchDeltaSinceLastDown += xyDelta;
            if (touchDeltaSinceLastDown.magnitude > 2)
            {
                //Debug.Log(touchDeltaSinceLastDown.magnitude.ToString());
                dragMoved = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (dragging && !dragMoved)
            {
                Fire();
            }

            dragging = false;
            dragMoved = false;
        }
    }
    
    void MoveCamera(Vector2 xyDelta)
    {            
        /*
            // from Worldcore demolition
            yaw += -0.01 * e.xy[0];
            yaw = yaw % TAU;
            pitch += -0.01 * e.xy[1];
            pitch = Math.min(pitch, toRad(-10));
            pitch = Math.max(pitch, toRad(-80));
            
            const pitchMatrix = m4_rotation([1,0,0], pitch)
            const yawMatrix = m4_rotation([0,1,0], yaw)

            let cameraMatrix = m4_translation([0,0,50]);
            cameraMatrix = m4_multiply(cameraMatrix,pitchMatrix);
            cameraMatrix = m4_multiply(cameraMatrix,yawMatrix);
         */

        // @@ movement ratios currently chosen by trial and error.
        float moveRatio = Input.touchSupported ? 0.002f : 0.01f;
        yaw += (moveRatio * Mathf.Rad2Deg * xyDelta.x);
        yaw %= 360;
        pitch += (-moveRatio * Mathf.Rad2Deg * xyDelta.y);
        pitch = Mathf.Clamp(pitch, 10, 80);
            
        Vector3 camOffset = new Vector3(0, 0, -50);
        Quaternion yawQ = Quaternion.AngleAxis(yaw, new Vector3(0, 1, 0));
        Quaternion pitchQ = Quaternion.AngleAxis(pitch, new Vector3(1, 0, 0));
        Quaternion camQ = yawQ * pitchQ;
        Vector3 pos = camQ * camOffset;
        mainCamera.transform.localPosition = pos;
        mainCamera.transform.localRotation = camQ;
    }

    void Fire()
    {
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
        CroquetBridge.SendCroquetSync("event", "shoot", positionStr);
    }

}
