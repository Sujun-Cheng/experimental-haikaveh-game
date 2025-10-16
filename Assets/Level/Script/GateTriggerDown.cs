using UnityEngine;

public class GateTriggerDown : MonoBehaviour
{
    private Animator GateAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GateAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("SetGateDown", 3);
        }
    }
    void SetGateDown()
    {
        GateAnimator.SetTrigger("GateDown");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
}
