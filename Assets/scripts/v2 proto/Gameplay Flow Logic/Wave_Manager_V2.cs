using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Singleton class! Don't have more than 1!

public class Wave_Manager_V2 : MonoBehaviour
{

    #region EVENTS
    public static event System.EventHandler<System.EventArgs> AllWavesCompletedEvent;
    public static event System.EventHandler<System.EventArgs> WaveStartEvent;
    public static event System.EventHandler<System.EventArgs> WaveEndEvent;
    #endregion
    #region MEMBERS
    //list of list of prefabs
    public List<Wave_V2> wave_list;
    
    private static bool flag_wave_in_progress;
    private int cur_wave_index = 0;

    #endregion
    #region EVENT SUBSCRIPTION
    void OnEnable()
    {
        UI_MGR_v2.OnStartWave += StartWave; //add StartWave() to click event
    }
    void OnDisable()
    {
        UI_MGR_v2.OnStartWave -= StartWave; //remove StartWave() from click event
    }
    #endregion
    #region INIT
    private void Awake()
    {
        flag_wave_in_progress = false;
    }
    #endregion

    public int GetCurrentWaveIndex() { return cur_wave_index; }
    public int GetCurrentWaveCount() { return cur_wave_index + 1; }

    void StartWave()
    {
        //check for remaining waves
        if (cur_wave_index >= wave_list.Count)
        {
            Debug.Log("StartWave() fail @ index");
        }
        //check for wave in progress
        else if (flag_wave_in_progress)
        {
            Debug.Log("StartWave() fail @ flag");

        }
        else
        {
            Debug.Log("Wave manager: starting wave");
            flag_wave_in_progress = true; //I'll bet its safer to leave this here 
            WaveStartEvent?.Invoke(this, System.EventArgs.Empty);
            StartCoroutine(ContinueWave(wave_list[cur_wave_index]));
        }
    }

    public static bool GetWaveStatus()
    {
        return flag_wave_in_progress;
    }
    private bool CheckForEnemiesAlive()
    {
        if (Enemy_Manager_V2.GetEnemyList().Count > 0)
        {
            return true;
        }
        else return false;
    }

    IEnumerator ContinueWave(Wave_V2 cur_wave)
    {
        foreach (Enemy_V2 en in cur_wave.enemy_list)
        {
            if (en == null) Debug.Log("WaveManager ContinueWave(): null found!");
            else
            {
                //clone
                Instantiate(en, new Vector3(0, 0, 0), Quaternion.identity); //dont bother setting a location here. should be done in enemy class Start()

                //spawn_cooldown accessed from prefab
                yield return new WaitForSeconds(en.spawn_cooldown);
            }
        }


        StartCoroutine(CheckForWaveEnd());
    }
    //TODO: refactor CheckForEnemiesAlive to be less dumb
    IEnumerator CheckForWaveEnd()
    {
        Debug.Log("WaveManager: Check for wave end");
        while (CheckForEnemiesAlive()) //Can impl this better
        {
            yield return new WaitForSeconds(.5f);
        }

        if (cur_wave_index < wave_list.Count)
        {
            Debug.Log("Wave manager: wave complete!");
            cur_wave_index++;


            if(cur_wave_index >= wave_list.Count)
            {
                Debug.Log("Wave manager: all waves completed successfully!");
                AllWavesCompletedEvent?.Invoke(this, System.EventArgs.Empty);
                GameOverHandler.DoGameWin();
            }
        }

        WaveEndEvent?.Invoke(this, System.EventArgs.Empty);
        flag_wave_in_progress = false;
    }
}


