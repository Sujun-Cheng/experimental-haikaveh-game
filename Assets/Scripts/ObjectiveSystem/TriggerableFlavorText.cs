using UnityEngine;

public class TriggerableFlavorText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string[] text;
    public AudioClip audioClip;
    private Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainCharacterController mc = other.gameObject.GetComponent<MainCharacterController>();

            if (mc != null && !mc.IsAIControlled())
            {
                //TODO: render text here
                //TODO: start audio here
            }
        }
    }

}
