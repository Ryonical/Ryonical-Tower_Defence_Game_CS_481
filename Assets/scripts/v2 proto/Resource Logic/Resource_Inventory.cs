using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//added
using System.Linq;

//SINGLETON! Don't have more than 1!


public class Resource_Inventory : MonoBehaviour
{
    

    #region MEMBERS
    public static List<Resource_Attributes> resource_inventory;
    #endregion
    #region EVENTS

    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Resource_V2.AddResourceEvent += AddResourcesToInventory;
    }
    private void OnDisable()
    {
        Resource_V2.AddResourceEvent -= AddResourcesToInventory;
    }
    #endregion
    #region EVENT HANDLERS
    void AddResourcesToInventory(object caller, AddResourceArgs args)
    {
        Resource_Attributes temp = FindResourceAttribute(args.resource_type);

        if (temp != null)
            temp.resource_amount += args.resource_amount;

        //PrintResourceInventory();
    }
    #endregion
    #region INIT
    private void Start()
    {
        if(resource_inventory == null)
        {
            resource_inventory = new List<Resource_Attributes>();
            
            //Debug.Log("Resource Inventory init successfully!");
        }
        else
        {
            resource_inventory.Clear();
        }

        foreach (Resource_V2.ResourceType rsc_t in System.Enum.GetValues(typeof(Resource_V2.ResourceType)))
        {
            Resource_Attributes temp = FindResourceAttribute(rsc_t);

            if (temp == null)
            {
                temp = gameObject.AddComponent<Resource_Attributes>();
                resource_inventory.Add(temp);
            }

            temp.resource_type = rsc_t;
            temp.resource_amount = 0;

            //Debug logging stuff
            //Resource_Attributes temp = resource_inventory.Find(x => x.resource_type == rsc_t);
            //Debug.Log("Init Resource_Attributes to Inv: " + temp.resource_type.ToString() + " " + temp.resource_amount.ToString());
        }
    }
    #endregion
    
    //will setting this as static cause issues??
    private static Resource_Attributes FindResourceAttribute(Resource_V2.ResourceType type)
    {
        Resource_Attributes temp = null;
        foreach (Resource_Attributes ra in resource_inventory)
        {
            if (ra.resource_type == type)
            {
                temp = ra;
                return temp;
            }
        }
        return temp;
    }

    public static int GetResourceAmount(Resource_V2.ResourceType type)
    {
        Resource_Attributes temp = FindResourceAttribute(type);
        return temp.resource_amount;
    }
    //sets resource_amount of type argument to amount argument
    public static void SetResourceAmount(Resource_V2.ResourceType type, int amount)
    {
        Resource_Attributes temp = FindResourceAttribute(type);
        temp.resource_amount = amount;
    }
    //will add argument amount to the resource_amount of type argument
    public static void AppendResourceAmount(Resource_V2.ResourceType type, int amount)
    {
        Resource_Attributes temp = FindResourceAttribute(type);
        temp.resource_amount += amount;
    }
    public static void AddResourcesToInventory(List<Resource_Attributes> ras_to_add)
    {
        foreach (Resource_Attributes ra in ras_to_add)
        {
            AppendResourceAmount(ra.resource_type, ra.resource_amount);
        }
    }
    public static void PrintResourceInventory()
    {
        foreach(Resource_Attributes ra in resource_inventory)
        {
            Debug.Log("rsc: " + ra.resource_type.ToString() + ": " + ra.resource_amount.ToString());
        }
    }

    //TODO: Towers cost a LIST of resources. This ain't gonna work. Refactor to accept param Resource_Attributes instead of this garbage!
    //Tries to take the amount of resources asked out of the inventory. Return bool indicates success/failure
    public static bool TryTakeResource(Resource_V2.ResourceType type, int amount)
    {
        Resource_Attributes temp = FindResourceAttribute(type);
        if(temp.resource_amount >= amount)
        {
            //no need to clamp
            temp.resource_amount -= amount;
            return true;
        }
        else return false;
    }
    public static bool TryTakeResource(Resource_Attributes attributes)
    {
        Resource_Attributes inventory_ra = FindResourceAttribute(attributes.resource_type);
        if (inventory_ra.resource_amount >= attributes.resource_amount)
        {
            //no need to clamp
            inventory_ra.resource_amount -= attributes.resource_amount;
            return true;
        }
        else return false;
    }
    public static bool TryTakeResources(List<Resource_Attributes> take_list)
    {
        if (CheckResourcesAvailable(take_list))
        {
            foreach (Resource_Attributes ra in take_list)
            {
                if (TryTakeResource(ra)) //method should fully handle taking the resource at this point; otherwise we have a bug somewhere...
                {
                }
                else
                {
                    Debug.LogError("Resource_Inventory.cs: Took more of a resource than in inventory! This is an internal script bug!");
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void RemoveLives(int lives)
    {
        if (lives <= 0) return; //someone is trying to goof my system!!! I ain't gonna have it...

        Resource_Attributes cur_lives_attribute = FindResourceAttribute(Resource_V2.ResourceType.lives);

        if(cur_lives_attribute.resource_amount > lives)
        {
            cur_lives_attribute.resource_amount -= lives;
        }
        else
        {
            cur_lives_attribute.resource_amount = 0;
            GameOverHandler.DoGameOver();
        }
    }


    public static bool CheckResourcesAvailable(List<Resource_Attributes> check_list)
    {
        foreach(Resource_Attributes ra in check_list)
        {
            if(GetResourceAmount(ra.resource_type) < ra.resource_amount)
            {
                return false;
            }
        }
        return true;
    }
}
