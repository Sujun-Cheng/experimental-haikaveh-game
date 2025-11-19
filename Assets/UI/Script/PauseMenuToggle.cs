using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuToggle : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool isPaused = false;
    private PlayerInput playerInput;
    private CursorLockMode previousLockState = CursorLockMode.Locked;
    private bool previousCursorVisible = false;
    private void Awake()
    {
        playerInput = playerInput = new PlayerInput();
        playerInput.PauseMenu.TogglePause.performed += (ctx) =>
        {
            TogglePause();
        };
        playerInput.PauseMenu.TogglePause.canceled += (ctx) =>
        {
            Debug.Log("Pause menu key action canceled");
        };
        playerInput.Enable();
        playerInput.Dialogue.Enable();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("GetComponent() didn't find CanvasGroup");
        }
    }

 

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Show pause menu
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            // Freeze game
            Time.timeScale = 0f;

            // Unlock and show cursor
            previousLockState = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Hide pause menu
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;

            // Resume game
            Time.timeScale = 1f;

            // Lock and hide cursor again
            Cursor.lockState = previousLockState;
            Cursor.visible = previousCursorVisible;
        }
    }
}