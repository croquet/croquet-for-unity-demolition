using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public GameObject fuseEffect;
    public GameObject explodeEffect;
    public GameObject visualGeometry;

    private MeshRenderer visualGeometryRenderer;

    public void Awake()
    {
        Croquet.Listen(gameObject, "fuseLit", LightFuse);
        Croquet.Listen(gameObject, "exploded", Explode);
    }                                     
    
    public void Start()
    {
        visualGeometryRenderer = visualGeometry.GetComponent<MeshRenderer>();
    }

    private void LightFuse()
    {
        GameObject go = Instantiate<GameObject>(fuseEffect);
        go.transform.position = transform.position;
        Destroy(go, 1.0f);
    }

    private void Explode()
    {
        // get rid of the visible barrel
        visualGeometryRenderer.enabled = false;
        
        // show an explosion
        GameObject go = Instantiate<GameObject>(explodeEffect);
        go.transform.position = transform.position;
        
        Destroy(go, 6.0f);
    }
    
}
