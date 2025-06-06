using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;
        
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    [Header("Pool Settings")]
    public GameObject playerBulletPrefab;
    public int playerBulletPoolSize = 50;

    public GameObject enemyProjectilePrefab;
    public int enemyProjectilePoolSize = 20;

    public GameObject enemy1Prefab;
    public int enemy1PoolSize = 10;
    public GameObject enemy2Prefab;
    public int enemy2PoolSize = 10;
    public GameObject enemy3Prefab;
    public int enemy3PoolSize = 10;
    public GameObject enemy4Prefab;
    public int enemy4PoolSize = 10;

    public GameObject bossMinionPrefab;
    public int bossMinionPoolSize = 15;

    public GameObject bombPrefab;
    public int bombPoolSize = 10;

    public GameObject bossPrefab;
    public int bossPoolSize = 1;
        
    public GameObject bossBulletPrefab;
    public int bossBulletPoolSize = 50;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializePools()
    {        
        if (playerBulletPrefab != null)
        {
            pools.Add(playerBulletPrefab.name, new ObjectPool(playerBulletPrefab, playerBulletPoolSize, this.transform));
        }
                
        if (enemyProjectilePrefab != null)
        {
            pools.Add(enemyProjectilePrefab.name, new ObjectPool(enemyProjectilePrefab, enemyProjectilePoolSize, this.transform));
        }

        // Inicializar pools para los enemigos
        if (enemy1Prefab != null) pools.Add(enemy1Prefab.name, new ObjectPool(enemy1Prefab, enemy1PoolSize, this.transform));
        if (enemy2Prefab != null) pools.Add(enemy2Prefab.name, new ObjectPool(enemy2Prefab, enemy2PoolSize, this.transform));
        if (enemy3Prefab != null) pools.Add(enemy3Prefab.name, new ObjectPool(enemy3Prefab, enemy3PoolSize, this.transform));
        if (enemy4Prefab != null) pools.Add(enemy4Prefab.name, new ObjectPool(enemy4Prefab, enemy4PoolSize, this.transform));

        // Inicializar pool para los minions del boss
        if (bossMinionPrefab != null) pools.Add(bossMinionPrefab.name, new ObjectPool(bossMinionPrefab, bossMinionPoolSize, this.transform));

        // Inicializar pool para las bombas
        if (bombPrefab != null) pools.Add(bombPrefab.name, new ObjectPool(bombPrefab, bombPoolSize, this.transform));

        // Inicializar pool para el Boss
        if (bossPrefab != null)
        {
            pools.Add(bossPrefab.name, new ObjectPool(bossPrefab, bossPoolSize, this.transform));
        }

        // Inicializar pool para las balas del boss
        if (bossBulletPrefab != null)
        {
            pools.Add(bossBulletPrefab.name, new ObjectPool(bossBulletPrefab, bossBulletPoolSize, this.transform));
        }

        Debug.Log($"ObjectPoolManager inicializado con {pools.Count} pools.");
    }
        
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogError("ObjectPoolManager: Prefab es null");
            return null;
        }
                
        string prefabKey = prefab.name.Replace("(Clone)", "");

        if (pools.TryGetValue(prefabKey, out ObjectPool pool))
        {
            GameObject obj = pool.GetPooledObject();

            if (obj != null)
            {
                Debug.Log($"Objeto {prefabKey} obtenido del pool. Activos: {pool.GetActiveCount()}/{pool.GetTotalCount()}");
                return obj;
            }
        }

        Debug.LogError($"ObjectPoolManager: No se encontró un pool para el prefab {prefabKey} o no hay objetos disponibles.");
        return null;     }

    
    public void ReturnPooledObject(GameObject obj)
    {
        if (obj == null) return;
               
        string originalName = obj.name.Replace("(Clone)", "");

        if (pools.TryGetValue(originalName, out ObjectPool pool))
        {
            pool.ReturnPooledObject(obj);
            Debug.Log($"Objeto {originalName} devuelto al pool. Activos: {pool.GetActiveCount()}/{pool.GetTotalCount()}");
        }
        else
        {
            Debug.LogWarning($"ObjectPoolManager: No se encontró pool para {originalName}, destruyendo objeto.");
            Destroy(obj);
        }
    }

    // Método de debug para verificar el estado de los pools
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugPoolStatus()
    {
        foreach (var kvp in pools)
        {
            Debug.Log($"Pool {kvp.Key}: {kvp.Value.GetActiveCount()}/{kvp.Value.GetTotalCount()} activos");
        }
    }
}
