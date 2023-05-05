using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionCustomBridge : CroquetBridgeExtension
{
    private CroquetBridge bridge;
    private void Awake()
    {
        bridge = this.gameObject.GetComponent<CroquetBridge>();
    }

    private void Start()
    {
        ShowQRCode qrShower = GameObject.FindObjectOfType<ShowQRCode>();
        if (qrShower != null)
        {
            int sessionNameValue = PlayerPrefs.GetInt("sessionNameValue", 1);
            qrShower.DisplayQR($"https://croquet.io/demolition-multi/?q={sessionNameValue}");
        }
    }

    public override bool ProcessCommand(string command, string[] args)
    {
        bool handled = true;
        if (command == "fuseLit") BarrelFuseLit(args);
        else if (command == "exploded") BarrelExploded(args);
        else handled = false;
        return handled;
    }
    
    void BarrelFuseLit(string[] args)
    {
        GameObject barrel = bridge.FindObject(args[0]);
        if (barrel != null)
        {
            barrel.GetComponent<ExplosionController>().LightFuse();
        }
    }
    
    void BarrelExploded(string[] args)
    {
        GameObject barrel = bridge.FindObject(args[0]);
        if (barrel != null)
        {
            barrel.GetComponent<ExplosionController>().Explode();
        }
    }

}
