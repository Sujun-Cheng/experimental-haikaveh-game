using UnityEngine;
using System.Collections.Generic;

public class StartOfGameDialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private List<dialogueString> startingDialogue = new List<dialogueString>();

    void Start()
    {
        if (dialogueManager != null && startingDialogue.Count > 0)
        {
            dialogueManager.DialogueStart(startingDialogue);
        }
    }
}