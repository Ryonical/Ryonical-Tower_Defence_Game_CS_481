using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lives : MonoBehaviour
{
    public int lives;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lives <= 0)
        {
            //end game/ go to menu/ falure screen
        }
    }

    public void SubtractLives(int healthRemaining)
    {
        lives -= healthRemaining;
    }

    public int GetLife()
    {
        return lives;
    }
}
