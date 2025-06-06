using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private List<GameObject> pooledObjects;
    private GameObject prefab;
    private int initialPoolSize;
    private Transform parentTransform;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.initialPoolSize = initialSize;
        this.parentTransform = parent;
        pooledObjects = new List<GameObject>();

       
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parentTransform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }

        Debug.Log($"ObjectPool creado para {prefab.name} con {initialPoolSize} objetos");
    }

   
    public GameObject GetPooledObject()
    {       
        foreach (GameObject obj in pooledObjects)
        {
            if (obj != null && !obj.activeInHierarchy)
            {
                Debug.Log($"Objeto {obj.name} reutilizado del pool. Total en pool: {pooledObjects.Count}");
                return obj;
            }
        }

        Debug.LogWarning($"ObjectPool: Todos los objetos de {prefab.name} están en uso. Pool size: {pooledObjects.Count}");

        
        if (pooledObjects.Count < initialPoolSize * 2)
        {
            GameObject newObj = GameObject.Instantiate(prefab, parentTransform);
            newObj.SetActive(false);
            pooledObjects.Add(newObj);
            Debug.LogWarning($"ObjectPool: Se creó un nuevo objeto para el pool de {prefab.name}. Total ahora: {pooledObjects.Count}");
            return newObj;
        }
       
        GameObject oldestObj = pooledObjects[0];
        ReturnPooledObject(oldestObj); 
        return oldestObj;
    }       
    public void ReturnPooledObject(GameObject obj)
    {
        if (obj == null) return;

        Debug.Log($"Objeto {obj.name} devuelto al pool");

        obj.SetActive(false);
                
        if (parentTransform != null)
        {
            obj.transform.position = parentTransform.position;
            obj.transform.rotation = parentTransform.rotation;
        }
        else
        {
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
        }
               
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        BulletEnemy bullet = obj.GetComponent<BulletEnemy>();
        if (bullet != null)
        {
           
            bullet.CancelInvoke();
        }
    }
       
    public int GetActiveCount()
    {
        int activeCount = 0;
        foreach (GameObject obj in pooledObjects)
        {
            if (obj != null && obj.activeInHierarchy)
                activeCount++;
        }
        return activeCount;
    }

    public int GetTotalCount()
    {
        return pooledObjects.Count;
    }
}