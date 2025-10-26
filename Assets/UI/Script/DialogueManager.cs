using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicesParent;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    [SerializeField] private float typingSpeed = 0.02f;
    [SerializeField] private CharacterManager characterManager;
    private List<dialogueString> dialogueList;
    private int currentDialogueIndex = 0;
    private bool optionSelected;
    // Need to disable player control/movement 

    void Awake()
    {
        dialogueParent.SetActive(false);
        if (choicesParent != null)
        {
            choicesParent.SetActive(false);
        }
    }

    public void DialogueStart(List<dialogueString> textToPrint)
    {
        dialogueParent.SetActive(true);
        // Need to disable player control/movement 
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            inputControl.DisableInput();
            inputControl.StopAllMovement();
        }

        dialogueList = textToPrint;
        currentDialogueIndex = 0;
        optionSelected = false;

        DisableButtons();

        StartCoroutine(PrintDialogue());
    }

    private void DisableButtons()
    {
        if (choicesParent != null)
        {
            choicesParent.SetActive(false);
        }
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        option2Button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
    }

    private IEnumerator PrintDialogue()
    {
        while (currentDialogueIndex < dialogueList.Count)
        {
            dialogueString line = dialogueList[currentDialogueIndex];

            line.startDialogueEvent?.Invoke();

            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));
                if (choicesParent != null)
                    choicesParent.SetActive(true);

                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TextMeshProUGUI>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TextMeshProUGUI>().text = line.answerOption2;

                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil(() => optionSelected);
            }
            else
            {
                if (choicesParent != null)
                    choicesParent.SetActive(false);

                yield return StartCoroutine(TypeText(line.text));
            }

            line.endDialogueEvent?.Invoke();

            optionSelected = false;
        }

        DialogueStop();
    }

    private void HandleOptionSelected(int indexJump)
    {
        optionSelected = true;
        DisableButtons();

        currentDialogueIndex = indexJump;
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        string speakerText = dialogueList[currentDialogueIndex].speaker;
        string fullText = !string.IsNullOrEmpty(speakerText) ? $"{speakerText}: {text}" : text;

        foreach (char letter in fullText.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        }

        if (dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
        }

        currentDialogueIndex++;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);

        if (choicesParent != null)
            choicesParent.SetActive(false);

        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            inputControl.EnableInput();
        }
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
    private IEnumerator InvokeWithDelay(UnityEvent unityEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        unityEvent?.Invoke();
    }
}

