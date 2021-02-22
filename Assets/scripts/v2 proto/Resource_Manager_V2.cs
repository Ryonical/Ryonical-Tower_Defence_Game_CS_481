using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Manager_V2 : MonoBehaviour
{
    #region EVENTS
    #endregion
    #region MEMBERS
    public const float default_resource_mass = 10f;

    //resource instances spawned during gameplay
    public static List<Resource_V2> resource_list;
    //resource prefabs. Uses a helper list to load the static list during runtime
    [Header("ADD RESOURCE PREFABS HERE")]
    public List<Resource_V2> resource_prefabs_initializer;
    public static List<Resource_V2> resource_prefabs;
    #endregion
    #region INIT
    private void Awake()
    {
        //init active resources list
        if (resource_list == null)
            resource_list = new List<Resource_V2>();

        //init + convert our inspector list to a static list
        if (resource_prefabs == null)
        {
            resource_prefabs = new List<Resource_V2>();
            foreach (Resource_V2 tow in resource_prefabs_initializer)
            {
                resource_prefabs.Add(tow);
                Debug.Log("resource prefab ID: " + tow.gameObject.ToString() + " initialized successfully.");
            }
        }
    }
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Resource_V2.OnSpawn += AddresourceToList;
        Resource_V2.OnDespawn += RemoveresourceFromList;
    }
    private void OnDisable()
    {
        Resource_V2.OnSpawn -= AddresourceToList;
        Resource_V2.OnDespawn -= RemoveresourceFromList;
    }
    #endregion
    #region EVENT HANDLERS
    void AddresourceToList(object caller, ResourceV2Args args)
    {
        resource_list.Add(args.resource);
    }
    void RemoveresourceFromList(object caller, ResourceV2Args args)
    {
        resource_list.Remove(args.resource);
    }
    #endregion

    public static List<Resource_V2> GetResourceList()
    {
        return resource_list;
    }
    public static List<Resource_V2> GetResourcePrefabList()
    {
        return resource_prefabs;
    }

}
