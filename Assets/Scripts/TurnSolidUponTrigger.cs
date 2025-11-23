using UnityEngine;

public class TurnSolidUponTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject[] gameObjects;
    private bool triggered;
    void Start()
    {
        triggered = false;
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(false);
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !triggered)
        {
            MainCharacterController mc = other.gameObject.GetComponent<MainCharacterController>();

            if (mc != null && !mc.IsAIControlled())
            {
                foreach (GameObject go in gameObjects)
                {
                    go.SetActive(true);
                }
                triggered = true;
            }
        }
    }
}
