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
        if (collision.impulse.magnitude > 0.25f)
        {
            //we'll just use the first contact point for simplicity
            EventManager.TriggerEvent<BoxCollisionEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);

        }
    }

}
