using UnityEngine;
using UnityEngine.Events;

public class CheckpointSystem : MonoBehaviour
{
    public GameObject currentWaypoint;
    private UnityAction<GameObject> deathEventListener;
    private UnityAction<GameObject> waypointChangeListener;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        deathEventListener = new UnityAction<GameObject>(deathEventHandler);
        waypointChangeListener = new UnityAction<GameObject>(waypointChangeEventHandler);
    }

    // Update is called once per frame
    void OnEnable()
    {
        EventManager.StartListening<DeathEvent, GameObject>(deathEventListener);
        EventManager.StartListening<CheckPassedEvent, GameObject>(waypointChangeListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening<DeathEvent, GameObject>(deathEventListener);
        EventManager.StopListening<CheckPassedEvent, GameObject>(waypointChangeListener);
    }

    void deathEventHandler(GameObject go)
    {

        print("Death event triggered");
        if (currentWaypoint != null)
        {

            Transform waypointPos = currentWaypoint.transform;
            go.transform.position = new Vector3(waypointPos.position.x + 1, waypointPos.position.y, waypointPos.position.z);
        }

        
    }

    void waypointChangeEventHandler(GameObject go)
    {
        //AudioSource.PlayClipAtPoint(this.explosionAudio, worldPos, 1f);
        currentWaypoint = go;


    }
}
