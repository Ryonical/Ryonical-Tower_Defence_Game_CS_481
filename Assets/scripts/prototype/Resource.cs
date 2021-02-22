using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//container used to hold a specific resource type
//throw this script onto some physics object!

public class Resource : MonoBehaviour
{

    #region EVENTS
    public static event System.EventHandler<CollectResourceArgs> CollectResourceEvent;
    #endregion

    //TODO: figure out what the resource names are gonna be
    public enum ResourceType { lives, money, debug_1 };

    #region MEMBERS 
    public ResourceType resource_name;
    public int resource_amount;
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
        CollectResourceEvent?.Invoke(this, new CollectResourceArgs(this) );
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//helper class
public class CollectResourceArgs : System.EventArgs
{
    public Resource resource;

    public CollectResourceArgs(Resource rsc)
    {
        this.resource = rsc;
    }
}
