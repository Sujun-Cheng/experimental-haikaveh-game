using UnityEngine;

public class GeneralTrigger : MonoBehaviour
{
    private Animator TrapAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TrapAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrapAnimator.SetTrigger("Up");
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        TrapAnimator.SetTrigger("TrapDown");
    //    }
    //}
 

}
