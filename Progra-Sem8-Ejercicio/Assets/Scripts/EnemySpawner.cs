using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameJolt.API;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    private int fib1 = 1, fib2 = 1;
    public float spawnInterval = 5f;
    private int enemiesPerWave = 0;       

    [Header("Spawn Settings")]
    public float spawnMargin = 5f;
    public float minSpawnDistanceToPlayer = 10f;
    public float minDistanceBetweenEnemies = 3f;
    public int maxSpawnAttempts = 10;

    private Camera mainCamera;
    private Transform playerTransform;

    private List<Vector3> lastSpawnPositions = new List<Vector3>();
    public int maxDebugSpawnPositions = 50;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("EnemySpawner: No se encontró la cámara principal.");
            enabled = false;
            return;
        }

        playerTransform = GameManager.Instance?.PlayerTransform;
        if (playerTransform == null)
        {
            Debug.LogError("EnemySpawner: No se encontró la referencia al jugador desde GameManager.");
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemiesRoutine());
    }      

    IEnumerator SpawnEnemiesRoutine()
    {
        enemiesPerWave = fib1;
        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return null;
        }

        int nextFib = fib1 + fib2;
        fib1 = fib2;
        fib2 = nextFib;

        if (enemiesPerWave == 5)
        {
            Trophies.Unlock(269858);
            Debug.Log("Estás en la oleada 5");
        }


        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            enemiesPerWave = fib1;

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return null;
            }

            nextFib = fib1 + fib2;
            fib1 = fib2;
            fib2 = nextFib;
        }        
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: No hay prefabs de enemigos asignados.");
            return;
        }

        GameObject enemyToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        TrySpawnObject(enemyToSpawn, minSpawnDistanceToPlayer, minDistanceBetweenEnemies);
    }      

    private void TrySpawnObject(GameObject prefab, float playerMinDist, float enemyMinDist)
    {
        bool foundValidPosition = false;
        Vector3 spawnPosition = Vector3.zero;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            spawnPosition = GetRandomSpawnPosition();

            if (lastSpawnPositions.Count >= maxDebugSpawnPositions)
            {
                lastSpawnPositions.RemoveAt(0);
            }
            lastSpawnPositions.Add(spawnPosition);

            if (Vector3.Distance(spawnPosition, playerTransform.position) < playerMinDist)
            {
                continue;
            }

            bool tooCloseToOthers = false;
            Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, enemyMinDist);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Player"))
                {
                    tooCloseToOthers = true;
                    break;
                }
            }

            if (!tooCloseToOthers)
            {
                foundValidPosition = true;
                break;
            }
        }

        if (foundValidPosition)
        {
            GameObject obj = ObjectPoolManager.Instance.GetPooledObject(prefab);
            if (obj != null)
            {
                obj.transform.position = spawnPosition;
                obj.transform.rotation = Quaternion.identity;
                obj.SetActive(true);
            }
            else
            {
                Debug.LogError($"EnemySpawner: No se pudo obtener el objeto del pool para el prefab {prefab.name}.");
            }
        }
        else
        {
            Debug.LogWarning($"EnemySpawner: No se pudo encontrar una posición válida para {prefab.name} después de {maxSpawnAttempts} intentos.");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        float distanceToPlayerPlane = Mathf.Abs(mainCamera.transform.position.y - playerTransform.position.y);

        Vector3 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToPlayerPlane));
        Vector3 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToPlayerPlane));

        float spawnX, spawnZ;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // Izquierda
                spawnX = minScreenBounds.x - spawnMargin;
                spawnZ = Random.Range(minScreenBounds.z - spawnMargin, maxScreenBounds.z + spawnMargin);
                break;
            case 1: // Derecha
                spawnX = maxScreenBounds.x + spawnMargin;
                spawnZ = Random.Range(minScreenBounds.z - spawnMargin, maxScreenBounds.z + spawnMargin);
                break;
            case 2: // Abajo
                spawnZ = minScreenBounds.z - spawnMargin;
                spawnX = Random.Range(minScreenBounds.x - spawnMargin, maxScreenBounds.x + spawnMargin);
                break;
            case 3: // Arriba
                spawnZ = maxScreenBounds.z + spawnMargin;
                spawnX = Random.Range(minScreenBounds.x - spawnMargin, maxScreenBounds.x + spawnMargin);
                break;
            default:
                spawnX = Random.Range(minScreenBounds.x - spawnMargin, maxScreenBounds.x + spawnMargin);
                spawnZ = Random.Range(minScreenBounds.z - spawnMargin, maxScreenBounds.z + spawnMargin);
                break;
        }

        return new Vector3(spawnX, playerTransform.position.y, spawnZ);
    }

    void OnDrawGizmos()
    {
        if (mainCamera == null || playerTransform == null) return;

        float distanceToPlayerPlane = Mathf.Abs(mainCamera.transform.position.y - playerTransform.position.y);
        Vector3 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, distanceToPlayerPlane));
        Vector3 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, distanceToPlayerPlane));

        Vector3 p1 = new Vector3(minScreenBounds.x, playerTransform.position.y, minScreenBounds.z);
        Vector3 p2 = new Vector3(maxScreenBounds.x, playerTransform.position.y, minScreenBounds.z);
        Vector3 p3 = new Vector3(maxScreenBounds.x, playerTransform.position.y, maxScreenBounds.z);
        Vector3 p4 = new Vector3(minScreenBounds.x, playerTransform.position.y, maxScreenBounds.z);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);

        Vector3 p1_margin = new Vector3(minScreenBounds.x - spawnMargin, playerTransform.position.y, minScreenBounds.z - spawnMargin);
        Vector3 p2_margin = new Vector3(maxScreenBounds.x + spawnMargin, playerTransform.position.y, minScreenBounds.z - spawnMargin);
        Vector3 p3_margin = new Vector3(maxScreenBounds.x + spawnMargin, playerTransform.position.y, maxScreenBounds.z + spawnMargin);
        Vector3 p4_margin = new Vector3(minScreenBounds.x - spawnMargin, playerTransform.position.y, maxScreenBounds.z + spawnMargin);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p1_margin, p2_margin);
        Gizmos.DrawLine(p2_margin, p3_margin);
        Gizmos.DrawLine(p3_margin, p4_margin);
        Gizmos.DrawLine(p4_margin, p1_margin);

        Gizmos.color = Color.red;
        foreach (Vector3 pos in lastSpawnPositions)
        {
            Gizmos.DrawWireSphere(pos, 1f);
        }
    }
}