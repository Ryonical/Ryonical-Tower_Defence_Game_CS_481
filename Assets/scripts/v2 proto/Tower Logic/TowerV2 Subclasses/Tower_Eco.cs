using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Eco : Tower_V2
{
    public List<Resource_V2> income_generated_per_wave;

    #region EVENT SUBSCRIPTIONS
    public override void OnEnable()
    {
        base.OnEnable();
        Wave_Manager_V2.WaveStartEvent += GenerateResources;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        Wave_Manager_V2.WaveStartEvent -= GenerateResources;
    }
    #endregion

    private void GenerateResources(object sender, EventArgs args)
    {
        Resource_Confetti.SpawnConfetti(income_generated_per_wave, gameObject.transform.position + new Vector3(0, 15)); //TODO: confetti spawn pos not linked to magic number 
    }

    public override void InitializeTowerBehavior()
    {
    }
    public override void StopTowerBehavior()
    {
    }

    public override void DisplayStats(object sender, MonobehaviourEventArgs args)
    {
        if (this == args.mono)
        {
            //Package up and send the list of stats to the Selection MGR
            List<string> stats_list = new List<string>();
            //name
            stats_list.Add(tower_name);
            //resources per wave
            foreach (Resource_V2 rsc in income_generated_per_wave) //TODO: unify common resource types into 1 string
            {
                stats_list.Add("Per Wave: " + rsc.resource_amount + " " + rsc.resource_name);
            }

            UI_Tower_Selection.DisplayStats(stats_list);
        }
    }
}
