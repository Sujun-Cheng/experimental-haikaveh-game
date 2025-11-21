using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Collectable Settings")]
    public bool autoCollectOnTrigger = true;
    public string playerTag = "Player";

    [Header("Visual Feedback")]
    public bool rotateItem = true;
    public float rotationSpeed = 50f;
    public bool bobUpDown = true;
    public float bobSpeed = 2f;
    public float bobHeight = 0.3f;

    [Header("Collection Effects")]
    public GameObject collectEffect; // Particle effect on collection
    public AudioClip collectSound;

    private Vector3 startPosition;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Rotate the item
        if (rotateItem)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // Bob up and down
        if (bobUpDown)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check for both "Player" tag and "Pickup" tag on collectable
        if (autoCollectOnTrigger && other.CompareTag(playerTag) && !isCollected)
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected) return;

        isCollected = true;

        // Notify ObjectiveManager
        if (InteractObjective.Instance != null)
        {
            InteractObjective.Instance.CollectItem();
            Debug.Log($"Collected {gameObject.name}!");
        }

        // Play effects
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Destroy the item
        Destroy(gameObject);
    }
}