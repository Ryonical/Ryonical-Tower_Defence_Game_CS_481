using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_V2 : MonoBehaviour
{

    public enum AttackPriority { hp_highest, hp_lowest, speed_fastest, speed_slowest, first, last }; //planned feature: allow user to choose how each tower prioritizes its targets


    #region EVENTS
    public static event System.EventHandler<TowerV2RefEventArgs> OnDespawn;
    public static event System.EventHandler<TowerV2RefEventArgs> OnSpawn;
    public static event System.EventHandler<TowerV2RefEventArgs> TowerClickedEvent;
    #endregion
    #region MEMBERS
    //prefab stats
    public string tower_name;
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

    AttackPriority current_attack_priority;
    #endregion
    #region INIT
    void Start()
    {
        attack_cooldown_current = attack_cooldown_default;
        attack_radius_current = attack_radius_default;
        attack_damage_current = attack_damage_default;

        current_attack_priority = AttackPriority.hp_lowest;
        StartAttacking();

        OnSpawn?.Invoke(this, new TowerV2RefEventArgs(this) );
    }
    #endregion

    private void OnMouseDown()
    {
        Debug.Log("Tower ID: " + gameObject.ToString() + " was clicked!");
        TowerClickedEvent?.Invoke(this, new TowerV2RefEventArgs(this));
    }

    public void StartAttacking()
    {
        StopAttacking();
        StartCoroutine(ContinueAttackingTargets());
    }
    public void StopAttacking()
    {
        StopCoroutine(ContinueAttackingTargets());
    }

    private void HandleTowerRemoval()
    {
        RefundResources();
        RemoveTower();
    }
    //TODO
    private void RefundResources()
    {

    }
    private void RemoveTower()
    {
        OnDespawn?.Invoke(this, new TowerV2RefEventArgs(this) );
        Destroy(this);
    }
    private void DummyAttack(Vector3 start_pos, Vector3 end_pos, float projectile_lifetime)
    {
        //calculate rate to get from start_pos to end_pos in given time
        float distance = Mathf.Abs((start_pos - end_pos).magnitude);
        float speed = Mathf.Abs(distance / projectile_lifetime);

        StartCoroutine(ContinueDummyAttack(start_pos, end_pos, speed));
    }

    IEnumerator ContinueDummyAttack(Vector3 start_pos, Vector3 end_pos, float projectile_speed)
    {
        GameObject proj;
        proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        proj.transform.localScale = new Vector3(1f, 1f, 1f);
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
                        break;
                    case AttackPriority.last:  //TODO
                        target = enemies_in_range[0];
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

                target.ApplyDamage(attack_damage_current);
                yield return new WaitForSeconds(attack_cooldown_current); //invoke cooldown if we successfully attacked
            }
            else
            {
                yield return null; //we did not attack. No cooldown invoked.
            }
        }
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