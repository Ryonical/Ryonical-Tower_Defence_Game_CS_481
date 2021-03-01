using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An execution flow helper class designed to let scripts check to see if any other scripts are currently channeling a player action, and handle that situation accordingly
//Kinda behaves like a Mutex but really who knows lol
//This is a first draft. I dunno if it will do everything we need it to...

public static class PlayerActionController
{
    private static bool flag_player_channeling_action;
    #region EVENTS
    public static event System.EventHandler<System.EventArgs> PlayerActionBeginEvent; //the more useful event I'd recommend framing logic around. Can be used, for instance, to cancel some channeled operation currently running
    public static event System.EventHandler<System.EventArgs> PlayerActionEndEvent;
    #endregion
    //need to have a way of promising to have an end condition to channeling an action...

    public static bool PlayerActionAvailable()
    {
        //Debug.Log("Player Action available is " + (!flag_player_channeling_action).ToString());
        return flag_player_channeling_action == false;
    }

    public static void StartPlayerAction()
    {
        if(PlayerActionAvailable())
        {
            flag_player_channeling_action = true;
            PlayerActionBeginEvent?.Invoke(null, System.EventArgs.Empty);
        }
        else
        {
            //Debug.LogError("PlayerActionController: Class Started a player action while another one is active!");
        }
    }
    public static void EndPlayerAction()
    {
        PlayerActionEndEvent?.Invoke(null, System.EventArgs.Empty);
        flag_player_channeling_action = false;
    }


}
