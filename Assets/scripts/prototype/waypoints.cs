using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waypoints : MonoBehaviour
{
    public List<GameObject> waypoint_list; //sequence of waypoints that forms the level path

    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Enemy_V2.OnRequestWaypointList += AttachWaypointList;
    }
    private void OnDisable()
    {
        Enemy_V2.OnRequestWaypointList -= AttachWaypointList;
    }
    #endregion

    void AttachWaypointList(object caller, EnemyV2RefEventArgs args)
    {
        if(args.enemy.waypoints_list == null)
            args.enemy.waypoints_list = this;
    }
}
