using UnityEngine;

public class FellOffWorld : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MainCharacterController mc = collision.gameObject.GetComponent<MainCharacterController>();
            if (mc != null)
            {
                EventManager.TriggerEvent<DeathEvent, GameObject>(collision.gameObject);
            }
        }
    }
}
