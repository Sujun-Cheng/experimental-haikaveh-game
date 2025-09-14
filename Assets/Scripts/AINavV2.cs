using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class AINavV2 : MonoBehaviour
{
    public Transform player;
    public float turnSensitivity;
    public float allowableMaxTolerance;
    private NavMeshAgent navMeshAgent;
    private Vector3 velocity;
    private RootMotionControlScript rootMotionScript;
    private CharacterInputController characterInputController;

    // Reflection fields for accessing private members
    private FieldInfo inputForwardField;
    private FieldInfo inputTurnField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
        rootMotionScript = GetComponent<RootMotionControlScript>();
        characterInputController = GetComponent<CharacterInputController>();
        SetupReflection();
    }

    void SetupReflection()
    {
        System.Type rootMotionType = typeof(RootMotionControlScript);

        // Get the private cached input fields from RootMotionControlScript
        inputForwardField = rootMotionType.GetField("_inputForward",
            BindingFlags.NonPublic | BindingFlags.Instance);
        inputTurnField = rootMotionType.GetField("_inputTurn",
            BindingFlags.NonPublic | BindingFlags.Instance);

        if (inputForwardField == null || inputTurnField == null)
        {
            Debug.LogError("AIMovement: Could not find required private fields in RootMotionControlScript.");
            Debug.LogError("Make sure the field names '_inputForward' and '_inputTurn' exist in RootMotionControlScript.");
        }
        else
        {
            Debug.Log("AIMovement: Reflection setup successful");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && characterInputController != null)
        {
            if (!characterInputController.enabled)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(player.position);
                SynchronizeAnimatorAndAgent();
            } else
            {
                navMeshAgent.enabled = false;
            }
           
        }
        

    }

    private void SynchronizeAnimatorAndAgent()
    {
        velocity = navMeshAgent.velocity;
        if (rootMotionScript == null) return;

        try
        {
            // Directly set the cached input values in RootMotionControlScript
            if (inputForwardField != null)
            {
                velocity.y = 0;
                Debug.Log($"applying forward velocity of {velocity.magnitude} to pursue player: {player}" );
                inputForwardField.SetValue(rootMotionScript, velocity.magnitude);
            }
            if (inputTurnField != null)
            {
                float angleToTarget = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
                float turnInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f) * 1.5f;
                Debug.Log($"applying turning velocity of {turnInput} to pursue player: {player}");
                inputTurnField.SetValue(rootMotionScript, turnInput);
            }

          
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AIMovement: Error setting AI input: {e.Message}");
        }

    }


}
