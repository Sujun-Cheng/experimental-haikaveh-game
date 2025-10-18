using UnityEngine;

public class GateTrigger : MonoBehaviour
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
            GateAnimator.SetTrigger("GateDown");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GateAnimator.SetTrigger("GateUp");
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
