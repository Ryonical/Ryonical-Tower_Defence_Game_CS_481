using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    public static void DoGameOver()
    {
        SceneManager.LoadScene(2);
    }

    public static void DoGameWin()
    {
        SceneManager.LoadScene(3);
    }
}
