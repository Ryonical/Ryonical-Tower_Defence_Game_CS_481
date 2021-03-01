using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; //Added by Jared Freeman
using System;

public class Tower_V2 : MonoBehaviour
{
    public enum AttackPriority { hp_highest, hp_lowest, speed_fastest, speed_slowest, first, last }; //planned feature: allow user to choose how each tower prioritizes its targets

    #region EVENTS
    public static event System.EventHandler<TowerV2RefEventArgs> OnDespawn;
    public static event System.EventHandler<TowerV2RefEventArgs> OnSpawn;
    public static event System.EventHandler<TowerV2RefEventArgs> TowerClickedEvent;
    #endregion
    #region MEMBERS
    [Header("Add a selection options list here")]
    public Selection_Option_List selection_options_list; //welp honestly this works so i guess itll have to do

    //prefab stats
    public string tower_name;
    [Header("Simply create Resource Attributes components and add them to the list.")]
    [Header("Avoid duplicating resource types!")]
    public List<Resource_Attributes> price_list;
    public float attack_cooldown_default;
    [Min(0f)]
    public float attack_radius_default;
    [Min(0f)]
    public float attack_damage_default;
    //changeable stats
    public float attack_cooldown_current;
    public float attack_radius_current;
    public float attack_damage_current;

    protected AttackPriority current_attack_priority;
    public KeyCode placement_hotkey;

    protected float time_since_last_attack;
    protected bool flag_show_cooldown_bar;
    #endregion
    #region EVENT SUBSCRIPTIONS
    public virtual void OnEnable()
    {
        UI_Tower_Selection.TryDisplayStatsEvent += DisplayStats;
        UI_Tower_Selection.OnStartSelectEvent += HandleSelectionStart;
        UI_Tower_Selection.OnEndSelectEvent += HandleSelectionEnd;
        Selection_Option.OnSelectionOptionInvoke += HandleSelectionOptionRequest;
    }
    public virtual void OnDisable()
    {
        UI_Tower_Selection.TryDisplayStatsEvent -= DisplayStats;
        UI_Tower_Selection.OnStartSelectEvent -= HandleSelectionStart;
        UI_Tower_Selection.OnEndSelectEvent -= HandleSelectionEnd;
        Selection_Option.OnSelectionOptionInvoke -= HandleSelectionOptionRequest;
    }
    #endregion
    #region EVENT HANDLERS
    //if the selection MGR is asking for this monobehavior, we package up our data and invoke display stats via the MGR
    public virtual void DisplayStats(object sender, MonobehaviourEventArgs args)
    {
        if (this == args.mono)
        {
            //Handle any unique display "update()" behaviors we want from the tower here


            //Package up and send the list of stats to the Selection MGR
            List<string> stats_list = new List<string>();
            //name
            stats_list.Add(tower_name);
            //attack priority
            stats_list.Add("Priority: " + current_attack_priority);
            //damage + cooldown + range
            if (attack_damage_current == attack_damage_default)
            {
                stats_list.Add("Damage: " + attack_damage_current.ToString());
            }
            else
            {
                if (attack_damage_current > attack_damage_default)
                {
                    stats_list.Add("Damage: " + attack_damage_default.ToString() + " + " + (attack_damage_current - attack_damage_default).ToString());
                }
                else
                {
                    stats_list.Add("Damage: " + attack_damage_default.ToString() + " - " + (attack_damage_default - attack_damage_current).ToString());
                }
            }
            if (attack_cooldown_current == attack_cooldown_default)
            {
                stats_list.Add("Cooldown: " + attack_cooldown_current.ToString());
            }
            else
            {
                if (attack_cooldown_current > attack_cooldown_default)
                {
                    stats_list.Add("Cooldown: " + attack_cooldown_default.ToString() + " + " + (attack_cooldown_current - attack_cooldown_default).ToString());
                }
                else
                {
                    stats_list.Add("Cooldown: " + attack_cooldown_default.ToString() + " - " + (attack_cooldown_default - attack_cooldown_current).ToString());
                }
            }
            if (attack_radius_current == attack_radius_default)
            {
                stats_list.Add("Range: " + attack_radius_current.ToString());
            }
            else
            {
                if (attack_radius_current > attack_radius_default)
                {
                    stats_list.Add("Range: " + attack_radius_default.ToString() + " + " + (attack_radius_current - attack_radius_default).ToString());
                }
                else
                {
                    stats_list.Add("Range: " + attack_radius_default.ToString() + " - " + (attack_radius_default - attack_radius_current).ToString());
                }
            }

            UI_Tower_Selection.DisplayStats(stats_list);
        }
    }
    private void HandleSelectionStart(object sender, MonobehaviourEventArgs args)
    {
        if (this == args.mono)
        {
            StartDisplayingCooldown();
        }
    }
    private void HandleSelectionEnd(object sender, MonobehaviourEventArgs args)
    {
        if (this == args.mono)
        {
            StopDisplayingCooldown();
        }
    }
    //check for monobehavior match then switch statement to dispatch into behavior subroutines
    private void HandleSelectionOptionRequest(object sender, SelectionOptionArgs args)
    {
        if (args.attached_monobehavior == this)
        {
            string option = args.option.option_name.ToLower();

            OverrideBaseSelectionOptions(option);

            switch (option)
            {
                case ("sell"):
                    Debug.Log("try sell");
                    SellTower();
                    break;
                case ("upgrade"):
                    UpgradeTower();
                    break;
                case ("attack priority"):
                    ChangeAttackPriority();
                    break;
                default:
                    HandleAdditionalSelectionOptions(option);
                    break;
            }
        }
    }

    //Can be implemented in child classes to handle more options
    public virtual void OverrideBaseSelectionOptions(string option) { }
    //Can be implemented in child classes to handle more options
    public virtual void HandleAdditionalSelectionOptions(string option) { }

    #endregion
    #region INIT
    void Start()
    {
        attack_cooldown_current = attack_cooldown_default;
        attack_radius_current = attack_radius_default;
        attack_damage_current = attack_damage_default;

        time_since_last_attack = Time.time;
        flag_show_cooldown_bar = false;

        current_attack_priority = AttackPriority.hp_lowest;
        StartTowerBehaviorLoop();

        //AttachResourceAttributes(); //obsolete

        if (selection_options_list == null)
        {
            selection_options_list = gameObject.AddComponent<Selection_Option_List>();
        }
        selection_options_list.AttachTo(this);

        OnSpawn?.Invoke(this, new TowerV2RefEventArgs(this));
    }
    #endregion

    private void OnMouseDown()
    {
        //Debug.Log("Tower ID: " + gameObject.ToString() + " was clicked!");
        TowerClickedEvent?.Invoke(this, new TowerV2RefEventArgs(this));
    }
    //TODO: cooldown bar
    private void OnGUI()
    {
        if (flag_show_cooldown_bar)
        {
            //ContinueDisplayingCooldown(); //NOT WORKING!
        }
    }

    //ISSUE: This will not execute prior to when resources are checked. Probably just need to remove this feature...
    private void AttachResourceAttributes()
    {
        price_list.Clear(); //make sure this is empty to prevent duplication of refs

        List<Resource_Attributes> ra_list = GetComponents<Resource_Attributes>().ToList<Resource_Attributes>();

        foreach (Resource_Attributes ra in ra_list)
        {
            price_list.Add(ra);
        }
    }

    #region TOWER BEHAVIOR INITIALIZERS
    //These two just handle execution of starting and finishing behavior loop
    public void StartTowerBehaviorLoop()
    {
        EndTowerBehaviorLoop();
        InitializeTowerBehavior();
    }
    public void EndTowerBehaviorLoop()
    {
        StopTowerBehavior();
    }
    //These two are for use in children. Likely will start a coroutine
    public virtual void InitializeTowerBehavior()
    {
        StartCoroutine(ContinueAttackingTargets());
    }
    public virtual void StopTowerBehavior()
    {
        StopCoroutine(ContinueAttackingTargets());
    }
    #endregion

    private void StartDisplayingCooldown()
    {
        flag_show_cooldown_bar = true;
    }
    private void StopDisplayingCooldown()
    {
        flag_show_cooldown_bar = false;
    }

    #region SELECTION OPTION HANDLERS
    //TODO:
    private void UpgradeTower()
    {

    }
    private void SellTower()
    {
        RefundResources();
        RemoveTower();
    }
    //There is almost certainly a better way to do this but enums are wonky...
    private void ChangeAttackPriority()
    {

        AttackPriority first = Enum.GetValues(typeof(AttackPriority)).Cast<AttackPriority>().Min();
        AttackPriority last = Enum.GetValues(typeof(AttackPriority)).Cast<AttackPriority>().Max();

        Debug.Log("first: " + first + "last: " + last);

        AttackPriority orig = current_attack_priority;

        foreach (AttackPriority prio in System.Enum.GetValues(typeof(AttackPriority)))
        {
            if((int)current_attack_priority <= (int)orig 
                || ((int)current_attack_priority > (int)prio && (int)orig < (int)prio)) //avoid reassigning original val
            {
                current_attack_priority = prio;
            }
            Debug.Log("c: " + (int)current_attack_priority + " p: " + (int)prio + " o: " + (int)orig);
        }
        if((int)last == (int)current_attack_priority && (int)current_attack_priority == (int)orig)
        {
            Debug.Log("TO FIRST: c: " + (int)current_attack_priority + " o: " + (int)orig + " f: " + (int)first);
            current_attack_priority = first;
        }
        
    }
    #endregion

    //TODO: Develop a better strategy for reducing the number of resources returned upon refund. it's currently a magic number...
    private void RefundResources()
    {
        List<Resource_Attributes> refund_ras = new List<Resource_Attributes>();
        foreach(Resource_Attributes ra in price_list)
        {
            Resource_Attributes ra_to_add = gameObject.AddComponent<Resource_Attributes>();
            ra_to_add.resource_amount = ra.resource_amount;
            ra_to_add.resource_type = ra.resource_type;

            ra_to_add.resource_amount = (int)(ra_to_add.resource_amount * .75f);

            refund_ras.Add(ra_to_add);
        }

        Resource_Inventory.AddResourcesToInventory(refund_ras);
    }
    private void RemoveTower()
    {
        OnDespawn?.Invoke(this, new TowerV2RefEventArgs(this) );
        StopAllCoroutines();
        Destroy(gameObject);
    }

    private void DummyAttack(Vector3 start_pos, Vector3 end_pos, float projectile_lifetime)
    {
        //calculate rate to get from start_pos to end_pos in given time
        float distance = Mathf.Abs((start_pos - end_pos).magnitude);
        float speed = Mathf.Abs(distance / projectile_lifetime);

        StartCoroutine(ContinueDummyAttack(start_pos, end_pos, speed));
    }

    //Could make this accept any gameobject to serve as the projectile mesh...
    IEnumerator ContinueDummyAttack(Vector3 start_pos, Vector3 end_pos, float projectile_speed)
    {
        GameObject proj;
        proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proj.transform.localScale = new Vector3(.8f, .8f, .8f);
        proj.transform.position = start_pos;

        while (proj.transform.position != end_pos)
        {
            Vector3 dir = end_pos - proj.transform.position;
            Vector3 distance = dir;
            dir.Normalize();

            //correct for overshooting
            if ((dir * Time.deltaTime * projectile_speed).sqrMagnitude < distance.sqrMagnitude)
                proj.transform.Translate(dir * Time.deltaTime * projectile_speed);
            else
                proj.transform.position = end_pos;

            yield return null;
        }

        Destroy(proj);
        yield return null;
    }

    //TODO: Finish Attack Priority finding
    //  ^ seems to be working
    //TODO: Change DummyAttack() to some real attack animation handler
    IEnumerator ContinueAttackingTargets()
    {
        while (true)
        {
            List<Enemy_V2> enemies_list = Enemy_Manager_V2.GetEnemyList();

            Enemy_V2 target = null;

            //get list of all targets in radius
            List<Enemy_V2> enemies_in_range = new List<Enemy_V2>();
            foreach (Enemy_V2 en in enemies_list)
            {
                if ((transform.position - en.transform.position).sqrMagnitude < attack_radius_current * attack_radius_current) //sqrmagnitude cheaper than a costly sqrt operation
                {
                    enemies_in_range.Add(en);
                }
            }

            // Find the target based on the priority param
            // Note that a property of newly spawned enemies guarantees that the lower the index, the earlier it was spawned!
            // Equal values during search will be ignored; in that case first success will always be used
            if (enemies_in_range.Count > 0)
                switch (current_attack_priority)
                {
                    case AttackPriority.hp_highest:
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.current_hp > target.current_hp)
                                target = en;
                        }
                        break;
                    case AttackPriority.hp_lowest:
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.current_hp < target.current_hp)
                                target = en;
                        }
                        break;
                    case AttackPriority.speed_fastest:
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.current_speed > target.current_speed)
                                target = en;
                        }
                        break;
                    case AttackPriority.speed_slowest:
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.current_speed < target.current_speed)
                                target = en;
                        }
                        break;
                    case AttackPriority.first:  //TODO
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.GetCurrentWaypointIndex() > target.GetCurrentWaypointIndex())
                            {
                                target = en;
                            }
                            else if(en.GetCurrentWaypointIndex() == target.GetCurrentWaypointIndex() && en.GetSquareDistanceToNextWaypoint() < target.GetSquareDistanceToNextWaypoint())
                            {
                                target = en;
                            }
                        }
                        break;
                    case AttackPriority.last:  //TODO
                        target = enemies_in_range[0];
                        foreach (Enemy_V2 en in enemies_in_range)
                        {
                            if (en.GetCurrentWaypointIndex() < target.GetCurrentWaypointIndex())
                            {
                                target = en;
                            }
                            else if (en.GetCurrentWaypointIndex() == target.GetCurrentWaypointIndex() && en.GetSquareDistanceToNextWaypoint() > target.GetSquareDistanceToNextWaypoint())
                            {
                                target = en;
                            }
                        }
                        break;
                    default:
                        target = enemies_in_range[0]; //default to first (only guaranteed) element.
                        break;
                }//end switch

            //Check to see if a target was found. Attack it
            if (target != null)
            {
                Debug.DrawLine(transform.position, target.transform.position, Color.yellow, .1f); //shows what it is hitting!
                DummyAttack(transform.position, target.transform.position, .1f); //TODO: change to a handler of some kind

                time_since_last_attack = Time.time;
                target.ApplyDamage(attack_damage_current);
                yield return new WaitForSeconds(attack_cooldown_current); //invoke cooldown if we successfully attacked
            }
            else
            {
                yield return null; //we did not attack. No cooldown invoked.
            }
        }
    }

    //TODO: Broken! fix!
    private void ContinueDisplayingCooldown()
    {
        float bar_width = 40;
        float bar_width_half = bar_width / 2;
        float bar_height = 10;
        float bar_height_half = bar_height / 2;


        Vector3 center_3 = Camera.current.WorldToScreenPoint(transform.position); //why is the y component scaling wrong?????????? 
        //center_3 *= -1;
        Debug.Log(center_3);

        //cooldown box 1
        GUI.Box(new Rect((center_3.x - bar_width_half), (center_3.y - bar_height_half), bar_width, bar_height), "");

        //dynamic cd box
        float cur = Mathf.Clamp((Time.time - time_since_last_attack), 0, attack_cooldown_current);
        Freeman_Utilities.MapValueFromRangeToRange(cur, 0, attack_cooldown_current, 0, bar_width);

        GUI.Box(new Rect((center_3.x - bar_width_half), (center_3.y - bar_height_half), cur, bar_height), "");

    }
}
//helper class to pass tower ref from events in this class
public class TowerV2RefEventArgs : System.EventArgs
{
    public Tower_V2 tower;

    public TowerV2RefEventArgs(Tower_V2 tow)
    {
        this.tower = tow;
    }
}