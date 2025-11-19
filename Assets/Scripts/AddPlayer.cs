using UnityEngine;

public class AddPlayer : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    public GameObject playerCompanion;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider collider)
    {
        if (characterManager != null && collider.gameObject!= playerCompanion)
        {
            characterManager.AddCompanion(playerCompanion);
            this.gameObject.SetActive(false);

        }
    }

}
