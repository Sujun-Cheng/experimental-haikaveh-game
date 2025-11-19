using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class dialogueString
{
    public string speaker;
    public string text; //Text from NPC
    public bool isEnd; //If the line is the final line

    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;

    [Header("Triggered Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;
}

public class NPCInteraction : MonoBehaviour
{
    public GameObject interactionPrompt;

    [Header("References")]
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private DialogueManager dialogueManager;
    private Animator anim;
    private PlayerInput playerInput;

    [Header("Dialogue Settings")]
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();

    private bool playerInRange = false;
    private bool isTalking = false;
    private Transform player => characterManager != null && characterManager.GetPlayerCharacter() != null 
        ? characterManager.GetPlayerCharacter().transform 
        : null;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        playerInput = new PlayerInput();
        playerInput.NPCInteraction.InitializeDialogue.performed += (ctx) =>
        {
            if (playerInRange)
                StartConversation();
        };
        playerInput.NPCInteraction.EndDialogue.performed += (ctx) =>
        {
            if (playerInRange)
                EndConversation();
        };

        playerInput.Enable();
        playerInput.NPCInteraction.Disable();
    }

    private void Start()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            playerInRange = true;
            interactionPrompt?.SetActive(true);
            playerInput.NPCInteraction.Enable();
        }
    }

    private void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            playerInRange = false;
            interactionPrompt?.SetActive(false);
            playerInput.NPCInteraction.Disable();
            EndConversation();
        }
    }

    private void Update()
    {
        if (playerInRange && player != null)
        {
            float rotateSpeed = 5f;
            Vector3 faceDirection = player.position;
            faceDirection.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(faceDirection - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    private void StartConversation()
    {
        if (isTalking || dialogueManager == null || dialogueStrings.Count == 0) return;

        isTalking = true;
        anim?.SetTrigger("Talk");

        dialogueManager.DialogueStart(new List<dialogueString>(dialogueStrings));
    }

    private void EndConversation()
    {
        if (!isTalking) return;

        isTalking = false;
        dialogueManager?.DialogueStop();
    }
}