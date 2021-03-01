using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection_Option_List : MonoBehaviour
{


    public List<Selection_Option> options_list;
    [Header("ID might be obsolete now")]
    public string list_id; //This could be the name of a tower, or some other identifier that the designer specifies. We just use this for lookup purposes. MIIIGHT be obsolete now LOL
    [Header("AutoAdded")]
    public MonoBehaviour attached_object;

    public void AttachTo(MonoBehaviour obj)
    {
        attached_object = obj;
        foreach(Selection_Option opt in options_list)
        {
            opt.attached_object = obj;
        }
    }

}
