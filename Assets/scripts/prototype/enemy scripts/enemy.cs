using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//added
using System.Linq;

//enemy archetypes inerit from this class

public class enemy : MonoBehaviour
{
    // Members
    public Money money;
    //this is kinda spaghetti but i dont wanna write a whole event system for this rn
    public WaveManager wave_manager;
    public void AttachEventManager(WaveManager mgr)
    {
        //if(event_manager != null)
        wave_manager = mgr;
    }

    //readonly?
    public float speed_max;
    public float hp_max;

    //resources awarded for defeating;
    [Header("ADD RESOURCES AS COMPONENTS FOR KILL REWARD (not implemented yet)")]
    public List<Resource> resource_kill_award_list;

    public float hp;
    public float speed;

    // spawn characteristics
    public float spawn_delay; //sec     time before this enemy can spawn
    public float spawn_cooldown; //sec  time added before next spawn can occur

    public int cur_waypoint_index; //index of the current waypoint we are going toward


    // Start is called before the first frame update
    void Start()
    {
        hp = hp_max;
        speed = speed_max;

        //get all resources attached to this gameobject. add them to kill reward
        //TODO: fix this!
        //resource_kill_award_list = GetComponents<Resource>().ToList();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(GameObject target)
    {
        Vector3 dir = target.transform.position - transform.position;
        Vector3 distance = dir;
        dir.Normalize();

        //correct for overshooting
        if ((dir * Time.deltaTime * speed).sqrMagnitude < distance.sqrMagnitude)
            transform.Translate(dir * Time.deltaTime * speed);
        else
            transform.position = target.transform.position;
    }

    public virtual void Kill()
    {
        //...
        money.AddMoney(50);
        Destroy(gameObject);
    }

    public void ApplyDamage(float damage)
    {
        hp -= damage;

        //check for death
        if(hp <= 0)
        {
            
            if (wave_manager != null)
            {
                wave_manager.KillEnemy(this);
            }
            else
            {
                Kill();
            }
        }
    }

}
