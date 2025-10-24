using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

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
    private bool playerInRange = false;
    private bool isTalking = false;
    // public Transform player;
    [SerializeField] private CharacterManager characterManager;
    private Transform player => characterManager != null && characterManager.GetPlayerCharacter() != null ? characterManager.GetPlayerCharacter().transform : null;
    private Animator anim;
    [SerializeField] private DialogueManager dialogueManager;

    // [TextArea(2, 5)]
    // public List<string> npcDialogue = new List<string>();
    [Header("Dialogue Settings")]
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();

    void Start()
    {
        anim = GetComponent<Animator>();
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
            EndConversation();
        }
    }

    void Update()
    {
        if (playerInRange && player != null)
        {
            float rotateSpeed = 5f;
            Vector3 faceDirection = player.position;
            faceDirection.y = transform.position.y;
            Quaternion targetRotation = Quaternion.LookRotation(faceDirection - transform.position);
            // transform.LookAt(faceDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed");
            StartConversation();
            anim.SetTrigger("Talk");
        }
        else if (playerInRange && Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Conversation ended1.");
            EndConversation();
        }
        else if (!playerInRange)
        {
            Debug.Log("Conversation ended2.");
            EndConversation();
        }
    }

    void StartConversation()
    {
        isTalking = true;
        // List<string> dialogue = new List<string>()
        // {
        //     "NPC: Hello there, traveler!",
        //     "The road ahead is dangerous.",
        //     "Take this potion, it might help you.",
        //     "Farewell, and good luck!"
        // };
        // dialogueManager.StartDialogue(dialogue);

        // if (dialogueManager != null && npcDialogue.Count > 0)
        // {
        //     List<string> prefixedDialogue = new List<string>();
        //     foreach (string line in npcDialogue)
        //     {
        //         prefixedDialogue.Add(gameObject.name + ": " + line);
        //     }
        //     dialogueManager.StartDialogue(prefixedDialogue);
        // }
        if (dialogueManager != null && dialogueStrings.Count > 0)
        {
            // if (characterManager != null && characterManager.GetPlayerCharacter() != null)
            // {
            //     CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            //     inputControl.DisableInput();
            // }

            dialogueManager.DialogueStart(dialogueStrings);
            isTalking = true;
        }
    }

    void EndConversation()
    {
        if (isTalking)
        {
            isTalking = false;

            // if (characterManager != null && characterManager.GetPlayerCharacter() != null)
            // {
            //     CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            //     inputControl.EnableInput();
            // }
        }
    }

}
