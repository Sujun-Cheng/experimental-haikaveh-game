using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Spawn Settings")]
    [SerializeField] private GameObject npcPrefab;
    [SerializeField] private int initialNPCCount = 20; // Number of NPCs to spawn at start
    [SerializeField] private float npcSpeed = 1f;
    [SerializeField] private float spawnInterval = 2f; // Spawn every X seconds

    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(80f, 0f, 30f);
    [SerializeField] private float spawnHeightOffset = 0.5f;

    [Header("Spacing")]
    [SerializeField] private float minDistanceFromPlayer = 5f;

    [Header("Terrain")]
    [SerializeField] private LayerMask terrainLayer;

    private Transform playerTransform;
    private Vector3 mapCenter;
    private Vector3 mapSize;
    private float spawnTimer = 0f;

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        mapCenter = transform.position;
        mapSize = spawnAreaSize;

        // Spawn initial NPCs
        for (int i = 0; i < initialNPCCount; i++)
        {
            SpawnSingleNPC();
        }
    }

    void Update()
    {
        // Increment timer
        spawnTimer += Time.deltaTime;

        // Spawn new NPC every interval
        if (spawnTimer >= spawnInterval)
        {
            SpawnSingleNPC();
            spawnTimer = 0f;
        }
    }

    private void SpawnSingleNPC()
    {
        Vector3 spawnPos = GenerateSpawnPosition();

        GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);

        // Add NPC walker component
        NPCWalker walker = npc.GetComponent<NPCWalker>();
        if (walker == null)
        {
            walker = npc.AddComponent<NPCWalker>();
        }

        // Set random direction
        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        ).normalized;

        walker.Initialize(randomDir, npcSpeed, mapCenter, mapSize);
    }

    private Vector3 GenerateSpawnPosition()
    {
        int maxAttempts = 30;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            attempts++;

            // Random position in spawn area
            Vector3 randomPos = new Vector3(
                Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
                0f,
                Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
            );

            Vector3 worldPos = transform.position + randomPos;

            // Raycast to find ground height
            RaycastHit hit;
            if (Physics.Raycast(worldPos + Vector3.up * 100f, Vector3.down, out hit, 200f, terrainLayer))
            {
                worldPos = hit.point + Vector3.up * spawnHeightOffset;
            }
            else
            {
                worldPos.y = transform.position.y + spawnHeightOffset;
            }

            // Check if valid position
            if (IsValidPosition(worldPos))
            {
                return worldPos;
            }
        }

        // If no valid position found, return center as fallback
        return transform.position + Vector3.up * spawnHeightOffset;
    }

    private bool IsValidPosition(Vector3 position)
    {
        // Check distance from player
        if (playerTransform != null)
        {
            if (Vector3.Distance(position, playerTransform.position) < minDistanceFromPlayer)
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}