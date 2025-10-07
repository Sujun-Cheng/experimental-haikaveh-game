using JetBrains.Annotations;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public string Name = "CharacterNameHere";
    private CharacterInputController cinput;
    private RootMotionControlScript rootMotionControlScript;
    private AINavV2 aiNavController;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public enum ControllState
    {
        PlayerControlled,
        AIControlledFollowing,
        AIControlledIdle,
        AIControlledFighting
    };
    public ControllState currentControlState;


    private void Start()
    {
        cinput = GetComponent<CharacterInputController>();
        rootMotionControlScript = GetComponent<RootMotionControlScript>();
        aiNavController = GetComponent<AINavV2>();
    }

    private void Update()
    {
        switch (currentControlState)
        {
            case ControllState.PlayerControlled:
                break;
            case ControllState.AIControlledIdle:
                break;
            case ControllState.AIControlledFollowing:
                break;
            case ControllState.AIControlledFighting:
                break;
        }

    }

    public void switchToPlayerControlled()
    {
        cinput.enabled = true;
        aiNavController.enabled = false;
        currentControlState = ControllState.PlayerControlled;
    }
    public void switchToAIControlledIdle()
    {
        cinput.enabled = false;
        aiNavController.enabled = true; //TODO: change this
        currentControlState = ControllState.AIControlledIdle;
    }
}
