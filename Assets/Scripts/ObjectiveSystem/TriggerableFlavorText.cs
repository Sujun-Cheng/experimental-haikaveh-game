using System;
using UnityEngine;

[Serializable]
public class ObjectiveTextLine
{
    public string text;
    public Color color;
}
public class TriggerableFlavorText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public ObjectiveTextLine[] text;
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
                EventManager.TriggerEvent<ObjectiveDialogueEvent, ObjectiveTextLine[]>(text);
                //TODO: start audio here
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MainCharacterController mc = other.gameObject.GetComponent<MainCharacterController>();

        if (mc != null && !mc.IsAIControlled())
        {
            //TODO: render text here
            this.enabled = false;
            //TODO: start audio here
        }
    }


}
