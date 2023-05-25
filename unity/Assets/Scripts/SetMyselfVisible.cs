using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMyselfVisible : MonoBehaviour
{
    private MeshRenderer[] childRenderers;
    private CroquetSpatialComponent sc;
    
    private void Start()
    {
        childRenderers = GetComponentsInChildren<MeshRenderer>();
        sc = GetComponent<CroquetSpatialComponent>();
    }

    private void Update()
    {
        // or
        // if(CroquetSpatialSystem.Instance.hasObjectMoved(gameObject.GetInstanceID()))
        if (sc.hasBeenMoved)
        {
            foreach (var renderer in childRenderers)
            {
                renderer.enabled = true;
            }
            Destroy(this);
        }
    }
}
