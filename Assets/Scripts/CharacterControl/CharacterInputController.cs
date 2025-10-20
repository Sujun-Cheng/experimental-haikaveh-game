using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CharacterInputController : MonoBehaviour
{

    public string Name = "George P Burdell";
    public CinemachineCamera thirdPersonCamera;
    private PlayerInput playerInput;

    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public bool InputMapToCircular = true;

    public float forwardInputFilter = 20f;
    public float turnInputFilter = 20f;

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

    private bool disabledAllMoves = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
        print("input initialized");

        playerInput.CharacterControls.Movement.performed += (ctx) =>
        {
            print($"Player {Name}: applying movement of {ctx.ReadValueAsObject()}");
            currentMovement = ctx.ReadValue<Vector2>();
        };

        playerInput.CharacterControls.Dash.performed += (ctx) =>
        {
            print($"Player {Name}: applying dash of {ctx.ReadValueAsObject()}");
            DashPressed = ctx.ReadValueAsButton();
        };
        playerInput.CharacterControls.Dash.canceled += (ctx) =>
        {
            print($"Player {Name}: applying dash of {ctx.ReadValueAsObject()}");
            DashPressed = ctx.ReadValueAsButton();
        };

        playerInput.CharacterControls.Jump.performed += (ctx) =>
        {
            print($"Player {Name}: applying jump of {ctx.ReadValueAsObject()}");
            Jump = ctx.ReadValueAsButton();
        };
        playerInput.CharacterControls.Jump.canceled += (ctx) =>
        {
            print($"Player {Name}: applying jump of {ctx.ReadValueAsObject()}");
            Jump = ctx.ReadValueAsButton();
        };
        playerInput.CharacterControls.Attack.performed += (ctx) =>
        {
            print($"Player {Name}: applying atk of {ctx.ReadValueAsObject()}");
            Debug.Log($"🖱️ [{Time.frameCount}] ATTACK SET TO TRUE in CharacterInputController!");
            Attack = ctx.ReadValueAsButton();
        };
        playerInput.CharacterControls.Attack.canceled += (ctx) =>
        {
            print($"Player {Name}: applying atk of {ctx.ReadValueAsObject()}");
            Attack = ctx.ReadValueAsButton();
        };
    }

    void Update()
    {
        float h = currentMovement.x;
        float v = currentMovement.y;
        if ( thirdPersonCamera != null)
        {
            Vector3 cameraReferenceForward = thirdPersonCamera.transform.forward;
            Vector3 cameraReferenceRight = thirdPersonCamera.transform.right;
            cameraReferenceForward.y = 0;
            cameraReferenceRight.y = 0;
            cameraReferenceForward.Normalize();
            cameraReferenceRight.Normalize();
            Vector3 relativeForwardMovement = cameraReferenceForward * v;
            Vector3 relativeRightMovement = cameraReferenceRight * h;
            Vector3 newMovementVector = relativeForwardMovement + relativeRightMovement;
            h = newMovementVector.x;
            v = newMovementVector.z;
        }
        //get forward vector between player and camera
        print($"h, v: {h}, {v}");
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

        //filteredTurnInput = Mathf.Lerp(filteredTurnInput, h,
        //    Time.deltaTime * turnInputFilter);


        filteredTurnInput = Mathf.Clamp(Mathf.Lerp(filteredTurnInput, h,
            Time.deltaTime * turnInputFilter), -forwardSpeedLimit, forwardSpeedLimit);
        Forward = filteredForwardInput;
        Turn = filteredTurnInput;

        // Capture button inputs - these will be checked by other scripts
        Action = Input.GetButtonDown("Fire1");
        Jump = Input.GetButtonDown("Jump");

        // Attack input - check multiple sources
        //Attack = Input.GetButtonDown("Fire1") || Input.GetMouseButtonDown(0);

        //// Debug when attack is pressed
        //if (Attack)
        //{
        //    Debug.Log($"🖱️ [{Time.frameCount}] ATTACK SET TO TRUE in CharacterInputController!");
        //}
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void DisableInput()
    {
        disabledAllMoves = true;
        playerInput.CharacterControls.Disable();
    }

    public void EnableInput()
    {
        disabledAllMoves = false;
        playerInput.CharacterControls.Enable();
    }

    public void StopAllMovement()
    {
        currentMovement = Vector2.zero;
        Forward = 0f;
        Turn = 0f;
        DashPressed = false;
        Jump = false;
        Action = false;
        Attack = false;
    }
}