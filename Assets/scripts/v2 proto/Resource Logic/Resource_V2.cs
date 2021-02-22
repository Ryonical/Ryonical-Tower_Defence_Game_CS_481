using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//container used to hold a specific resource type
//throw this script onto some physics object!

public class Resource_V2 : MonoBehaviour
{
    //TODO: figure out what the resource names are gonna be
    public enum ResourceType { lives, money, debug_1 };
    #region EVENTS
    public static event System.EventHandler<ResourceV2Args> CollectResourceEvent;
    public static event System.EventHandler<ResourceV2Args> OnSpawn;
    public static event System.EventHandler<ResourceV2Args> OnDespawn;
    public static event System.EventHandler<AddResourceArgs> AddResourceEvent;
    #endregion
    #region MEMBERS 
    public ResourceType resource_name;
    public int resource_amount;
    public Rigidbody attached_rigidbody;
    #endregion
    #region INIT
    private void Awake()
    {
        attached_rigidbody = GetComponent<Rigidbody>();
        //create a rigidbody if there is not one already on the resource
        if(attached_rigidbody == null)
        {
            attached_rigidbody = new Rigidbody();
            attached_rigidbody.mass = Resource_Manager_V2.default_resource_mass;
            attached_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        OnSpawn?.Invoke(this, new ResourceV2Args(this));
    }
    private void Start()
    {
        StartCoroutine(AutoPickupTimer());
    }
    #endregion

    public string GetResourceTypeString(ResourceType rsc)
    {
        string str = "";

        switch (rsc)
        {
            case ResourceType.lives:
                str = "lives";
                break;
            case ResourceType.money:
                str = "money";
                break;
            case ResourceType.debug_1:
                str = "debug resource 1";
                break;
            default:
                str = "<missing name>";
                break;
        }

        return str;
    }

    private void OnMouseOver()
    {
        KillResource();
    }

    private void KillResource()
    {
        StopAllCoroutines();
        CollectResourceEvent?.Invoke(this, new ResourceV2Args(this));
        AddResourceEvent?.Invoke(this, new AddResourceArgs(resource_amount, resource_name));
        OnDespawn?.Invoke(this, new ResourceV2Args(this));
        Destroy( gameObject );
    }

    IEnumerator AutoPickupTimer()
    {
        yield return new WaitForSeconds(5f);


        KillResource();
    }
}

//helper class
public class ResourceV2Args : System.EventArgs
{
    public Resource_V2 resource;

    public ResourceV2Args(Resource_V2 rsc)
    {
        this.resource = rsc;
    }
}
//helper class
public class AddResourceArgs : System.EventArgs
{
    public Resource_V2.ResourceType resource_type;
    public int resource_amount;

    public AddResourceArgs(int rsc_amt, Resource_V2.ResourceType rsc_type)
    {
        this.resource_amount = rsc_amt;
        this.resource_type = rsc_type;
    }
}
