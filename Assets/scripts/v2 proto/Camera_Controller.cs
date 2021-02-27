using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Scale camera speed based on orthogonal size 

//Idea: 
//  We want players to have control to zoom in and look around. We want to bound the camera to stay in-bounds. 
//  We need to develop a solution that gives the players freedom to move the camera but with zoom in / out limits that are consistent.

//Solution: 
//  The camera (in iso mode) fully zoomed out has a rectangular FOV that we should abide by. 
//  We'll need to write a piece of code that gives us a "cone of moveable space" that the camera can legally be navigated to.
//  This cone should be constructed using the camera's starting angle (which should represent the fully zoomed out camera) and perhaps a raycast sample of how far it is from the map
//  The cone will then have an angled cut so it can glide over the map at a consistent distance above the terrain

//Note: 
//  This solution may benefit from some user-defined shapes instead of the fully automated process I'm describing. I dunno yet! I haven't fully worked out the problem!

public class Camera_Controller : MonoBehaviour
{
    #region MEMBERS
    private float camera_size_default;
    private Vector3 home_position;

    [Min(.1f)]
    public float glide_duration; //this is one sneeze away from being an arbitrary rate
    [Min(.1f)]
    public float input_movement_speed_max; //this is one sneeze away from being an arbitrary rate
    [Min(.1f)]
    public float input_scroll_speed_max; //some magic, arbitrary "size" units rate

    [SerializeField]
    private bool flag_camera_in_use;
    [SerializeField]
    private bool flag_camera_size_in_use;

    [Min(1)]
    public float camera_size_max;
    [Min(1)]
    public float camera_size_min;

    Camera cam;

    //inputs
    private float in_x;
    private float in_y;
    private float in_scroll;
    #endregion
    #region INIT
    private void Start()
    {
        cam = GetComponent<Camera>();
        if(cam == null)
        {
            Debug.LogError("Camera Controller could not attach camera! Is it attached to the same gameobject?");
            Destroy(gameObject);
        }

        //home stuff
        home_position = transform.position;
        camera_size_default = cam.orthographicSize;
        //init
        flag_camera_in_use = false;
        flag_camera_size_in_use = false;
    }
    #endregion
    #region EVENT SUBSCRIPTIONS
    private void OnEnable()
    {
        Text_Bubble.RequestAlignToCameraAnglesEvent += CopyRotationToGameObject;
    }
    private void OnDisable()
    {
        Text_Bubble.RequestAlignToCameraAnglesEvent -= CopyRotationToGameObject;
    }
    #endregion
    #region EVENT HANDLERS
    void CopyRotationToGameObject(object caller, MonobehaviourEventArgs args)
    {
        args.mono.gameObject.transform.rotation = gameObject.transform.rotation;
    }
    #endregion

    private void Update()
    {
        if(!flag_camera_in_use)
        {
            ProcessCameraCommands();
            GetInputAxes();
            HandleInputAxes();
        }

    }

    //Handle the list of camera inputs beyond axes (wasd)
    private void ProcessCameraCommands()
    {
        if(Input.GetButtonDown("Camera Home"))
        {
            GoHome();
        }
        else
        { }
    }

    private void HandleInputAxes()
    {
        transform.position += transform.right * in_x * input_movement_speed_max * Time.deltaTime;
        transform.position += Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, Vector3.up)) * in_y * input_movement_speed_max * Time.deltaTime;

        //maybe check for flag earlier in logic
        if (!flag_camera_size_in_use)
        {
            cam.orthographicSize -= in_scroll * input_scroll_speed_max * Time.deltaTime; //inverting this w/ -= gives us "forward scroll to zoom in" functionality
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, camera_size_min, camera_size_max);
        }
    }

    private void GetInputAxes()
    {
        in_y = Input.GetAxis("Vertical");
        in_x = Input.GetAxis("Horizontal");
        in_scroll = Input.GetAxis("Mouse ScrollWheel");
    }

    private void GoHome()
    {
        InterpCamSize(camera_size_default, 10f);
        GlideTo(home_position);
    }

    private void InterpCamSize(float desired_size, float duration)
    {
        StartCoroutine(ContinueInterpolatingCameraSize(desired_size, duration));
    }

    private void GlideTo(Vector3 target)
    {
        StartCoroutine(ContinueGlidingToTarget(target));
    }

    //NOTE: only size operations should lock camera inputs. Size ops should not handle locking camera!
    IEnumerator ContinueInterpolatingCameraSize(float desired_size, float duration)
    {
        flag_camera_size_in_use = true;

        float tolerance = .1f; 
        float time_start = Time.time;
        while ( Mathf.Abs(desired_size - cam.orthographicSize) > tolerance)
        {
            float time_cur = Time.time;
            float remapped = Freeman_Utilities.MapValueFromRangeToRange((time_cur - time_start), 0f, glide_duration, 0f, 1f);

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, desired_size, remapped);

            yield return null;
        }
        cam.orthographicSize = desired_size;
        flag_camera_size_in_use = false;
    }

    IEnumerator ContinueGlidingToTarget(Vector3 target)
    {
        flag_camera_in_use = true;
        float dist_tolerance = .2f; // dunno what this should be yet

        float time_start = Time.time;

        while ( Mathf.Abs((transform.position - target).magnitude) > dist_tolerance )
        {
            float time_cur = Time.time;

            /* IMPLEMENTED IN ITS OWN FUNCTION NOW
            //gets param assuming time diff mapped to [0, duration] then maps to [0, 1] to use as lerp param
            float orig_value = (time_cur - time_start);
            float normal = Mathf.InverseLerp(0f, glide_duration, orig_value);
            float remapped = Mathf.Lerp(0, 1, normal);
            */

            float remapped = Freeman_Utilities.MapValueFromRangeToRange((time_cur - time_start), 0f, glide_duration, 0f, 1f);

            //now we use the lerp param we just calculated
            transform.position = Vector3.Lerp(transform.position, target, remapped);
            yield return null;
        }
        //we're close enough. Just set the position already!!!!
        transform.position = target;
        flag_camera_in_use = false; //unlock camera movement
    }

    /* MOVED TO FREEMAN_UTILITIES CLASS
    //better in placed a library we make but OH WELL I GUESS
    float MapValueFromRangeToRange(float val_a, float range_a_start, float range_a_end, float range_b_start, float range_b_end)
    {
        float normal = Mathf.InverseLerp(range_a_start, range_a_end, val_a);
        return Mathf.Lerp(range_b_start, range_b_end, normal);
    }
    */
}
