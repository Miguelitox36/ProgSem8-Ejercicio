using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using GameJolt.API;

public class Boss : Enemy
{
    private List<IBossPattern> patterns = new List<IBossPattern>();
    private int currentPattern = 0;
    private bool isAttacking = false;

    [Header("Boss Pattern Prefabs")]
    public GameObject bulletPrefab;
    public GameObject minionPrefab;

    [Header("Boss Settings")]
    public float attackCooldown = 2f;
       
    public Transform PlayTarget => target;

    public override void Start()
    {
        base.Start();
        
        health = 100f;
               
        patterns.Add(new SpiralShoot());
        patterns.Add(new DashPlayer());
        patterns.Add(new SpawnMinions());
        patterns.Add(new BurstShot());
                
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (target == null)
            {
                Debug.LogError("Boss: No se encontró el jugador con la etiqueta 'Player'.");
            }
        }

        Debug.Log($"Boss inicializado con {patterns.Count} patrones de ataque.");
                
        StartCoroutine(StartFirstAttack());
    }

    private IEnumerator StartFirstAttack()
    {
        yield return new WaitForSeconds(1f);
        StartAttack();
    }

    public override void Update()
    {        
        if (!isAttacking)
        {
            Move();
        }
    }

    public override void Attack()
    {
        
    }

    private IEnumerator ExecuteAttackPattern()
    {
        isAttacking = true;
                
        patterns[currentPattern].ExecutePattern(this);
        Debug.Log($"Boss ejecutando patrón {currentPattern}: {patterns[currentPattern].GetType().Name}");
                
        currentPattern = (currentPattern + 1) % patterns.Count;
                
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
                
        if (gameObject.activeInHierarchy)
        {
            StartAttack();
        }
    }

    public override void Move()
    {        
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0;
                       
            float bossSpeed = moveSpeed * 0.5f;

            if (characterController != null)
            {
                characterController.Move(directionToTarget * bossSpeed * Time.deltaTime);
                                
                if (directionToTarget.magnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(directionToTarget),
                        Time.deltaTime * 2f
                    );
                }
            }
        }
    }

    protected override void Die()
    {
        Trophies.Unlock(269856);
        Debug.Log("¡Boss derrotado!");

        // Detener cualquier corrutina de ataque
        StopAllCoroutines();

        // Dar más puntos por matar al boss
        if (GameManager.Instance != null)
        {
            for (int i = 0; i < 10; i++) // Equivalente a 10 enemigos normales
            {
                GameManager.Instance.EnemyKilled();
            }
        }

        base.Die();
    }

    // Método para iniciar manualmente un ataque
    public void StartAttack()
    {
        if (!isAttacking && gameObject.activeInHierarchy)
        {
            StartCoroutine(ExecuteAttackPattern());
        }
    }

    // Método para obtener los prefabs desde los patrones
    public GameObject GetBulletPrefab() => bulletPrefab;
    public GameObject GetMinionPrefab() => minionPrefab;
}