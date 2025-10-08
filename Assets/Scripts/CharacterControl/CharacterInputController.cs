using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputController : MonoBehaviour
{

    public string Name = "George P Burdell";
    private PlayerInput playerInput;

    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public bool InputMapToCircular = true;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;

    private float forwardSpeedLimit = 1f;

    private Vector2 currentMovement;

    // Input properties - using simple get/set like the original
    public bool DashPressed { get; private set; }
    public float Forward { get; private set; }
    public float Turn { get; private set; }
    public bool Action { get; private set; }
    public bool Jump { get; private set; }

    // Attack uses the same pattern as Action - just stores the button state
    public bool Attack { get; private set; }


    private void Awake()
    {
        playerInput = new PlayerInput();
        print("input initialized");

        playerInput.CharacterControls.Movement.performed += (ctx) => {
            print($"Player {Name}: applying movement of {ctx.ReadValueAsObject()}");
            currentMovement = ctx.ReadValue<Vector2>();
        };

        playerInput.CharacterControls.Dash.performed += (ctx) => {
            print($"Player {Name}: applying dash of {ctx.ReadValueAsObject()}");
            DashPressed = ctx.ReadValueAsButton();
        };

        playerInput.CharacterControls.Jump.performed += (ctx) =>
        {
            print($"Player {Name}: applying jump of {ctx.ReadValueAsObject()}");
            Jump = ctx.ReadValueAsButton();
        };
    }

    void Update()
    {
        float h = currentMovement.x;
        float v = currentMovement.y;

        // BEGIN ANALOG ON KEYBOARD DEMO CODE
        if (Input.GetKey(KeyCode.Q))
            h = -0.5f;
        else if (Input.GetKey(KeyCode.E))
            h = 0.5f;

        if (Input.GetKeyUp(KeyCode.Alpha1))
            forwardSpeedLimit = 0.1f;
        else if (Input.GetKeyUp(KeyCode.Alpha2))
            forwardSpeedLimit = 0.2f;
        else if (Input.GetKeyUp(KeyCode.Alpha3))
            forwardSpeedLimit = 0.3f;
        else if (Input.GetKeyUp(KeyCode.Alpha4))
            forwardSpeedLimit = 0.4f;
        else if (Input.GetKeyUp(KeyCode.Alpha5))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha6))
            forwardSpeedLimit = 0.6f;
        else if (Input.GetKeyUp(KeyCode.Alpha7))
            forwardSpeedLimit = 0.7f;
        else if (Input.GetKeyUp(KeyCode.Alpha8))
            forwardSpeedLimit = 0.8f;
        else if (Input.GetKeyUp(KeyCode.Alpha9))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha0))
            forwardSpeedLimit = 1.0f;
        // END ANALOG ON KEYBOARD DEMO CODE  

        // Do filtering and clamping
        filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v,
            Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);

        filteredTurnInput = Mathf.Lerp(filteredTurnInput, h,
            Time.deltaTime * turnInputFilter);

        Forward = filteredForwardInput;
        Turn = filteredTurnInput;

        // Capture button inputs - these will be checked by other scripts
        Action = Input.GetButtonDown("Fire1");
        Jump = Input.GetButtonDown("Jump");

        // Attack input - check multiple sources
        Attack = Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0);

        // Debug when attack is pressed
        if (Attack)
        {
            Debug.Log($"🖱️ [{Time.frameCount}] ATTACK SET TO TRUE in CharacterInputController!");
        }
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}