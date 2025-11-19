using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choicesParent;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.02f;
    [SerializeField] private CharacterManager characterManager;

    private List<dialogueString> dialogueList;
    private int currentDialogueIndex;
    private bool optionSelected;
    private bool nextLinePressed;

    private PlayerInput playerInput;
    public bool IsDialogueActive { get; private set; } = false;
    private bool isStoppingDialogue = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Dialogue.NextLine.performed += (ctx) => nextLinePressed = ctx.ReadValueAsButton();
        playerInput.Dialogue.NextLine.canceled += (ctx) => nextLinePressed = ctx.ReadValueAsButton();

        playerInput.Enable();
        playerInput.Dialogue.Disable();

        dialogueParent.SetActive(false);
        choicesParent?.SetActive(false);
    }

    void Update()
    {
        if (IsDialogueActive && Keyboard.current.escapeKey.wasPressedThisFrame && !isStoppingDialogue)
        {
            DialogueStop();
        }
    }

    public void DialogueStart(List<dialogueString> textToPrint)
    {
        PauseMenuToggle.instance.DisablePauseInput();
        PauseMenuToggle.instance.blockPause = true;
        // Stop previous dialogue if needed
        if (IsDialogueActive)
            DialogueStop();

        IsDialogueActive = true;
        dialogueParent.SetActive(true);
        playerInput.Dialogue.Enable();
        playerInput.PauseMenu.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player control
        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            characterManager.PlayerInput.CharacterSwitching.Disable();
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            inputControl.DisableInput();
            inputControl.StopAllMovement();
        }

        dialogueList = new List<dialogueString>(textToPrint); 
        currentDialogueIndex = 0;
        optionSelected = false;
        nextLinePressed = false;

        DisableButtons();

        StartCoroutine(PrintDialogue());
    }

    private void DisableButtons()
    {
        choicesParent?.SetActive(false);
        option1Button.interactable = false;
        option2Button.interactable = false;
        option1Button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        option2Button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
    }

    private IEnumerator PrintDialogue()
    {
        try
        {
            while (currentDialogueIndex < dialogueList.Count)
            {
                dialogueString line = dialogueList[currentDialogueIndex];
                line.startDialogueEvent?.Invoke();

                if (line.isQuestion)
                {
                    yield return StartCoroutine(TypeText(line.text));
                    choicesParent?.SetActive(true);

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
                    choicesParent?.SetActive(false);
                    yield return StartCoroutine(TypeText(line.text));
                }

                if (line.isEnd)
                    {
                        DialogueStop();
                        yield break;
                    }

                line.endDialogueEvent?.Invoke();
                optionSelected = false;
                currentDialogueIndex++;
            }
        }
        finally
        {
            DialogueStop();
        }
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
            if (nextLinePressed)
            {
                dialogueText.text = fullText;
                nextLinePressed = false;
                break;
            }
            yield return new WaitForSeconds(typingSpeed);
        }

        if (!dialogueList[currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil(() => nextLinePressed);
            nextLinePressed = false;
        }

        if (dialogueList[currentDialogueIndex].isEnd)
            yield break;
    }

    public void DialogueStop()
    {
        StopAllCoroutines();

        PauseMenuToggle.instance.blockPause = false;
        PauseMenuToggle.instance.EnablePauseInput();

        dialogueText.text = "";
        dialogueParent.SetActive(false);
        choicesParent?.SetActive(false);
        playerInput.Dialogue.Disable();
        playerInput.PauseMenu.Enable();

        dialogueList = null;
        currentDialogueIndex = 0;
        optionSelected = false;
        nextLinePressed = false;
        IsDialogueActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (characterManager != null && characterManager.GetPlayerCharacter() != null)
        {
            CharacterInputController inputControl = characterManager.GetPlayerCharacter().GetComponent<CharacterInputController>();
            inputControl.EnableInput();
            characterManager.PlayerInput.CharacterSwitching.Enable();
        }

    }

}