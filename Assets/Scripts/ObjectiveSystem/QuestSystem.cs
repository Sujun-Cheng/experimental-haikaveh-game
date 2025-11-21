using UnityEngine;
using UnityEngine.Events;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }
    [Header("UI Reference")]
    public ObjectiveUI objectiveUI;
    public GameObject winTextObject;
    [Header("Quest System Status")]
    public string id;
    public string displayName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Objective[] Objectives;

    // Update is called once per frame
    public int questStep;
    private UnityAction<Objective> objectiveFinishedEventListener;
    private UnityAction<Objective> objectiveUpdateEventListener;
    public UnityEvent onAllObjectivesComplete;

    public enum QuestState
    {
        NOT_STARTED,
        IN_PROGRESS,
        COMPLETED
    };
    public QuestState state;


    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        objectiveFinishedEventListener = new UnityAction<Objective>(objectiveFinishedEventHandler);
        objectiveUpdateEventListener = new UnityAction<Objective>(objectiveProgressUpdateEventHandler);
    }
    private void Start()
    {
        state = QuestState.IN_PROGRESS;
        winTextObject.SetActive(false);
        for (int i = 0; i < Objectives.Length; i ++)
        {
            Objectives[i].State = Objective.ObjectiveState.NOT_STARTED;
            Objectives[i].gameObject.SetActive(false);
        }
        questStep = 0;
        Objectives[questStep].State = Objective.ObjectiveState.IN_PROGRESS;
        Objectives[questStep].gameObject.SetActive(true);

        UpdateUI();

    }

    void OnEnable()
    {
        EventManager.StartListening<ObjectiveCompleteEvent, Objective>(objectiveFinishedEventListener);
        EventManager.StartListening<ObjectiveUpdateEvent, Objective>(objectiveUpdateEventListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening<ObjectiveCompleteEvent, Objective>(objectiveFinishedEventListener);
        EventManager.StopListening<ObjectiveUpdateEvent, Objective>(objectiveUpdateEventListener);

    }

    void objectiveFinishedEventHandler(Objective go)
    {
        Debug.Log($"Received objective finish event {go}, attempting to reconcile with {Objectives[questStep]}");
        if (go != null && Objectives.Length > questStep && Objectives[questStep] == go)
        {
            print("Objective Finished");

            questStep++;
            UpdateUI();
            if (questStep >= Objectives.Length)
            {
                state = QuestState.COMPLETED;
                WinGame();
            }
            else
            {
                Objectives[questStep].State = Objective.ObjectiveState.IN_PROGRESS;
                Objectives[questStep].gameObject.SetActive(true);
            }
        }
        

    }

    void objectiveProgressUpdateEventHandler(Objective go)
    {
        if (go != null && Objectives[questStep] == go)
        {
            UpdateUI();
        }
       
    }
    public void WinGame()
    {
        Debug.Log("ALL OBJECTIVES COMPLETE! YOU WIN!"); // need to add game win UI later
        winTextObject.SetActive(true);
        onAllObjectivesComplete?.Invoke();
    }
    private void UpdateUI()
    {
        if (objectiveUI != null)
        {
            objectiveUI.UpdateObjectives(
                Objectives,
                questStep
            );
        }
    }
}
