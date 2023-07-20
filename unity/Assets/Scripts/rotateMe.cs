using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateMe : MonoBehaviour
{
    public Vector3 rotationAxis = Vector3.up;
    public float rotSpeed= 0.05f;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(rotationAxis, rotSpeed);
    }
}
