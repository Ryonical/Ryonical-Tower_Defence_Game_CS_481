using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public waypoints waypoint_mgr;
    public EventManager event_mgr; //not a fan of this method...

    public Transform spawn_location;
    private bool wave_in_progress = false;

    public List<wave> wave_list;

    [SerializeField]
    private int cur_wave_index = 0;

    public List<enemy> enemies_alive;
    List<enemy> GetEnemiesAliveList() { return enemies_alive; } //making a list into a property retroactively was a bit weird so i just manually implemented get

    public List<Tower> towers_alive;
    public Lives life;
    // event subscription
    void OnEnable()
    {
        EventManager.OnClicked += StartWave; //add StartWave() to click event
    }
    void OnDisable()
    {
        EventManager.OnClicked -= StartWave; //remove StartWave() from click event
    }

    private void Start()
    {
        //for testing
        wave_in_progress = false;
        foreach (Tower to in towers_alive)
        {
            to.StartAttacking(Tower.AttackPriority.hp_lowest, enemies_alive);
        }
    }

    // Update is called once per frame
    // Currently handles updating enemy positions, removal, etc.
    // Also marks wave_in_progress flag based on whether enemies are currently alive. COULD lead to edge case where start can be clicked more than once!
    // TODO: Simplify Update function and organize this code into its own function
    void Update()
    {
        Stack<enemy> remove_enemies = new Stack<enemy>(); //buffer to mark enemies for removal

        if (enemies_alive.Count > 0)
        {
            //move enemies, check if they get to end
            foreach (enemy en in enemies_alive)
            {

                wave_in_progress = true;

                //move to next waypoint. if at that waypoint, increment. 
                //If there are no more waypoints, die i guess...

                //have to short circuit this with first statement
                if (en.cur_waypoint_index < waypoint_mgr.waypoint_list.Count
                    && waypoint_mgr.waypoint_list[en.cur_waypoint_index].transform.position
                        == en.transform.position
                       )
                {
                    en.cur_waypoint_index++;
                }

                if (en.cur_waypoint_index < waypoint_mgr.waypoint_list.Count)
                {
                    en.MoveTo(waypoint_mgr.waypoint_list[en.cur_waypoint_index]);
                }
                else
                {
                    //mark enemies for destruction
                    life.SubtractLives(1);
                    remove_enemies.Push(en);

                }
            }
        }
        else
        {
            wave_in_progress = false;
        }

        //destroy enemies
        while(remove_enemies.Count > 0)
        {
            enemy en = remove_enemies.Pop();
            KillEnemy(en);
        }
    }

    void StartWave()
    {
        //check for remaining waves
        if(cur_wave_index >= wave_list.Count)
        {
            Debug.Log("StartWave() fail @ index");
        }
        //check for wave in progress
        else if (wave_in_progress)
        {
            Debug.Log("StartWave() fail @ flag");

        }
        else
        {
            //Debug.Log("StartWave()");
            wave_in_progress = true; //I'll bet its safer to leave this here 
            StartCoroutine(ContinueWave(wave_list[cur_wave_index]));
        }
    }

    IEnumerator ContinueWave(wave cur_wave)
    {
        foreach (enemy en in cur_wave.enemy_list)
        {
            if (en == null) Debug.Log("WaveManager ContinueWave(): null found!");
            else
            {
                enemy en_instance;
                enemies_alive.Add(en_instance = Instantiate(en, spawn_location.position, Quaternion.identity));
                en_instance.AttachEventManager(this);

                yield return new WaitForSeconds(en.spawn_cooldown);
            }
        }
        cur_wave_index++;
    }

    public void KillEnemy(enemy en)
    {
        if ( en == enemies_alive.Find(x => x.gameObject.GetInstanceID().Equals(en.gameObject.GetInstanceID())) ) //test this one! ... Seems to be working - Jared
        {
            enemies_alive.Remove(en); //remove from list!
            en.Kill(); //for cleanup and/or custom features for this enemy type!
        }
    }
    public void KillEnemy(int en_index)
    {
        if(en_index < enemies_alive.Count)
        {
            enemy en = enemies_alive[en_index];
            KillEnemy(en);
        }
    }

    public void AddTower(GameObject tower)
    {
        int temp = 0;
        Tower twoer = tower.GetComponent<Tower>();
        towers_alive.Add(twoer);
        temp = towers_alive.Count;
        towers_alive[temp-1].StartAttacking(Tower.AttackPriority.hp_lowest, enemies_alive);
    }
}
