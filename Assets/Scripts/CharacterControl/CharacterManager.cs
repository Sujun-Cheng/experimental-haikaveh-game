using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;


public class CharacterManager : MonoBehaviour {


    public MainCharacterController[] ControllableCharacters = {}; 

    public CinemachineCamera thirdPersonCamera;


    protected int nextCharacterIndex = 0;

    protected void disableAllCharacters() {

        foreach (MainCharacterController c in ControllableCharacters)
        {
            c.switchToAIControlledIdle();
        }
            
    }

    protected void setCharacter(int charIndex) {

        disableAllCharacters();

        if (charIndex < 0)
            charIndex = 0;

        if (charIndex >= ControllableCharacters.Length)
            charIndex = ControllableCharacters.Length - 1;

        ControllableCharacters[charIndex].switchToPlayerControlled();

        thirdPersonCamera.Follow = ControllableCharacters[charIndex].transform;
        thirdPersonCamera.LookAt = ControllableCharacters[charIndex].transform;

        Debug.Log("Character " + ControllableCharacters[charIndex].Name + " was selected.");
   
    }

    protected void incrementCharacterIndex() {

        ++nextCharacterIndex;

        if (nextCharacterIndex < 0 || nextCharacterIndex >= ControllableCharacters.Length)
            nextCharacterIndex = 0;
    }

    protected void toggleCharacter() {

        setCharacter(nextCharacterIndex);

        incrementCharacterIndex();
    }

	
	void Start () {
		
        if (thirdPersonCamera == null)
            Debug.LogError("camera must be set");

        setCharacter(nextCharacterIndex);
        incrementCharacterIndex();

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Debug.Log("Character toggled");
            toggleCharacter();

        }
		
	}
}
