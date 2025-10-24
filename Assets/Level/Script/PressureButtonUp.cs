using UnityEngine;

public class MonoBehaviourScript : MonoBehaviour
{
    public GameObject item;
    private Animator Animator;
    bool isUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator = item.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isUp)
        {
            Invoke("SetUp", 3);
            isUp = true;
        }
    }
    void SetUp()
    {
        Animator.SetTrigger("Up");
    }
}
