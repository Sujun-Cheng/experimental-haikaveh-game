using UnityEngine;

public class ReachDestinationObjective : Objective
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Collider Destination;
    [SerializeField] private bool reachedDestination;
    private void Awake()
    {
        Destination = GetComponent<Collider>();
    }
    void Start()
    {
        
        Destination.isTrigger = true;
        reachedDestination = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainCharacterController mc = other.gameObject.GetComponent<MainCharacterController>();
            
            if (mc != null && !mc.IsAIControlled())
            {
                reachedDestination = true;
                FinishObjective();
            }
        }
        
    }

    // Update is called once per frame
    public override bool CheckCompleteCondition()
    {
        return reachedDestination;
    }
}
