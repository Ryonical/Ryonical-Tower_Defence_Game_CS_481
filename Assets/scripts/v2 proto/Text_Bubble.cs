using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Idea: Instantiate these text bubbles to present strings in 3D space rather than on the HUD
//Motivation: Damage numbers, Resource income indicator, Status and info numbers, etc

public class Text_Bubble : MonoBehaviour
{
    #region MEMBERS
    public static float text_move_speed = 3f;
    private string display_message;
    private TextMesh text_mesh;
    #endregion
    #region EVENTS
    public static event System.EventHandler<MonobehaviourEventArgs> RequestAlignToCameraAnglesEvent;
    #endregion
    #region EVENT SUBSCRIPTIONS
    #endregion
    #region EVENT HANDLERS
    #endregion
    #region INIT
    private void Awake()
    {
        if(text_mesh == null)
            text_mesh = gameObject.AddComponent<TextMesh>();
        text_mesh.color = Color.yellow;
        RequestAlignToCameraAnglesEvent?.Invoke(this, new MonobehaviourEventArgs(this));
    }
    #endregion

    public static Text_Bubble CreateTemporaryTextBubble(string message, float duration, GameObject parent)
    {
        GameObject goj = new GameObject();
        Text_Bubble txt = goj.AddComponent<Text_Bubble>();

        goj.transform.SetParent(parent.transform);

        txt.RemoveAfterSeconds(duration);
        txt.UpdateTextMessage(message);
        return txt;
    }
    public static Text_Bubble CreateTemporaryTextBubble(string message, float duration, Vector3 position)
    {
        GameObject goj = new GameObject();
        Text_Bubble txt = goj.AddComponent<Text_Bubble>();

        goj.transform.position = position;

        txt.RemoveAfterSeconds(duration);
        txt.UpdateTextMessage(message);
        return txt;
    }

    void UpdateTextMessage(string message)
    {
        display_message = message;
        text_mesh.text = display_message;
    }

    void RemoveAfterSeconds(float duration)
    {
        StartCoroutine(ContinueRemoveAfterSeconds(duration));
    }

    void KillBubble()
    {
        Destroy(gameObject);
    }

    IEnumerator ContinueRemoveAfterSeconds(float duration)
    {
        float start_time = Time.time;
        float cur_time = Time.time;

        while(Mathf.Abs((cur_time - start_time)) < duration)
        {
            gameObject.transform.position += Vector3.up * text_move_speed * Time.deltaTime;
            cur_time = Time.time;
            yield return null;
        }

        KillBubble();
    }
}
