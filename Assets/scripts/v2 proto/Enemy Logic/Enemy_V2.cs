using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//added
using System.Linq;
using System;

//enemy archetypes inerit from this class

public class Enemy_V2 : MonoBehaviour
{
    #region MEMBERS
    //resources awarded for defeating this enemy type
    //this should be a list of prefabs
    [Header("ADD RESOURCES PREFABS HERE FOR KILL REWARD")]
    public List<Resource_V2> resource_kill_award;
    public waypoints waypoints_list;

    //prefab stats
    [Min(.01f)]
    public float speed_max;
    [Min(1f)]
    public float hp_max;
    //in-game (changing) stats
    public float current_hp;
    public float current_speed;
    // spawn characteristics
    public float spawn_delay; //sec     time before this enemy can spawn
    public float spawn_cooldown; //sec  time added before next spawn can occur

    //utility
    [SerializeField]
    private int cur_waypoint_index; //index of the current waypoint we are going toward
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        
    }
    #endregion
    #region EVENTS
    public static event System.EventHandler<EnemyV2RefEventArgs> OnRequestWaypointList;
    public static event System.EventHandler<EnemyV2RefEventArgs> OnDespawn;
    public static event System.EventHandler<EnemyV2RefEventArgs> OnSpawn;

    public static event System.EventHandler<EnemyV2RefEventArgs> DamagePlayerEvent;
    #endregion
    #region INIT
    private void Start()
    {
        current_hp = hp_max;
        current_speed = speed_max;
        cur_waypoint_index = 0;

        //request waypoint list and initialize position to first waypoint
        OnRequestWaypointList?.Invoke(this, new EnemyV2RefEventArgs(this) );
        if(waypoints_list != null)
        {
            if(waypoints_list.waypoint_list.Count > 0)
            {
                //set position to first waypoint
                transform.position = waypoints_list.waypoint_list[0].transform.position;
            }
        }
        else
        {
            Debug.Log("Enemy ID " + this.gameObject.ToString() + ": waypoint list not connected! Destroying!");
            Destroy(this);
        }

        StartMoving();
        OnSpawn?.Invoke(this, new EnemyV2RefEventArgs(this));
    }
    #endregion

    public void ApplyDamage(float damage)
    {
        current_hp -= damage;

        //check for death
        if (current_hp <= 0)
        {
            HandleDeath();
        }
    }
    public void StartMoving()
    {
        //make sure not to duplicate the coroutine...
        StopMoving();
        StartCoroutine(ContinueMoving());
    }
    public void StopMoving()
    {
        StopCoroutine(ContinueMoving());
    }

    private void HandleDeath()
    {
        //if we reach end of waypoints path
        if(transform.position == waypoints_list.waypoint_list[ waypoints_list.waypoint_list.Count - 1].transform.position )
        {
            DamagePlayer();
        }
        else if (current_hp <= 0)
        {
            DropResources();
        }
        else
        {
            Debug.LogError("Enemy destroyed without dying OR reaching the waypoint end? What did you do?????");
        }
        Kill();
    }
    private void DamagePlayer()
    {
        Debug.Log("Enemy: Damage player event");
        DamagePlayerEvent?.Invoke(this, new EnemyV2RefEventArgs(this));
    }
    private void Kill()
    {
        OnDespawn?.Invoke(this, new EnemyV2RefEventArgs(this));
        Destroy(gameObject);
    }
    //TODO
    private void DropResources()
    {
        //create a gameobject with a working resource confetti script
        GameObject goj_instance;
        goj_instance = new GameObject();
        goj_instance.transform.position = transform.position;
        goj_instance.transform.rotation = Quaternion.identity;
        Resource_Confetti conf = goj_instance.AddComponent<Resource_Confetti>() as Resource_Confetti;
        conf.resource_list = resource_kill_award;

        //throw new NotImplementedException();
    }
    private void MoveTo(GameObject target)
    {
        Vector3 dir = target.transform.position - transform.position;
        Vector3 distance = dir;
        dir.Normalize();

        //correct for overshooting
        if ((dir * Time.deltaTime * current_speed).sqrMagnitude < distance.sqrMagnitude)
            transform.Translate(dir * Time.deltaTime * current_speed);
        else
            transform.position = target.transform.position;
    }
    
    IEnumerator ContinueMoving()
    {
        while (transform.position != waypoints_list.waypoint_list[waypoints_list.waypoint_list.Count - 1].transform.position)
        {
            MoveTo(waypoints_list.waypoint_list[cur_waypoint_index]);

            if(transform.position == waypoints_list.waypoint_list[cur_waypoint_index].transform.position && cur_waypoint_index < waypoints_list.waypoint_list.Count)
            {
                cur_waypoint_index++;
            }
            yield return null;
        }

        HandleDeath();
    }
}

//helper class to pass enemy ref from events in this class
public class EnemyV2RefEventArgs : System.EventArgs
{
    public Enemy_V2 enemy;

    public EnemyV2RefEventArgs(Enemy_V2 en)
    {
        this.enemy = en;
    }
}
