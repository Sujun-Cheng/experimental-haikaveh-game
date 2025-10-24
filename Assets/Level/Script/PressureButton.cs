using UnityEngine;

public class TirggerButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator PressureButtonAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PressureButtonAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PressureButtonAnimator.SetTrigger("Down");
        }
    }
    
}
