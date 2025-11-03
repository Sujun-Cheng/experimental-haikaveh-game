using UnityEngine;

public class GuitarHandler : MonoBehaviour
{
    public Animator animator;
    public Transform handHold; 
    public GameObject GuitarPrefab;
    public GameObject slashVFXPrefab; 
    public Transform vfxSpawnPoint; 

    private GameObject currGuitar;

    void Start()
    {
        EquipGuitar();
    }

    public void EquipGuitar()
    {
        if (currGuitar != null) return;

        currGuitar = Instantiate(GuitarPrefab, handHold);
        //currGuitar.transform.localPosition = Vector3.zero;
        //currGuitar.transform.localRotation = Quaternion.identity;
        currGuitar.transform.localPosition = new Vector3(0.27276f, -1.3091f, 1.4247f);
        currGuitar.transform.localRotation = Quaternion.Euler(-47.131f, -111.762f, 142.959f);
        currGuitar.transform.localScale = new Vector3(11f, 12f, 9f);

        currGuitar.SetActive(true); 
    }

    public void ShowGuitar()
    {
        if (currGuitar != null)
            currGuitar.SetActive(true);
    }

    public void PlaySlashVFX()
    {
        if (slashVFXPrefab != null)
        {
            Transform spawnPos = vfxSpawnPoint != null ? vfxSpawnPoint : handHold;
            GameObject vfx = Instantiate(slashVFXPrefab, spawnPos.position, spawnPos.rotation);
            Destroy(vfx, 2f); // Auto-destroy after 2 seconds
        }
    }

}
