using UnityEngine;
using GameJolt.API;

public class BossSpawner : MonoBehaviour
{
    [Header("Boss Spawn Settings")]
    public int enemiesNeededForBoss = 10;
    public Transform bossSpawnPoint;
    public bool bossHasSpawned = false;

    private int enemiesKilled = 0;
    private GameObject currentBoss;

    void Start()
    {
        if (bossSpawnPoint == null)
        {
            bossSpawnPoint = this.transform;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log($"Enemigos eliminados: {enemiesKilled}/{enemiesNeededForBoss}");

        if (enemiesKilled >= enemiesNeededForBoss && !bossHasSpawned)
        {
            Trophies.Unlock(269857);
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        if (ObjectPoolManager.Instance != null && ObjectPoolManager.Instance.bossPrefab != null)
        {
            currentBoss = ObjectPoolManager.Instance.GetPooledObject(ObjectPoolManager.Instance.bossPrefab);

            if (currentBoss != null)
            {
                currentBoss.transform.position = bossSpawnPoint.position;
                currentBoss.transform.rotation = bossSpawnPoint.rotation;
                currentBoss.SetActive(true);

                bossHasSpawned = true;

                Debug.Log("¡Boss ha aparecido después de matar 10 enemigos!");

                ShowBossWarning();
            }
            else
            {
                Debug.LogError("No se pudo obtener el boss del pool");
            }
        }
    }

    private void ShowBossWarning()
    {
        Debug.Log("¡ADVERTENCIA: BOSS APARECIDO!");
       
    }

    public void ResetSpawner()
    {
        enemiesKilled = 0;
        bossHasSpawned = false;

        if (currentBoss != null)
        {
            ObjectPoolManager.Instance?.ReturnPooledObject(currentBoss);
            currentBoss = null;
        }
    }
}