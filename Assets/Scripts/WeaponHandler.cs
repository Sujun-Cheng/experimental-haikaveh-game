using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Animator animator;
    public Transform handHold; // Assign WeaponHoldSpot
    public GameObject weaponPrefab;
    public GameObject slashVFXPrefab; // Assign your slash effect here
    public Transform vfxSpawnPoint; //optional spawn point for VFX

    public GameObject explosionVFXPrefab; // add another effect after slash
    public Transform explosionSpawnPoint;

    private GameObject currWeapon;

    void Start()
    {
        // Create the weapon when the game starts
        EquipWeapon();
    }

    // Call this to equip the weapon (can do on Start or pickup)
    public void EquipWeapon()
    {
        if (currWeapon != null) return;
        currWeapon = Instantiate(weaponPrefab, handHold);
        currWeapon.transform.localPosition = Vector3.zero;
        currWeapon.transform.localRotation = Quaternion.identity;
        currWeapon.SetActive(false); // start hidden until attack
    }

    // Called by animation event during attack
    public void ShowWeapon()
    {
        if (currWeapon != null)
            currWeapon.SetActive(true);
    }

    public void HideWeapon()
    {
        if (currWeapon != null)
            currWeapon.SetActive(false);
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
    public void PlayExplosionVFX()
    {
        if (explosionVFXPrefab != null)
        {
            Transform spawnPos = explosionSpawnPoint != null ? explosionSpawnPoint : handHold;
            GameObject vfx = Instantiate(explosionVFXPrefab, spawnPos.position, spawnPos.rotation);
            Destroy(vfx, 3f); // Auto-destroy after 3 seconds (adjust as needed)
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }
}