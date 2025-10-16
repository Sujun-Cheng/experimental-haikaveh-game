using UnityEngine;

public class TrapTriggerUp : MonoBehaviour
{
    private Animator TrapAnimator;
    void Start()
    {
        TrapAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("SetGateDown", 2);
        }
    }
    void SetGateDown()
    {
        TrapAnimator.SetTrigger("TrapUp");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

}
