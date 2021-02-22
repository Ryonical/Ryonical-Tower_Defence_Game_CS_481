using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int money;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMoney(int added)
    {
        money += added;
    }

    public int GetMoney()
    {
        return money;
    }

    public void SubtractMoney(int sub)
    {
        money -= sub;
    }

}
