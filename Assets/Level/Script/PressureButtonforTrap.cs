using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject trap;
    private Animator TrapAnimator;
    bool isUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TrapAnimator = trap.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUp)
        {
            Invoke("SetTrapUp", 3);
            isUp = true;
        }
    }
    void SetTrapUp()
    {
        TrapAnimator.SetTrigger("TrapUp");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

}
