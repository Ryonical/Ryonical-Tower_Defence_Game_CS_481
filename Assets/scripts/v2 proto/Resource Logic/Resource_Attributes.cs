using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//helper class for tracking resource attributes
public class Resource_Attributes : MonoBehaviour
{
    public Resource_Attributes(Resource_V2.ResourceType type, int amt)
    {
        resource_type = type;
        resource_amount = amt;
    }

    public Resource_V2.ResourceType resource_type;
    public int resource_amount;
}
