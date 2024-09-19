using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour, ICroquetDriven
{
    public GameObject fuseEffect;
    public GameObject explodeEffect;
    public GameObject visualGeometry;

    private MeshRenderer visualGeometryRenderer;

    public void PawnInitializationComplete()
    {
        // because this script is attached to the prefab, attempting to add these subscriptions during
        // Awake() would likely be too early: before the construction and initialisation of the
        // EntityComponent (on which adding a Listen subscription depends).
        // but because all explicit Croquet say() events are automatically forwarded over the
        // bridge, regardless of whether anyone in Unity has subscribed, adding the subscription
        // here will be in time to catch any say() that has already been published.
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
        //Destroy(visualGeometry); // bad bad line do not uncomment

        // show an explosion
        GameObject go = Instantiate<GameObject>(explodeEffect);
        go.transform.position = transform.position;

        Destroy(go, 6.0f);
    }

}
