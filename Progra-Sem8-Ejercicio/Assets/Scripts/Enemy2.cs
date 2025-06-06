using UnityEngine;
using System.Collections;

public class Enemy2 : Enemy
{
    public GameObject projectilePrefab;
    public float shootCooldown = 8f;
    public float shootRange = 10f;
    public float projectileSpawnOffset = 0.5f;

    private Coroutine shootRoutineInstance;
    private float lastShootTime = 0f; 
        
    public override void Start()
    {
        base.Start(); 
        health = 15f;
        moveSpeed = 2.5f;
        attackDamage = 5f;
        lastShootTime = Time.time;
    }
      
    void OnEnable()
    {
        
        lastShootTime = Time.time;
        
        if (shootRoutineInstance != null)
        {
            StopCoroutine(shootRoutineInstance);
        }                
        shootRoutineInstance = StartCoroutine(ShootRoutine());
    }

   
    void OnDisable()
    {
        if (shootRoutineInstance != null)
        {
            StopCoroutine(shootRoutineInstance);
            shootRoutineInstance = null;
        }
    }
        
    IEnumerator ShootRoutine()
    {
        
        yield return new WaitForSeconds(1f);

        while (true) 
        {
            
            if (target != null &&
                Vector3.Distance(transform.position, target.position) < shootRange &&
                Time.time >= lastShootTime + shootCooldown)
            {
                Attack(); 
                lastShootTime = Time.time;
            }
           
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    public override void Attack()
    {
        if (target == null)
        {
            return;
        }

        
        if (Vector3.Distance(transform.position, target.position) < shootRange)
        {
            Debug.Log($"[{Time.time}] Enemy2 ({gameObject.name}) disparando proyectil. Instancia: {GetInstanceID()}");

            
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            Vector3 spawnPosition = transform.position + directionToTarget * projectileSpawnOffset;

            
            GameObject projectile = ObjectPoolManager.Instance.GetPooledObject(projectilePrefab);

            if (projectile != null)
            {
                
                projectile.transform.position = spawnPosition;                
                projectile.transform.rotation = Quaternion.LookRotation(directionToTarget);
               
                BulletEnemy bulletEnemy = projectile.GetComponent<BulletEnemy>();
                if (bulletEnemy != null)
                {                    
                    projectile.SetActive(true);
                    bulletEnemy.Initialize(directionToTarget, attackDamage);
                }
                else
                {
                    Debug.LogError("Enemy2: El prefab de proyectil no tiene el script BulletEnemy.");
                    ObjectPoolManager.Instance.ReturnPooledObject(projectile);
                    return;
                }

                Debug.Log($"[{Time.time}] Proyectil disparado hacia dirección: {directionToTarget}");
            }
            else
            {
                Debug.LogError($"Enemy2: No se pudo obtener el objeto del pool para el prefab {projectilePrefab.name}.");
            }
        }
    }

    
    public override void Move()
    {
        
        if (target != null && Vector3.Distance(transform.position, target.position) > shootRange * 0.8f)
        {
            base.Move(); 
        }
        
        else if (target != null && Vector3.Distance(transform.position, target.position) < shootRange * 0.7f)
        {
           
            Vector3 directionAwayFromPlayer = (transform.position - target.position).normalized;
            transform.position += directionAwayFromPlayer * moveSpeed * Time.deltaTime;
        }
    }
}