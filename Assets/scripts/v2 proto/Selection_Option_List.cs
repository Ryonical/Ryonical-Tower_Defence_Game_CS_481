using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection_Option_List : MonoBehaviour
{
    public List<Selection_Option> options_list;
    [Header("ID")]
    public string list_id; //This could be the name of a tower, or some other identifier that the designer specifies. We just use this for lookup purposes

}
