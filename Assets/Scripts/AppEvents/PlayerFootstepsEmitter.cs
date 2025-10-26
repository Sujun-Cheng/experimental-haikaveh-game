using UnityEngine;

public class PlayerFootstepsEmitter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ExecuteFootstep()
    {

        EventManager.TriggerEvent<PlayerFootstepsEvent, Vector3>(transform.position);
    }
}
