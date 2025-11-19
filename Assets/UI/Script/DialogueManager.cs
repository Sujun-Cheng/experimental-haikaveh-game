using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private PlayerInput playerInput;
    private bool nextLinePressed;
    public bool IsDialogueActive { get; private set; } = false;
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Dialogue.NextLine.performed += (ctx) =>
        {
            nextLinePressed = ctx.ReadValueAsButton();
        };
        playerInput.Dialogue.NextLine.canceled += (ctx) =>
        {
            nextLinePressed = ctx.ReadValueAsButton();
        };
        playerInput.Enable();
        playerInput.Dialogue.Disable();
        dialogueParent.SetActive(false);
        if (choicesParent != null)
        {
            choicesParent.SetActive(false);
        }
    }

    public void DialogueStart(List<dialogueString> textToPrint)
    {
        if (IsDialogueActive) return;
        IsDialogueActive = true;
        dialogueParent.SetActive(true);
        // Need to disable player control/movement 
        //Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        playerInput.Dialogue.Enable();
        Cursor.lockState = CursorLockMode.None;
        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            characterManager.PlayerInput.CharacterSwitching.Disable();
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            print($"disabling control for character {characterManager.GetPlayerCharacter()}, using input controll {inputControl}");
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

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            // If space bar pressed while typing, instantly show the rest of the line
            if (nextLinePressed)
            {
                dialogueText.text = fullText;
                nextLinePressed = false;  // reset to avoid skipping right away
                break;
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        // Wait for player input to proceed (unless it's a question line)
        if (!dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => nextLinePressed);
            nextLinePressed = false;
        }

        // Stop dialogue if marked as end
        if (dialogueList[currentDialogueIndex].isEnd)
        {
            DialogueStop();
            yield break;
        }

        currentDialogueIndex++;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        if (choicesParent != null)
            choicesParent.SetActive(false);
        playerInput.Dialogue.Disable();
        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            print($"reenabling player input: {characterManager.GetPlayerCharacter().name}");
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            inputControl.EnableInput();

            characterManager.PlayerInput.CharacterSwitching.Enable();
        }

        IsDialogueActive = false;
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
    private IEnumerator InvokeWithDelay(UnityEvent unityEvent, float delay)
    {
        yield return new WaitForSeconds(delay);
        unityEvent?.Invoke();
    }
}

