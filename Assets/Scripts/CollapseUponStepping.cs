using UnityEngine;

public class CollapseUponStepping : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = false;
        rb.useGravity = true; 
    }

}
