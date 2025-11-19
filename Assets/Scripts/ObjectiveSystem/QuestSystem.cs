using UnityEngine;
using UnityEngine.Events;

public class QuestSystem : MonoBehaviour
{
    public string id;
    public string displayName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] Objectives;
    // Update is called once per frame
    public int questStep;
    private UnityAction<GameObject> objectiveFinishedEventListener;
    public enum QuestState
    {
        STARTED,
        IN_PROGRESS,
        COMPLETED
    };
    public QuestState state;


    void Awake()
    {
        objectiveFinishedEventListener = new UnityAction<GameObject>(objectiveFinishedEventHandler);
    }
    private void Start()
    {
        state = QuestState.STARTED;
    }

    void OnEnable()
    {
        EventManager.StartListening<ObjectiveCompleteEvent, GameObject>(objectiveFinishedEventListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening<ObjectiveCompleteEvent, GameObject>(objectiveFinishedEventListener);
    }

    void objectiveFinishedEventHandler(GameObject go)
    {
        if (go != null && Objectives.Length > 0 && Objectives[0] == go)
        {

            print("Objective Finished");
            questStep++;
        }

    }
}
