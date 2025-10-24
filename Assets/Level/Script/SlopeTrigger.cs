using UnityEngine;

public class SlopeTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator SlopeAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SlopeAnimator = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SlopeAnimator.SetTrigger("SlopeUp");
        }
    }
}
