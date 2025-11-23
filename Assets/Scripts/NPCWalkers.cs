using UnityEngine;

public class NPCWalker : MonoBehaviour
{
    private Vector3 walkDirection;
    private float moveSpeed;
    private Vector3 mapCenter;
    private Vector3 mapSize;
    private bool initialized = false;

    public void Initialize(Vector3 direction, float speed, Vector3 center, Vector3 size)
    {
        walkDirection = direction;
        moveSpeed = speed;
        mapCenter = center;
        mapSize = size;
        initialized = true;

        // Rotate NPC to face walking direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (!initialized) return;

        // Move NPC
        transform.position += walkDirection * moveSpeed * Time.deltaTime;

        // Check if NPC is outside map bounds
        if (IsOutsideMapBounds())
        {
            Destroy(gameObject);
        }
    }

    private bool IsOutsideMapBounds()
    {
        Vector3 localPos = transform.position - mapCenter;

        // Check X and Z boundaries with some buffer
        float bufferDistance = 2f;

        if (Mathf.Abs(localPos.x) > (mapSize.x / 2) + bufferDistance)
        {
            return true;
        }

        if (Mathf.Abs(localPos.z) > (mapSize.z / 2) + bufferDistance)
        {
            return true;
        }

        return false;
    }
}