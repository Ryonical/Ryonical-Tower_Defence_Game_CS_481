using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Kinda more of a selection manager at this point...

public class UI_Tower_Selection : MonoBehaviour
{
    #region MEMBERS
    private bool flag_options_display_in_progress;
    [SerializeField]
    Selection_Option_List cur_options_list;
    [SerializeField]
    MonoBehaviour cur_selected_monobehavior;

    public GameObject selection_indicator_model;
    [SerializeField]
    private GameObject cur_selection_indicator;
    #endregion
    #region EVENTS
    public static event System.EventHandler<MonobehaviourEventArgs> TryDisplayStatsEvent;
    public static event System.EventHandler<MonobehaviourEventArgs> OnStartSelectEvent;
    public static event System.EventHandler<MonobehaviourEventArgs> OnEndSelectEvent;

    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Tower_V2.TowerClickedEvent += TryDisplayOptions;
        Tower_V2.OnDespawn += CheckStopDisplayingOptions;
        PlayerActionController.PlayerActionBeginEvent += CheckStopDisplayingOptions;
        //PlayerActionController.PlayerActionEndEvent += CheckStopDisplayingOptions;
    }
    private void OnDisable()
    {
        Tower_V2.TowerClickedEvent -= TryDisplayOptions;
        Tower_V2.OnDespawn -= CheckStopDisplayingOptions;
        PlayerActionController.PlayerActionBeginEvent -= CheckStopDisplayingOptions;
        //PlayerActionController.PlayerActionEndEvent -= CheckStopDisplayingOptions;
    }
    #endregion
    #region EVENT HANDLERS
    private void TryDisplayOptions(object sender, TowerV2RefEventArgs args)
    {
        if(PlayerActionController.PlayerActionAvailable())
        {
            StopDisplayingOptions();
            Debug.Log("Displaying Options");

            UpdateOptionsList(args.tower.selection_options_list);
            UpdateSelectedMonobehavior(args.tower);
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

            cur_selection_indicator = Instantiate(selection_indicator_model, cur_selected_monobehavior.transform.position + new Vector3(0, 7), Quaternion.identity); //TODO: remove magic vector 

            OnStartSelectEvent?.Invoke(this, new MonobehaviourEventArgs(cur_selected_monobehavior));
            flag_options_display_in_progress = true;
        }
    }
    private void StopDisplayingOptions()
    {
        //PlayerActionController.EndPlayerAction(); //I want this action to be channeled BEHIND other player actions, meaning it will defer to other player actions. That makes these action takes unnecessary

        Destroy(cur_selection_indicator.gameObject);

        OnEndSelectEvent?.Invoke(this, new MonobehaviourEventArgs(cur_selected_monobehavior));
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
    void UpdateSelectedMonobehavior(MonoBehaviour mono)
    {
        cur_selected_monobehavior = mono;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            StopDisplayingOptions();
        }
        if (flag_options_display_in_progress)
        {
            foreach (Selection_Option option in cur_options_list.options_list)
            {
                if (Input.GetKeyDown(option.hotkey_name))
                {
                    option.RequestHandleOption();
                }
            }
        }
    }

    //I literally hate this method with a passion
    private void OnGUI()
    {
        
        if (flag_options_display_in_progress)
        {
            DisplayOptions();
            TryDisplayStatsEvent?.Invoke(this, new MonobehaviourEventArgs(cur_selected_monobehavior));
        }
    }

    //TODO: finish, improve
    private void DisplayOptions()
    {
        //init
        float button_right_margin = 10;
        float button_offset = 10;
        float button_height = 30;
        float button_width = 150;
        float cur_button_offset = button_offset + button_height;

        if (/*cur_options_list != null && */cur_options_list.options_list.Capacity > 0)
        {
            foreach (Selection_Option option in cur_options_list.options_list)
            {
                //create string to display
                string display_string = option.option_name;
                if(option.hotkey_name.ToString() != "None")
                {
                    display_string += " [" + option.hotkey_name + "]";
                }
                

                //TODO: invoke option action if hotkey is pressed (and if hotkey exists!)
                if (GUI.Button(new Rect(Screen.width - button_right_margin - button_width, Screen.height - cur_button_offset, button_width, button_height), display_string))
                {
                    //TODO: invoke option action if button is pressed
                    option.RequestHandleOption();
                }
                
                /* MOVED TO UPDATE() function because apparently OnGui() is called however often it wants as opposed to once per frame!
                if(!flag_handle_req_done && Input.GetKeyDown(option.hotkey_name))
                {
                    option.RequestHandleOption();
                }
                */

                cur_button_offset += (button_offset + button_height);
            }
        }
    }

    public static void DisplayStats(List<string> stats_list)
    {
        //init
        float rect_left_margin = 10;
        float rect_offset = 10;
        float rect_height = 25;
        float rect_width = 130;
        float cur_rect_offset = rect_offset + rect_height;

        foreach (string str in stats_list)
        {
            GUI.Box(new Rect(rect_left_margin, Screen.height - cur_rect_offset, rect_width, rect_height), str);
            cur_rect_offset += (rect_offset + rect_height);
        }
    }
}
