using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static AINavSteeringController;

public class AINavV2 : MonoBehaviour
{
    public Transform player;
    public float turnSensitivity;
    public float allowableMaxTolerance;
    public float agentAngleNormalizationFactor = 10f;
    private NavMeshAgent navMeshAgent;
    private Vector3 velocity;
    private RootMotionControlScript rootMotionScript;
    private CharacterInputController characterInputController;
    private Animator anim;

    // Reflection fields for accessing private members
    private FieldInfo inputForwardField;
    private FieldInfo inputTurnField;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected const float VectorLengthThreshold = 0.001f;
    protected Vector3 origPositionBeforeRootMotionApplied;
    protected Quaternion origRotationBeforeRootMotionApplied;

    public float CalculatedInputTurn
    {
        get;
        private set;
    }

    public float CalculatedInputForward
    {
        get;
        private set;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
        rootMotionScript = GetComponent<RootMotionControlScript>();
        characterInputController = GetComponent<CharacterInputController>();
        //rootMotionScript.enabled = false;
        //characterInputController.enabled = false;
        anim = GetComponent<Animator>();
        SetupReflection();
        //navMeshAgent.SetDestination(player.transform.position);
        //navMeshAgent.updatePosition = false;
        //navMeshAgent.updateRotation = false;
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
           
                NavMeshHit hit;
                //if (NavMesh.Raycast(navMeshAgent.transform.position, player.position, out hit, NavMesh.AllAreas))
                //{
                //    navMeshAgent.SetDestination(hit.position);
                //}
                //else
                //{
                navMeshAgent.SetDestination(player.position);
                //}
                if (navMeshAgent.isOnNavMesh)
                {
                    Debug.Log("Agent is on mesh ");
                }
                else
                {
                    Debug.Log("Agent is no longer on mesh");
                }

                SynchronizeAnimatorAndAgent();
            

        }
        //float currAnimValueTurn = 0f;
        //float currAnimValueForward = 0f;

        //float newInputForward;
        //float newInputTurn;




        //currAnimValueForward = anim.GetFloat("vely");

        //currAnimValueTurn = anim.GetFloat("velx");

        ////print("currAnimValueForward: " + currAnimValueForward);

        //ControlledUpdate(currAnimValueTurn, currAnimValueForward, out newInputTurn, out newInputForward);
        //print($"newInputTurn: {newInputTurn}, newInputForward: {newInputForward}");
        //CalculatedInputTurn = newInputTurn;
        //CalculatedInputForward = newInputForward;
        //anim.SetFloat("vely", newInputForward);
        //anim.SetFloat("velx", newInputTurn);
    }

    private void SynchronizeAnimatorAndAgent()
    {
        velocity = navMeshAgent.velocity;
        if (rootMotionScript == null) return;

        try
        {
            float currAnimValueTurn = 0f;
            float currAnimValueForward = 0f;

            float newInputForward;
            float newInputTurn;




            currAnimValueForward = anim.GetFloat("vely");

            currAnimValueTurn = anim.GetFloat("velx");

            //print("currAnimValueForward: " + currAnimValueForward);

            //ControlledUpdate(currAnimValueTurn, currAnimValueForward, out newInputTurn, out newInputForward);
            //print($"newInputTurn: {newInputTurn}, newInputForward: {newInputForward}");

            // Directly set the cached input values in RootMotionControlScript
            if (inputForwardField != null)
            {
                velocity.y = 0;

                float agentDesiredSpeed = navMeshAgent.desiredVelocity.magnitude;
                float agentNormForward = agentDesiredSpeed / navMeshAgent.speed;
                Debug.Log($"applying forward velocity of {Mathf.Clamp(agentNormForward, 0f, 1f)} to pursue player: {player}");
                inputForwardField.SetValue(rootMotionScript, Mathf.Clamp(agentNormForward, 0f, 1f));
                //Debug.Log($"applying forward velocity of {Mathf.Clamp(newInputForward, 0f, 1f)} to pursue player: {player}");
                //inputForwardField.SetValue(rootMotionScript, Mathf.Clamp(newInputForward, 0f, 1f));
            }
            if (inputTurnField != null)
            {
                Vector3 candidateAgentDesiredHeading = player.position - this.transform.position;
                Vector3 agentDesiredHeading = candidateAgentDesiredHeading.normalized;
                Vector2 desiredHeading2D = new Vector2(agentDesiredHeading.x, agentDesiredHeading.z);


                Vector3 currHeading = this.transform.forward;
                Vector2 currHeading2D = new Vector2(currHeading.x, currHeading.z);
                float angle = SignedAngle(currHeading2D, desiredHeading2D);
                float angleToTarget = (angle / Time.deltaTime) / (navMeshAgent.angularSpeed * agentAngleNormalizationFactor);
                float turnInput = Mathf.Clamp(angleToTarget / 45, -1f, 1f);
                Debug.Log($"applying turning velocity of {turnInput} to pursue player: {player}");
                inputTurnField.SetValue(rootMotionScript, turnInput);
                //Debug.Log($"applying turning velocity of {newInputTurn} to pursue player: {player}");
                //inputTurnField.SetValue(rootMotionScript, newInputTurn);
            }




        }
        catch (System.Exception e)
        {
            Debug.LogError($"AIMovement: Error setting AI input: {e.Message}");
        }

    }

    protected static float SignedAngle(Vector2 a, Vector2 b)
    {
        //Find relative angle we need to correct to
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(a.y, a.x) - Mathf.Atan2(b.y, b.x));

        angle %= 360f;
        angle = (angle + 360f) % 360f;
        if (angle > 180f)
            angle -= 360f;

        return angle;


        //Another method of getting relative angle
        //		float angle =  Vector2.Angle (desiredHeading, currHeading);
        //
        //
        //		//determine sign based on CW or CCW
        //		if (currHeading.y * desiredHeading.x < currHeading.x * desiredHeading.y)
        //			angle = -angle;
        //
    }

     public void ControlledUpdate(float currAnimValueTurn, float currAnimValueForward, out float animValueTurn, out float animValueForward  )
	{
        Vector3 targetPos;
        navMeshAgent.SetDestination(player.transform.position);
        targetPos = player.transform.position;
          
        Vector2 pos2d = new Vector2(this.transform.position.x, this.transform.position.z);
        Vector2 targ2d = new Vector2(targetPos.x, targetPos.z);
        float agentDesiredSpeed = navMeshAgent.desiredVelocity.magnitude;

        print($"target position: {targetPos}, desired speed: {agentDesiredSpeed}");

		//normalize to [0, MAX_SPEED] so we can create a mecanim input value
		//I don't think the agent ever goes backward
		float agentNormForward = agentDesiredSpeed / navMeshAgent.speed;

		//just in case agent going faster than the speed limit
		agentNormForward = Mathf.Clamp(agentNormForward, 0f, 1f);

        Vector3 candidateAgentDesiredHeading = transform.forward;

        print($"navMeshAgent.desiredVelocity {navMeshAgent.desiredVelocity}, agentNormForward: {agentNormForward}");

        candidateAgentDesiredHeading = navMeshAgent.desiredVelocity;

        if (candidateAgentDesiredHeading.magnitude < VectorLengthThreshold)
        {
            //the candidate is very short, maybe from some obstacle avoidance or something
            //try another

            #if UNITY_EDITOR
                print($"(path plan) bad vector 1 {candidateAgentDesiredHeading}");
            #endif

            candidateAgentDesiredHeading = navMeshAgent.steeringTarget - navMeshAgent.nextPosition;

            if (candidateAgentDesiredHeading.magnitude < VectorLengthThreshold)
            {
                //another too short

                #if UNITY_EDITOR
                    print($"(path plan) bad vector 2 {candidateAgentDesiredHeading}");
                #endif

                //use something reasonable, can safely assume it's a unit vector w/o noisy direction
                candidateAgentDesiredHeading = transform.forward;
            }                  
        }
            
		//Start working towards a normalized turn value for mecanim 

        Vector3 agentDesiredHeading = candidateAgentDesiredHeading.normalized; 
		Vector2 desiredHeading2D = new Vector2 (agentDesiredHeading.x, agentDesiredHeading.z);


		Vector3 currHeading = this.transform.forward;
		Vector2 currHeading2D = new Vector2 (currHeading.x, currHeading.z);


		float angle = SignedAngle (currHeading2D, desiredHeading2D); //correction angle needed to match agent


        //if NPC is capab

        float normFac = agentAngleNormalizationFactor;
		float agentNormAngle = (angle/Time.deltaTime) / (navMeshAgent.angularSpeed * normFac);

		//clamp to range [-1, 1]
		agentNormAngle = Mathf.Clamp (agentNormAngle, -1f, 1f);

		float agentNormAngleSign = Mathf.Sign (agentNormAngle); //pay no attention to this imaginary workaround
		//use of power allows lerp param to be non-linear and curve to bow up or down depending on power 
		agentNormAngle =  Mathf.Pow (Mathf.Abs(agentNormAngle), 1);
		agentNormAngle *= agentNormAngleSign;

        





       
		//Here, we optionally apply input filtering/smoothing similar to what Unity's InputManager is capable of

		//prime with good values in case we don't run input smoothing
		float mecanimInputTurn = agentNormAngle;
		float mecanimInputForward = agentNormForward;

		//if (inputDoSmoothing) {


  //          float dt = Time.deltaTime;

  //          switch(inputSmoothingType) {
                          
  //              case SmoothingType.simpleSmoothing:

  //                  mecanimInputForward = Mathf.Lerp(currAnimValueForward, agentNormForward, dt*inputTurnFilterWeight);
  //                  mecanimInputTurn = Mathf.Lerp(currAnimValueTurn, agentNormAngle, dt*inputForwardFilterWeight);

  //                  break;

  //              case SmoothingType.dampedSmoothing:

  //                  mecanimInputForward = Mathf.SmoothDamp(currAnimValueForward, agentNormForward,ref inputVelForward, inputForwardDampedSmoothingTime, Mathf.Infinity, dt);
  //                  mecanimInputTurn = Mathf.SmoothDampAngle(currAnimValueTurn, agentNormAngle, ref inputVelTurn, inputTurnDampedSmoothingTime, Mathf.Infinity, dt);

  //                  break;

  //              case SmoothingType.physicsModelSmoothing:

		//	        // physics based model for input changes

  //                  float deltaSquared = dt * dt;

		//	        //our accel from current to new value is faster if the values are further away
		//	        //technique sometimes called poor man's Kalman Filter
		//	        //conceptually, treats values that are slightly different than current as noise that can largely be ignored
  //                  float filteredAccelForward = filterAccel(currAnimValueForward, agentNormForward, 1f, inputFilterPowerForward, inputMaxAccelForward);

		//	        //now we use filtering based on physics to move from curr input value toward target. probably won't get there immediately due
		//	        //to physics contraints. But will eventually after a few updates()
  //                  mecanimInputForward = filter(currAnimValueForward, agentNormForward, dt, deltaSquared, 
  //                      filteredAccelForward, ref inputVelForward);

		//	        //same filtering approach as above for forward Direction now applied to turning
  //                  float filteredAccelTurn = filterAccel(currAnimValueTurn, agentNormAngle, 2f, inputFilterPowerTurn, inputMaxAccelTurn);

  //                  mecanimInputTurn = filter(currAnimValueTurn, agentNormAngle, dt, deltaSquared, 
  //                      filteredAccelTurn, ref inputVelTurn);

  //                  break;
  //          }

		//}
	
		//allow override of top speed in case we want to slow down the action
		float mecanimSpeedForward = Mathf.Min (1, mecanimInputForward);



        //output values for input to character controller

        if (!navMeshAgent.isOnOffMeshLink)
        {
            animValueTurn = mecanimInputTurn;
            animValueForward = mecanimSpeedForward;


//            //try to avoid hitting edge of navmesh because more likely to get stuck
//            Vector3 candidatePos = transform.position + Time.deltaTime * mecanimSpeedForward * mecanimMaxSpeed * transform.forward;
//
//            NavMeshHit hit;
//            if (NavMesh.Raycast(transform.position, candidatePos, out hit, NavMesh.AllAreas))
//            {
//                animValueForward = 0f;
//                if (animValueTurn >= 0)
//                    animValueTurn = 1f;
//                else
//                    animValueTurn = -1f;
//            }
//            else
//            {
//                animValueForward = mecanimSpeedForward;
//            }
        }
        else
        {
            animValueTurn = currAnimValueTurn;

            animValueForward = currAnimValueForward;
        }
   

        origPositionBeforeRootMotionApplied = this.transform.position;
        origRotationBeforeRootMotionApplied = this.transform.rotation;

		////////////////////////////////////
		//Some debugging output to inspector follows
		////////////////////////////////////

        #if UNITY_EDITOR
		//if (debugOutput) {

		//	//draw to scene view in editor
  //          Debug.DrawRay (this.transform.position, agentDesiredHeading.normalized * 5f, Color.green);
		//	Debug.DrawRay (this.transform.position, currHeading * 5f, Color.blue);


		//	Vector2 target;

		//	if(waypointPositions.Length > 0)
		//		target = new Vector2(waypointPositions[currWayPoint].x, waypointPositions[currWayPoint].z);
		//	else
		//		target = new Vector2(this.transform.position.x, this.transform.position.z);

		//	float targDist = Vector2.Distance (new Vector2(this.transform.position.x, this.transform.position.z), 
		//											target);


		//	float distToSteeringTarget = Vector2.Distance (new Vector2(this.transform.position.x, this.transform.position.z), 
		//		new Vector2(agent.steeringTarget.x, agent.steeringTarget.y));

		//	//Debug output to Inspector
		//	debugDistToTarget = targDist;
  //          debugDistToSteeringTarget = distToSteeringTarget;
		//	debugForward = transform.forward;
		//	debugNormSpeed = agentNormForward;
		//	debugDesiredHeadingCorrection = angle;
		//	debugNormAngle = mecanimInputTurn;	
  //          debugAgentPathPending = agent.pathPending;
  //          debugAgentHasPath = agent.hasPath; 
  //          debugMecanimInputTurn = mecanimInputTurn;
  //          debugMecanimInputForward = mecanimSpeedForward;

		//}
        #endif

	}
    //void OnAnimatorMove()
    //{


    //        //Vector3 origPosition = this.transform.position;


    //        //regular mecanim root motion behavior BEGINS
    //        this.transform.position = anim.rootPosition;
    //        this.transform.rotation = anim.rootRotation;
    //    //regular mecanim root motion behavior ENDS
    //        navMeshAgent.nextPosition = this.transform.position; //pull agent back if she went too far


    //}

    public void OnEnable()
    {
        navMeshAgent.enabled = true;
    }

    public void OnDisable()
    {
        navMeshAgent.enabled = false;
    }

}
