using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMGR : MonoBehaviour
{
    public Tower tower;
    public Money money;
    public List<Tower> towers;
    public GameObject towerPrefab;
    public WaveManager wave;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateTower(float x, float y, float z)
    {
        if (money.GetMoney() >= 100)
        {
            GameObject newTow = Instantiate(towerPrefab) as GameObject;
            money.SubtractMoney(100);
            newTow.transform.position = new Vector3 (x, y, z);
            wave.AddTower(newTow);
        }
    }
}
