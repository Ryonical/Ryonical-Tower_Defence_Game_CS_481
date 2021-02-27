using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SINGLETON! Have only 1 in scene! 
//Should look remarkably similar to Enemy_Manager_V2...

public class Tower_Manager_V2 : MonoBehaviour
{
    #region EVENTS
    #endregion
    #region MEMBERS
    //tower instances spawned during gameplay
    public static List<Tower_V2> tower_list;

    //tower prefabs. Uses a helper list to load the static list during runtime
    [Header("ADD TOWER PREFABS HERE")]
    public List<Tower_V2> tower_prefabs_initializer;
    public static List<Tower_V2> tower_prefabs;

    #endregion
    #region INIT
    private void Awake()
    {
        //init active towers list
        if (tower_list == null)
            tower_list = new List<Tower_V2>();

        //init + convert our inspector list to a static list
        if (tower_prefabs == null)
        {
            tower_prefabs = new List<Tower_V2>();
            foreach (Tower_V2 tow in tower_prefabs_initializer)
            {
                tower_prefabs.Add(tow);
                //Debug.Log("Tower prefab ID: " + tow.gameObject.ToString() + " initialized successfully.");
            }
        }
    }
    private void Start()
    {
        InitializePrefabTowerSelectOptionsToDictionary();
    }
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Tower_V2.OnSpawn += AddTowerToList;
        Tower_V2.OnDespawn += RemoveTowerFromList;
    }
    private void OnDisable()
    {
        Tower_V2.OnSpawn -= AddTowerToList;
        Tower_V2.OnDespawn -= RemoveTowerFromList;
    }
    #endregion
    #region EVENT HANDLERS
    void AddTowerToList(object caller, TowerV2RefEventArgs args)
    {
        tower_list.Add(args.tower);
    }
    void RemoveTowerFromList(object caller, TowerV2RefEventArgs args)
    {
        tower_list.Remove(args.tower);
    }
    #endregion

    public static List<Tower_V2> GetTowerList()
    {
        return tower_list;
    }
    public static List<Tower_V2> GetTowerPrefabList()
    {
        return tower_prefabs;
    }

    //the best method name :)
    private void InitializePrefabTowerSelectOptionsToDictionary()
    {
        //Idea: Toss the tower selection options into the dictionary, then discard the list so prefab doesn't instantiate with that garbage
    }
}
