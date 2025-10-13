using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Animator animator;
    public Transform handHold; // Assign WeaponHoldSpot
    public GameObject weaponPrefab;
    public GameObject slashVFXPrefab; // Assign your slash effect here
    private GameObject currWeapon;

    void Awake()
    {
        if (!handHold)
            handHold = transform.Find("RightHand/WeaponHoldSpot");
        if (!animator)
            animator = GetComponent<Animator>();
    }

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