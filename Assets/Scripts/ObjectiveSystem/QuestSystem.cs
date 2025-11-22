using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

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
    public GameObject Arrow;
    // Update is called once per frame
    public int questStep;
    private UnityAction<Objective> objectiveFinishedEventListener;
    private UnityAction<Objective> objectiveUpdateEventListener;
    private UnityAction<string, Color> objectiveDialogueEventListener;
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
        objectiveDialogueEventListener = new UnityAction<string, Color>(objectiveDialogueEventHandler);
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

    private void Update()
    {
        if (Arrow != null && state != QuestState.COMPLETED)
        {
            if (questStep < Objectives.Length)
            {
                Objective objective = Objectives[questStep];
                if (objective != null)
                {
                    Vector3 pos = objective.transform.position;
                    pos.y = Arrow.transform.position.y;
                    Arrow.transform.LookAt(pos);
                }
            }

        }
    }

    void OnEnable()
    {
        EventManager.StartListening<ObjectiveCompleteEvent, Objective>(objectiveFinishedEventListener);
        EventManager.StartListening<ObjectiveUpdateEvent, Objective>(objectiveUpdateEventListener);
        EventManager.StartListening<ObjectiveDialogueEvent, string, Color>(objectiveDialogueEventListener);

    }

    private void OnDisable()
    {
        EventManager.StopListening<ObjectiveCompleteEvent, Objective>(objectiveFinishedEventListener);
        EventManager.StopListening<ObjectiveUpdateEvent, Objective>(objectiveUpdateEventListener);
        EventManager.StopListening<ObjectiveDialogueEvent, string, Color>(objectiveDialogueEventListener);

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

    void objectiveDialogueEventHandler(string text, Color color)
    {
        objectiveUI.UpdateFlavorText(text, color);

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
