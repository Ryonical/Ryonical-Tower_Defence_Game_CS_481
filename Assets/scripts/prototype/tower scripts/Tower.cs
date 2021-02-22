using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{

    // Members
    public string tower_name;
    public float attack_cooldown;
    public float attack_radius;
    public float attack_damage;

    public enum AttackPriority {hp_highest, hp_lowest, speed_fastest, speed_slowest, first, last}; //planned feature: allow user to choose how each tower prioritizes its targets

    // Start is called before the first frame update
    void Start()
    {
        //start coroutine for ContinueAttackingTargets ?
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartAttacking(AttackPriority prio, List<enemy> enemies_list)
    {
        StartCoroutine(ContinueAttackingTargets(prio, enemies_list));
    }
    public void StopAttacking()
    {
        StopCoroutine("ContinueAttackingTargets");
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

    IEnumerator ContinueAttackingTargets(AttackPriority prio, List<enemy> enemies_list)
    {
        while(true)
        {
            //Debug.Log("ContinueAttackingTargets");
            enemy target = null;

            //get list of all targets in radius
            List<enemy> enemies_in_range = new List<enemy>();
            foreach (enemy en in enemies_list)
            {
                if ((transform.position - en.transform.position).sqrMagnitude < attack_radius * attack_radius) //sqrmagnitude cheaper than a costly sqrt operation
                {
                    enemies_in_range.Add(en);
                }
            }

            // Find the target based on the priority param
            // Note that a property of newly spawned enemies guarantees that the lower the index, the earlier it was spawned!
            // Equal values during search will be ignored; in that case first success will always be used
            if (enemies_in_range.Count > 0)
                switch (prio)
                {
                    case AttackPriority.hp_highest:
                        target = enemies_in_range[0];
                        foreach (enemy en in enemies_in_range)
                        {
                            if (en.hp > target.hp)
                                target = en;
                        }
                        break;
                    case AttackPriority.hp_lowest:
                        target = enemies_in_range[0];
                        foreach (enemy en in enemies_in_range)
                        {
                            if (en.hp < target.hp)
                                target = en;
                        }
                        break;
                    case AttackPriority.speed_fastest:
                        target = enemies_in_range[0];
                        foreach (enemy en in enemies_in_range)
                        {
                            if (en.speed > target.speed)
                                target = en;
                        }
                        break;
                    case AttackPriority.speed_slowest:
                        target = enemies_in_range[0];
                        foreach (enemy en in enemies_in_range)
                        {
                            if (en.speed < target.speed)
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

            //attack the targeted enemy
            if (target != null)
            {
                Debug.DrawLine(transform.position, target.transform.position, Color.yellow, .1f); //shows what it is hitting!
                DummyAttack(transform.position, target.transform.position, .1f);


                target.ApplyDamage(attack_damage);
                yield return new WaitForSeconds(attack_cooldown); //invoke cooldown if we successfully attacked
            }
            else
            {
                yield return null; //we did not attack. No cooldown invoked.
            }
        }
    }
}
