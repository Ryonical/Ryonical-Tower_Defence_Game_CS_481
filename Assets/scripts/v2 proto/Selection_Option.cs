using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A list of Selection_Option's are to be attached to every tower type. 
//Should the list be instantiated once per tower type or for every tower instance? Control VS Space efficiency

public class Selection_Option : MonoBehaviour
{
    #region MEMBERS
    public string option_name;
    public KeyCode hotkey_name;
    public string option_tooltip; //could describe cost or other helpful usage advice. Show this when user mouses over button
    [Header("AutoAdded")]
    public MonoBehaviour attached_object;

    #endregion
    #region EVENTS
    public static event System.EventHandler<SelectionOptionArgs> OnSelectionOptionInvoke;

    #endregion
    #region EVENT SUBSCRIPTIONS
    #endregion
    #region INIT
    #endregion
    #region EVENT HANDLERS
    #endregion

    //so we're going to just send a string, have a switch statement of handler subroutines, and try to find a match...
    public void RequestHandleOption()
    {
        OnSelectionOptionInvoke?.Invoke(this, new SelectionOptionArgs(this, attached_object));
    }
}
//helper class
public class SelectionOptionArgs : System.EventArgs
{
    public MonoBehaviour attached_monobehavior;
    public Selection_Option option;
    public SelectionOptionArgs(Selection_Option opt, MonoBehaviour mono)
    {
        this.option = opt;
        this.attached_monobehavior = mono;
    }
}

