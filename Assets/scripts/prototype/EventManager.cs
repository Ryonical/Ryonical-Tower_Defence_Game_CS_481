using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//kinda more of a GUI mgr now...
public class EventManager : MonoBehaviour
{
    //delegate stuff that would be nicer to use than the spaghetti im gonna write...
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    
    public ResourceManager resource_mgr;
    private string resource_display_string;
    public Lives life;
    public Money money;
    public List<GameObject> towerPlaces;
    public TowerMGR towermgr;

    private Vector3 currentMousePos;
    public float shortDist = float.MaxValue;
    public GameObject closest = null;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, 5, 100, 30), "Start Wave"))
        {
            if(life.GetLife() > 0)
            {
                OnClicked?.Invoke();
            }
            
        }
        if (GUI.Button(new Rect(Screen.width / 1.2f - 50, 300, 100, 30), "Buy Tower 1"))
        {
            StartCoroutine(UpdateGUIElements());
        }
        GUI.Box(new Rect(5, 5, 300, 30), resource_display_string);
        //GUI.Box(new Rect(1000, 300, 300, 30), "thing");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    //Dunno if needed yet
    IEnumerator UpdateGUIElements()
    {
        //resets shortestDist for this use
        shortDist = float.MaxValue;
        //these are supposed to be the positions of the closest tower placement point
        float x;
        float y;
        float z;
        
        //this should get the current position of the mouse
        currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //go through all of the points of towers
        foreach (GameObject place in towerPlaces)
        {
            //finds shoretest distance
            if (shortDist > Vector3.Distance(currentMousePos, place.transform.position))
            {
                //saves shortest distance for above if
                shortDist = Vector3.Distance(currentMousePos, place.transform.position);
                //saves the position
                closest = place;
            }
        }
        //sets the position to be placed
        x = closest.transform.localPosition.x;
        y = closest.transform.localPosition.y;
        z = closest.transform.localPosition.z;
        //creates the tower immediatly
        towermgr.CreateTower(x,y,z);
        yield return new WaitForSeconds(.2f);

    }
    
    // Update is called once per frame
    void Update()
    {
        resource_display_string = "";
        resource_display_string += "Lives : ";
        resource_display_string += life.GetLife();
        resource_display_string += " Money : ";
        resource_display_string += money.GetMoney();
    }

    public void UpdateResourceRecord()
    {

    }
}
