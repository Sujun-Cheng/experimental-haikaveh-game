using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(20f, 0f, 20f);
    [SerializeField] private float spawnHeightOffset = 0.5f;

    [Header("Spacing")]
    [SerializeField] private float minDistanceBetweenEnemies = 3f;
    [SerializeField] private float minDistanceFromPlayer = 8f;

    [Header("Terrain")]
    [SerializeField] private LayerMask terrainLayer;

    private Transform playerTransform;

    void Start()
    {
        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        List<Vector3> spawnPositions = GenerateSpawnPositions(enemyCount);

        foreach (Vector3 position in spawnPositions)
        {
            Instantiate(enemyPrefab, position, Quaternion.identity);
        }
    }

    private List<Vector3> GenerateSpawnPositions(int count)
    {
        List<Vector3> positions = new List<Vector3>();
        int maxAttempts = count * 10;
        int attempts = 0;

        while (positions.Count < count && attempts < maxAttempts)
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
            if (IsValidPosition(worldPos, positions))
            {
                positions.Add(worldPos);
            }
        }

        return positions;
    }

    private bool IsValidPosition(Vector3 position, List<Vector3> existingPositions)
    {
        // Check distance from player
        if (playerTransform != null)
        {
            if (Vector3.Distance(position, playerTransform.position) < minDistanceFromPlayer)
            {
                return false;
            }
        }

        // Check distance from other enemies
        foreach (Vector3 existingPos in existingPositions)
        {
            if (Vector3.Distance(position, existingPos) < minDistanceBetweenEnemies)
            {
                return false;
            }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        // Draw spawn area
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}