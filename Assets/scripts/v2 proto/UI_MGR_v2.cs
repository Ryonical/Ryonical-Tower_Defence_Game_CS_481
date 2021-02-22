using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MGR_v2 : MonoBehaviour
{
    #region EVENTS
    public delegate void StartWaveAction();
    public static event StartWaveAction OnStartWave;
    public static event System.EventHandler<TowerPlacementEventArgs> OnStartTowerPlacement;
    #endregion
    #region MEMBERS
    #endregion

    #region GUI UPDATE METHODS
    private void OnGUI()
    {
        DisplayWaveStartButton();
        DisplayTowerPlaceButtons();
        DisplayLives();
        DisplayResources();
        DisplayWaveCounter();
    }
    //TODO
    private void DisplayWaveCounter()
    {
    }
    //TODO
    private void DisplayResources()
    {
        string display_string = "";

        foreach(Resource_Inventory.Resource_Attributes ra in Resource_Inventory.resource_inventory)
        {
            display_string += "[" + ra.resource_type.ToString() + ": " + ra.resource_amount.ToString() + "] ";
        }
        GUI.Box(new Rect(5, 5, 300, 30), display_string);
    }
    //TOOD
    private void DisplayLives()
    {
    }
    //TODO: Change away from dev button!
    private void DisplayTowerPlaceButtons()
    {
        //debug tower placement
        if (GUI.Button(new Rect(20, 60, 100, 30), "Tower 1a"))
        {
            if (CheckWaveStart())
                OnStartTowerPlacement?.Invoke(this, new TowerPlacementEventArgs( "tower 1a" ) );
        }
    }
    //TODO: Change from dev button
    private void DisplayWaveStartButton()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Start Wave"))
        {
            if (CheckWaveStart())
                OnStartWave?.Invoke();
        }
    }
    private bool CheckWaveStart()
    {
        //a new wave can start if wave is currently not happening
        return ! Wave_Manager_V2.GetWaveStatus();
    }
    #endregion

}

public class TowerPlacementEventArgs : System.EventArgs
{
    public string tower_name;
    
    public TowerPlacementEventArgs(string tow)
    {
        this.tower_name = tow;
    }
}