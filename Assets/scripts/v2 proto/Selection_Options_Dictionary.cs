using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //added

//Contains the list of options each tower type should have
//I'm going to obsolete this for now because it feels like a deeply bad idea...

public static class Selection_Options_Dictionary
{   
    #region MEMBERS
    static List<Selection_Option_List> list_of_option_lists;

    #endregion
    #region EVENTS
    #endregion
    #region EVENT SUBSCRIPTIONS
    #endregion
    #region EVENT HANDLERS
    #endregion

    //TODO: Invent some way of updating an individual option, likely by its reference
    public static void UpdateOptions(Selection_Option option, Selection_Option updated_option)
    {
        Debug.LogError("Selection_Options_Dictionary: UpdateOptions function not implemented!");
    }
    
    //TODO
    public static Selection_Option_List RetrieveOptionList(string id)
    {
        Selection_Option_List list = null;
        list = list_of_option_lists.Find(x => x.list_id == id);
        return list;
    }

}
