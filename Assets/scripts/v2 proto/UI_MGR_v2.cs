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
    public static event System.EventHandler<System.EventArgs> RequestPlacementNodePing;
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

        foreach(Resource_Attributes ra in Resource_Inventory.resource_inventory)
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
        int start_pos_y = 50;

        int bt_height = 30;
        int bt_width = 100;
        int bt_x = 20;
        int bt_y_offset = 10;

        int sidebox_offset = 5;
        int sidebox_width = 5;

        GUI.Box(new Rect(bt_x, start_pos_y, bt_width + bt_width + sidebox_offset, bt_height), "Place Tower");

        int cur_bt_y = start_pos_y + bt_height + bt_y_offset; //we placed a box already so we calculate

        foreach (Tower_V2 tower in Tower_Manager_V2.tower_prefabs) //TODO: remove hotkey handling from this method and add it to the Update() method. It gets called 1 or more times when nested in OnGUI()...
        {
            string display_string = tower.tower_name;
            if(tower.placement_hotkey.ToString() != "Empty")
            {
                display_string += " [" + tower.placement_hotkey.ToString() + "]";
            }


            bool flag_tower_place_event_started = false;

            if (GUI.Button(new Rect(bt_x, cur_bt_y, bt_width, bt_height), display_string))
            {
                StartTowerPlacement(tower);
                flag_tower_place_event_started = true;
            }

            if (Input.GetKeyDown(tower.placement_hotkey) && !flag_tower_place_event_started)
            {
                StartTowerPlacement(tower);
            }

            string price_string = "";
            foreach(Resource_Attributes ra in tower.price_list)
            {
                price_string += ra.resource_type.ToString() + ": " + ra.resource_amount.ToString() + " ";
            }

            GUI.Box(new Rect(bt_x + bt_width + sidebox_offset, cur_bt_y, bt_width, bt_height), price_string);


            cur_bt_y += bt_y_offset + bt_height;
        }

        //encompassing sidebox
        GUI.Box(new Rect(Mathf.Clamp((bt_x - sidebox_offset - sidebox_width), 0, Mathf.Infinity)
            , start_pos_y
            , sidebox_width
            , (cur_bt_y - start_pos_y - bt_y_offset))   //we subtract offset once here so there will not be a bit of hanging rectangle below the last button
            , ""); 
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

    private void StartTowerPlacement(Tower_V2 tower)
    {
        if(CheckWaveStart()) //REMOVE THIS TO ALLOW TO TOWER PLACEMENT WHENEVER
        {
            OnStartTowerPlacement?.Invoke(this, new TowerPlacementEventArgs(tower.tower_name));
            RequestPlacementNodePing?.Invoke(this, System.EventArgs.Empty);
        }
    }

}

public class TowerPlacementEventArgs : System.EventArgs
{
    public string tower_name;
    
    public TowerPlacementEventArgs(string tow)
    {
        this.tower_name = tow;
    }
}