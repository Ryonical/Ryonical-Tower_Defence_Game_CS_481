using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacementNode : MonoBehaviour
{
    #region EVENTS
    public static event System.EventHandler<TowerPlaceSelectEventArgs> TowerNodeSelectEvent;
    public static event System.EventHandler<TowerPlaceSelectEventArgs> NodeMouseEnterEvent;
    public static event System.EventHandler<TowerPlaceSelectEventArgs> NodeMouseExitEvent;

    private bool mouse_currently_over;
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Tower_V2.OnDespawn += DetachTower;
        UI_MGR_v2.RequestPlacementNodePing += PingPlacementNode;
    }


    private void OnDisable()
    {
        Tower_V2.OnDespawn -= DetachTower;
        UI_MGR_v2.RequestPlacementNodePing -= PingPlacementNode;
    }
    #endregion
    #region MEMBERS
    public Tower_V2 attached_tower;
    #endregion
    #region INIT
    private void Start()
    {
        mouse_currently_over = false;
    }
    #endregion

    private void OnMouseDown()
    {
        TowerNodeSelectEvent?.Invoke(this, new TowerPlaceSelectEventArgs(transform, this) );
    }
    private void OnMouseEnter()
    {
        NodeMouseEnterEvent?.Invoke(this, new TowerPlaceSelectEventArgs(transform, this));
        mouse_currently_over = true;
    }
    private void OnMouseExit()
    {
        NodeMouseExitEvent?.Invoke(this, new TowerPlaceSelectEventArgs(transform, this));
        mouse_currently_over = false;
    }

    public bool IsOccupied()
    {
        return (attached_tower != null) ;
    }

    private void PingPlacementNode(object sender, EventArgs args)
    {
        if(mouse_currently_over)
        {
            NodeMouseExitEvent?.Invoke(this, new TowerPlaceSelectEventArgs(transform, this));
            NodeMouseEnterEvent?.Invoke(this, new TowerPlaceSelectEventArgs(transform, this));
        }
    }

    //Tries to attach tower to this node. Will return false if there is already a tower at this node.
    public bool TryAttachTower(Tower_V2 tow)
    {
        if(attached_tower == null)
        {
            attached_tower = tow;
            tow.transform.position = transform.position;
            return true;
        }
        else return false;
    }

    private void DetachTower(object caller, TowerV2RefEventArgs args)
    {
        attached_tower = null;
    }
}

//helper class
public class TowerPlaceSelectEventArgs : System.EventArgs
{
    public Transform position;
    public TowerPlacementNode node;
    
    public TowerPlaceSelectEventArgs(Transform pos, TowerPlacementNode nd)
    {
        this.position = pos;
        this.node = nd;
    }
}
