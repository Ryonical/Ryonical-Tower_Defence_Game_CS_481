using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//THIS CLASS IS A SINGLETON! More than one in a scene will cause issues!

public class Enemy_Manager_V2 : MonoBehaviour
{
    #region EVENTS
    #endregion
    #region MEMBERS
    public static List<Enemy_V2> enemy_list;
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Enemy_V2.OnSpawn += AddEnemyToList;
        Enemy_V2.OnDespawn += RemoveEnemyFromList;
    }
    private void OnDisable()
    {
        Enemy_V2.OnSpawn -= AddEnemyToList;
        Enemy_V2.OnDespawn -= RemoveEnemyFromList;
    }
    #endregion
    #region INIT
    private void Awake()
    {
        if(enemy_list == null)
            enemy_list = new List<Enemy_V2>();
    }
    #endregion

    void AddEnemyToList(object caller, EnemyV2RefEventArgs args )
    {
        Debug.Log("Enemy ID " + args.enemy.gameObject.ToString() + " added to list successfully!");
        enemy_list.Add(args.enemy);
    }
    void RemoveEnemyFromList(object caller, EnemyV2RefEventArgs args)
    {
        Debug.Log("Enemy ID " + args.enemy.gameObject.ToString() + " removed from list successfully!");
        enemy_list.Remove(args.enemy);
    }

    public static List<Enemy_V2> GetEnemyList()
    {
        return enemy_list;
    }
}
