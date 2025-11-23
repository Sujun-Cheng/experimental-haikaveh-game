using System.Collections;
using UnityEngine;

public class PressureButtonDisappear : MonoBehaviour
{
    public GameObject[] items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool isDown = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isDown)
        {
            isDown = true;
            StartCoroutine(FallAway());
           
            
            
        }
    }
    private IEnumerator FallAway()
    {
        foreach (var item in items)
        {
            item.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
    }

}
