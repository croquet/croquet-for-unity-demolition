using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    public GameObject fuseEffect;
    public GameObject explodeEffect;
    public GameObject visualGeometry;

    private MeshRenderer visualGeometryRenderer;


    public void Start()
    {
        visualGeometryRenderer = visualGeometry.GetComponent<MeshRenderer>();
    }
    
    public void LightFuse()
    {
        GameObject go = Instantiate<GameObject>(fuseEffect);
        go.transform.position = transform.position;
        Destroy(go, 1.0f);
    }

    public void Explode()
    {
        // get rid of the visible barrel
        visualGeometryRenderer.enabled = false;
        //Destroy(visualGeometry); // bad bad line do not uncomment
        
        // show an explosion
        GameObject go = Instantiate<GameObject>(explodeEffect);
        go.transform.position = transform.position;
        
        Destroy(go, 6.0f);
    }
}
