using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//added by me:
using System.Linq;

public class TowerPlacementListener : MonoBehaviour
{
    #region MEMBERS
    [SerializeField]
    private bool flag_placement_in_progress;
    private Tower_V2 tower_to_place;
    #endregion
    #region EVENTS
    public static event System.EventHandler<TowerV2RefEventArgs> TowerPlacedEvent;
    #endregion
    #region EVENT SUBSCRIPTIONS

    private void OnEnable()
    {
        TowerPlacementNode.TowerNodeSelectEvent += OnNodeSelect;
        UI_MGR_v2.OnStartTowerPlacement += StartTowerPlacement;
    }
    private void OnDisable()
    {
        TowerPlacementNode.TowerNodeSelectEvent -= OnNodeSelect;
        UI_MGR_v2.OnStartTowerPlacement -= StartTowerPlacement;
    }
    #endregion
    #region EVENT HANDLERS

    void OnNodeSelect(object caller, TowerPlaceSelectEventArgs args)
    {
        //Debug.Log("node selected");

        //TODO: Have an actual, nonmagical cost
        //handle placing tower
        if (flag_placement_in_progress && Resource_Inventory.TryTakeResources(Resource_V2.ResourceType.money, 35))
        {
            //Debug.Log("placement attempt");

            Tower_V2 tower_instance = Instantiate(tower_to_place, new Vector3(0, 0, 0), Quaternion.identity);
            //if the node can't attach the new tower instance, we destroy it
            if(!args.node.TryAttachTower(tower_instance))
            {
                //Debug.Log("placement attempt failed! destroying instance...");
                Destroy(tower_instance);
            }
            else
            {
                //Debug.Log("placement attempt successful!");
                flag_placement_in_progress = false;
            }

            TowerPlacedEvent?.Invoke(this, new TowerV2RefEventArgs(tower_instance));
        }


    }
    //starts the tower placement behavior
    void StartTowerPlacement(object caller, TowerPlacementEventArgs args)
    {
        flag_placement_in_progress = true;

        //Find a tower with the name matching our argument
        tower_to_place = Tower_Manager_V2.GetTowerPrefabList().Find(x => x.tower_name == args.tower_name); //fancy predicate using Linq!
        //Debug.Log("Tower to place is ID of: " + tower_to_place.gameObject.ToString());
    }
    #endregion

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            CancelTowerPlacement();
        }
    }
    private void CancelTowerPlacement()
    {
        //Debug.Log("placement event canceled");
        flag_placement_in_progress = false;
    }

}
