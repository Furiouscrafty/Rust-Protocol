using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Round Settings")]
    [SerializeField] private RoundSettings roundSettings;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 5f;

    [Header("Spawn Location")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool useRandomOffset = false;
    [SerializeField] private float randomOffsetRadius = 5f;

    private bool wasInRound = false;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Update()
    {
        if (roundSettings == null)
            return;

        bool inRoundNow = roundSettings.state == RoundSettings.RoundState.InRound;

        // Round just started
        if (inRoundNow && !wasInRound)
        {
            StartSpawning();
        }
        // Round just ended
        else if (!inRoundNow && wasInRound)
        {
            StopSpawning();
            DestroyAllSpawnedEnemies();
        }

        wasInRound = inRoundNow;
    }

    public void StartSpawning()
    {
        StopAllCoroutines();
        StartCoroutine(SpawnEnemies());
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }

    private void DestroyAllSpawnedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        int count = spawnedEnemies.Count;

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();

        Debug.Log($"Cleaned up {count} spawned enemies from the scene");
    }

    private IEnumerator SpawnEnemies()
    {
        while (roundSettings != null && roundSettings.state == RoundSettings.RoundState.InRound)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = GetSpawnPosition();
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        spawnedEnemies.Add(spawnedEnemy);
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 basePosition = spawnPoint != null ? spawnPoint.position : transform.position;

        if (useRandomOffset)
        {
            Vector2 randomCircle = Random.insideUnitCircle * randomOffsetRadius;
            basePosition += new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        return basePosition;
    }
}
