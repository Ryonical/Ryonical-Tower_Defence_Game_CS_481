using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource_Confetti : MonoBehaviour
{
    //members
    public List<Resource_V2> resource_list;

    /*
    //ctors WAIT THIS AINT GONNA WORK (monobehaviors dont allow constructors)
    public Resource_Confetti(List<Resource_V2> list)
    {
        resource_list = list;
    }
    */

    public static void SpawnConfetti(List<Resource_V2> list, Vector3 location)
    {
        GameObject goj = new GameObject("Resource Confetti");
        goj.transform.position = location;
        Resource_Confetti conf = goj.AddComponent<Resource_Confetti>();
        conf.resource_list = list;
    }


    private void Start()
    {
        StartCoroutine(ContinueSpawningResources());
    }
    void KillConfetti()
    {
        Destroy(gameObject);
    }

    //TODO: finalize instantiation y value (right now it spawns an arbitrary amount above the Resource_Confetti's transform.position)
    IEnumerator ContinueSpawningResources()
    {
        //Debug.Log("spawning confetti!");

        foreach (Resource_V2 rsc in resource_list)
        {
            Resource_V2 resource_spawned;
            resource_spawned = Instantiate(rsc, new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z), Quaternion.identity );

            resource_spawned.attached_rigidbody.AddForce(new Vector3(
                Random.Range(-1f, 1f)
                , Random.Range(-1f, 1f)
                , Random.Range(.6f, .9f)
                ) * 55f, ForceMode.Impulse);

            yield return new WaitForSeconds(.2f);
        }

        KillConfetti();
    }
}
