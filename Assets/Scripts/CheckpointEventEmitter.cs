using UnityEngine;

public class CheckpointEventEmitter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MainCharacterController mc = collision.gameObject.GetComponent<MainCharacterController>();
            if (mc != null && !mc.IsAIControlled())
            {
                EventManager.TriggerEvent<CheckPassedEvent, GameObject>(this.gameObject);
            }
        }
    }
}
