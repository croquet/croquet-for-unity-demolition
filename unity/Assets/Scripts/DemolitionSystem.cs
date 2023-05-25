using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemolitionSystem : CroquetSystem
{
    public override List<string> KnownCommands { get; } = new List<string>()
    {
        "fuseLit",
        "exploded"
    };
    
    protected override Dictionary<int, CroquetComponent> components { get; set; }

    public override void ProcessCommand(string command, string[] args)
    {
        if (command == "fuseLit") BarrelFuseLit(args);
        else if (command == "exploded") BarrelExploded(args);
    }
    
    void BarrelFuseLit(string[] args)
    {
        GameObject barrel = CroquetEntitySystem.Instance.GetGameObjectByCroquetHandle(args[0]);
        if (barrel != null)
        {
            barrel.GetComponent<ExplosionController>().LightFuse();
        }
    }
    
    void BarrelExploded(string[] args)
    {
        GameObject barrel = CroquetEntitySystem.Instance.GetGameObjectByCroquetHandle(args[0]);
        if (barrel != null)
        {
            barrel.GetComponent<ExplosionController>().Explode();
        }
    }

}
