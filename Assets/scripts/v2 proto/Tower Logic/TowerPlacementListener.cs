using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//added by me:
using System.Linq;
using System;

public class TowerPlacementListener : MonoBehaviour
{
    #region MEMBERS
    private bool flag_placement_in_progress;
    private Tower_V2 tower_to_place;

    [Header("ADD MATERIALS HERE")]
    public Material red_ghost_material;
    public Material blue_ghost_material;
    public Material fully_transparent_ghost_material;

    private GameObject ghost_tower;

    [Min(0.01f)]
    private static float info_bubble_duration = 3.25f;
    #endregion
    #region INIT
    private void Start()
    {
        if (red_ghost_material == null || blue_ghost_material == null || fully_transparent_ghost_material == null)
        {
            Debug.LogError("TowerPlacementListener: One or more materials not defined!");
        }
    }
    #endregion
    #region EVENTS
    public static event System.EventHandler<TowerV2RefEventArgs> TowerPlacedEvent;
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        TowerPlacementNode.TowerNodeSelectEvent += OnNodeSelect;
        TowerPlacementNode.NodeMouseEnterEvent += OnNodeMouseEnter;
        TowerPlacementNode.NodeMouseExitEvent += OnNodeMouseExit;
        UI_MGR_v2.OnStartTowerPlacement += StartTowerPlacement;
        PlayerActionController.PlayerActionBeginEvent += CheckForPlacementCancel;
    }
    private void OnDisable()
    {
        TowerPlacementNode.TowerNodeSelectEvent -= OnNodeSelect;
        TowerPlacementNode.NodeMouseEnterEvent -= OnNodeMouseEnter;
        TowerPlacementNode.NodeMouseExitEvent -= OnNodeMouseExit;
        UI_MGR_v2.OnStartTowerPlacement -= StartTowerPlacement;
        PlayerActionController.PlayerActionBeginEvent -= CheckForPlacementCancel;
    }

    #endregion
    #region EVENT HANDLERS

    //TODO: 
    //[DONE?] Display ghost tower on mouse enter IFF flag_placement_in_progress
    //[DONE?] Provide contextual hint (blue / red) showing if tower is legally placeable at the Node
    //Need: Node Occupied status, current resource amount.
    //TODO:  Must continue to update current resource amount and check to see if we need to change from red to blue
    //  Can change to a continuecheckforresources coroutine or something that ends at the other stop conds in this listener impl
    void OnNodeMouseEnter(object caller, TowerPlaceSelectEventArgs args)
    {
        //Decision tree of ghost rendering, Might be better to handle differently but this works for now
        if(flag_placement_in_progress)
        {
            if(args.node.IsOccupied())
            {
                RenderGhostRed(args.position.position); //oops... we're going to have to live with this transform "position" mislabel for now...
            }
            else
            {
                if(Resource_Inventory.CheckResourcesAvailable(tower_to_place.price_list))
                {
                    RenderGhostBlue(args.position.position);
                }
                else
                {
                    RenderGhostRed(args.position.position);
                }
            }
            
        }
    }

    //TODO: Stop displaying ghost tower IFF it's not destroyed already (via tower placement)
    void OnNodeMouseExit(object caller, TowerPlaceSelectEventArgs args)
    {
        if (flag_placement_in_progress)
        {
            RemoveGhost();
        }

    }

    void OnNodeSelect(object caller, TowerPlaceSelectEventArgs args)
    {
        //Debug.Log("node selected");

        //TODO: Have an actual, nonmagical cost
        //handle placing tower
        //Can only place if there is no tower already present and we are currently trying to place a tower
        if (flag_placement_in_progress)
        {
           if(!args.node.IsOccupied())
            {
                //Debug.Log("placement attempt");
                if (Resource_Inventory.TryTakeResources(tower_to_place.price_list))
                {
                    RemoveGhostFade();

                    Tower_V2 tower_instance = Instantiate(tower_to_place, new Vector3(0, 0, 0), Quaternion.identity);
                    //if the node can't attach the new tower instance, we destroy it
                    if (!args.node.TryAttachTower(tower_instance))
                    {
                        //Debug.Log("placement attempt failed! destroying instance...");
                        Destroy(tower_instance);
                    }
                    else
                    {
                        //Debug.Log("placement attempt successful!");
                        EndTowerPlacement();
                    }

                    TowerPlacedEvent?.Invoke(this, new TowerV2RefEventArgs(tower_instance));
                }
                else
                {
                    //Write tower construction failed! code here. 
                    //Text bubble explains why placement cant happen. Make sure this isnt spammable!?

                    Text_Bubble.CreateTemporaryTextBubble("Not enough resources!", info_bubble_duration, args.node.gameObject.transform.position );
                }
            }
            else
            {
                //TODO: Tower failed! reason: space is already occupied!
                Text_Bubble.CreateTemporaryTextBubble("Space occuupied!", info_bubble_duration, args.node.gameObject.transform.position);
            }
        }


    }

    //Tower Placement Player Action. 
    //These 2 methods handle assignments to the placement_in_progress flag as well as sending the PlayerActionController transmissions 
    private void StartTowerPlacement(object caller, TowerPlacementEventArgs args)
    {
        PlayerActionController.StartPlayerAction();
        flag_placement_in_progress = true;

        //Find a tower with the name matching our argument
        tower_to_place = Tower_Manager_V2.GetTowerPrefabList().Find(x => x.tower_name == args.tower_name); //fancy predicate using Linq!
        //Debug.Log("Tower to place is ID of: " + tower_to_place.gameObject.ToString());
    }
    private void EndTowerPlacement()
    {
        if(flag_placement_in_progress)
        {
            //Debug.Log("placement event canceled");
            PlayerActionController.EndPlayerAction();
            RemoveGhost();
            flag_placement_in_progress = false;
        }
    }
    //If another Player Action is invoked elsewhere in the game, this method will automatically cancel a tower placement order
    private void CheckForPlacementCancel(object sender, EventArgs e)
    {
        //End our placement event if the flag is true. This relies on the execution flow of local method StartTowerPlacement() invoking StartPlayerAction before setting the flag
        if (flag_placement_in_progress)
        {
            EndTowerPlacement();
        }
    }


    #endregion

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            EndTowerPlacement();
        }
    }

    GameObject InitializeGhost(Tower_V2 tower_to_ghost, Vector3 position)
    {
        //clean up old ghost if it hasn't already been done
        RemoveGhost();

        //new gameobjects
        ghost_tower = new GameObject();
        GameObject render_gameobject = new GameObject();

        //set transform parent
        render_gameobject.transform.SetParent(ghost_tower.transform);

        //init components + copy mesh filter and renderer data from prefab Tower_V2 to render_gameobject
        MeshFilter init_filter = render_gameobject.AddComponent<MeshFilter>();
        MeshFilter tower_to_ghost_filter = tower_to_ghost.GetComponentInChildren<MeshFilter>();
        init_filter.mesh = tower_to_ghost_filter.sharedMesh;
        MeshRenderer init_renderer = render_gameobject.AddComponent<MeshRenderer>();

        //init origin worldspace location
        ghost_tower.transform.position = position;

        //init tower local transform
        render_gameobject.transform.localScale = tower_to_ghost_filter.gameObject.transform.localScale;
        render_gameobject.transform.localRotation = tower_to_ghost_filter.gameObject.transform.localRotation;
        render_gameobject.transform.localPosition = tower_to_ghost_filter.gameObject.transform.localPosition;

        //we set material later
        return ghost_tower;
    }
    //Sets first material found on goj arg to mat and returns true. Returns false if no mesh renderer component found
    //Might be better placed as a global utility...
    bool SetMeshMaterial(GameObject goj, Material mat)
    {
        MeshRenderer renderer;
        if ((renderer = goj.GetComponentInChildren<MeshRenderer>()) != null)
        {
            renderer.material = mat;
            return true;
        }
        else return false;
    }

    void RenderGhostBlue(Vector3 position)
    {
        ghost_tower = InitializeGhost(tower_to_place, position);
        SetMeshMaterial(ghost_tower.gameObject, blue_ghost_material);
    }
    void RenderGhostRed(Vector3 position)
    {
        ghost_tower = InitializeGhost(tower_to_place, position);
        SetMeshMaterial(ghost_tower.gameObject, red_ghost_material);
    }
    void RemoveGhost()
    {
        if (ghost_tower != null) Destroy(ghost_tower);
    }
    void RemoveGhostFade()
    {
        if (ghost_tower != null)
        {
            StartCoroutine(ContinueRemoveGhostFade(2f));
        }
    }

    //assumes ghost material is emmissive AND  transparent. Interpolates both to 0 in given duration
    IEnumerator ContinueRemoveGhostFade(float duration)
    {
        //Debug.Log("Ghost Fade");
        float start_time = Time.time;
        float cur_time = Time.time;

        MeshRenderer mesh_renderer = ghost_tower.GetComponentInChildren<MeshRenderer>();
        Material mat = mesh_renderer.material;

        while ( Mathf.Abs(cur_time - start_time) < duration )
        {
            //Interp emmissive
            float t = Freeman_Utilities.MapValueFromRangeToRange(Mathf.Abs(cur_time - start_time), 0f, duration, 0f, 1f);

            mat.Lerp(mat, fully_transparent_ghost_material, t);

            cur_time = Time.time;
            yield return null;
        }

        //Debug.Log("Ghost Fade Complete");
        RemoveGhost();
    }
}
