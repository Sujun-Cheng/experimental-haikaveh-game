using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public GameObject gate;
    private Animator GateAnimator;
    bool isDown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GateAnimator = gate.GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDown)
        {
            Invoke("SetGateDown", 3);
            isDown = true;
        }
    }
    void SetGateDown()
    {
        GateAnimator.SetTrigger("GateDown");
    }
    // Update is called once per frame

}
