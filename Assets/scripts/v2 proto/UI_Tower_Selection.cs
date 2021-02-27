using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Implements the selection UI for towers

public class UI_Tower_Selection : MonoBehaviour
{
    #region MEMBERS
    private bool flag_options_display_in_progress;
    [SerializeField]
    Selection_Option_List cur_options_list;
    #endregion
    #region EVENTS
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Tower_V2.TowerClickedEvent += TryDisplayOptions;
        PlayerActionController.PlayerActionBeginEvent += CheckStopDisplayingOptions;
    }
    private void OnDisable()
    {
        Tower_V2.TowerClickedEvent -= TryDisplayOptions;
    }
    #endregion
    #region EVENT HANDLERS
    private void TryDisplayOptions(object sender, TowerV2RefEventArgs args)
    {
        if(PlayerActionController.PlayerActionAvailable())
        {
            Debug.Log("Displaying Options");

            UpdateOptionsList(args.tower.selection_options_list);
            StartDisplayingOptions();
        }
    }
    #endregion
    #region INIT
    private void OnStart()
    {
        flag_options_display_in_progress = false;
    }
    #endregion
    #region PLAYER ACTION HANDLING
    private void StartDisplayingOptions()
    {
        if (PlayerActionController.PlayerActionAvailable()) //possibly redundant but good for protection
        {
            //Changed this feature to just autocancel if another starts
            //PlayerActionController.StartPlayerAction(); //it is CRITICAL this comes before flag setting
            flag_options_display_in_progress = true;
        }
    }
    private void StopDisplayingOptions()
    {
        //PlayerActionController.EndPlayerAction(); //I want this action to be channeled BEHIND other player actions, meaning it will defer to other player actions. That makes these action takes unnecessary
        flag_options_display_in_progress = false;
    }
    private void CheckStopDisplayingOptions(object sender, System.EventArgs args)
    {
        if(flag_options_display_in_progress)
        {
            StopDisplayingOptions();
        }
    }
    #endregion

    void UpdateOptionsList(Selection_Option_List opt)
    {
        cur_options_list = opt;
    }

    private void OnGUI()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            StopDisplayingOptions();
        }
        if(flag_options_display_in_progress)
        {
            DisplayOptions();
        }
    }

    //TODO: finish, improve
    private void DisplayOptions()
    {
        //init
        float button_right_margin = 10;
        float button_offset = 10;
        float button_height = 40;
        float button_width = 150;
        float cur_button_offset = button_offset;

        if (/*cur_options_list != null && */cur_options_list.options_list.Capacity > 0)
        {
            foreach (Selection_Option option in cur_options_list.options_list)
            {
                //TODO: invoke option action if hotkey is pressed (and if hotkey exists!)
                if (GUI.Button(new Rect(Screen.width - button_right_margin - button_width, cur_button_offset, button_width, button_height), (option.option_name + " - " + option.hotkey_name)))
                {
                    //TODO: invoke option action if button is pressed
                }
                cur_button_offset += (button_offset + button_height);
            }
        }
    }
}
