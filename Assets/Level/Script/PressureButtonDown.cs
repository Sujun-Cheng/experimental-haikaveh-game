using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public GameObject item;
    private Animator Animator;
    bool isDown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator = item.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDown)
        {
            Invoke("SetDown", 3);
            isDown = true;
        }
    }
    void SetDown()
    {
        Animator.SetTrigger("Down");
    }
    // Update is called once per frame

}
