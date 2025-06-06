using System.Collections;
using UnityEngine;

public class SpawnMinions : IBossPattern
{
    public void ExecutePattern(Boss boss)
    {
        boss.StartCoroutine(SpawnRoutine(boss));
    }

    private IEnumerator SpawnRoutine(Boss boss)
    {
        if (boss.GetMinionPrefab() == null)
        {
            Debug.LogError("SpawnMinions: minionPrefab no asignado en el Boss.");
            yield break;
        }

        int minionsToSpawn = 2 + (GameManager.Instance != null ? GameManager.Instance.level / 3 : 0);
        float spawnDelay = 0.4f;

        for (int i = 0; i < minionsToSpawn; i++)
        {            
            float angle = i * (360f / minionsToSpawn);
            Vector3 spawnOffset = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * 4f,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * 4f
            );
            Vector3 spawnPosition = boss.transform.position + spawnOffset;

            GameObject minion = ObjectPoolManager.Instance.GetPooledObject(boss.GetMinionPrefab());
            if (minion != null)
            {
                minion.transform.position = spawnPosition;
                minion.transform.rotation = Quaternion.identity;
                minion.SetActive(true);
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}
