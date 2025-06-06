using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    public float health = 20f;
    public float moveSpeed = 2f;
    public float attackDamage = 5f;
    protected Transform target;
    protected CharacterController characterController; 

    public Transform Target => target;

    public abstract void Attack();

    public virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogWarning($"Enemy: No se encontró CharacterController en {gameObject.name}. Considera añadirlo para una mejor colisión.");
        }

        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (GameManager.Instance != null)
        {
            float levelMultiplier = 1f + (GameManager.Instance.level * 0.1f);
            health *= levelMultiplier;
            moveSpeed *= levelMultiplier;
        }
    }

    public virtual void Update()
    {
        Move();
        Attack();
    }

    public virtual void Move()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0;

           
            Vector3 avoidanceForce = Vector3.zero;
            if (characterController != null)
            {
                
                Collider[] colliders = Physics.OverlapSphere(transform.position, characterController.radius * 2, LayerMask.GetMask("Enemy"));
                foreach (Collider hit in colliders)
                {
                    Enemy otherEnemy = hit.GetComponent<Enemy>();
                    
                    if (otherEnemy != null && otherEnemy != this && otherEnemy.characterController != null)
                    {
                        Vector3 awayFromOther = (transform.position - otherEnemy.transform.position);
                        float distance = awayFromOther.magnitude;
                        
                        float minSeparationDistance = characterController.radius + otherEnemy.characterController.radius;
                        if (distance < minSeparationDistance + 0.5f && distance > 0.1f) 
                        {                           
                            avoidanceForce += awayFromOther.normalized / distance;
                        }
                    }
                }
                avoidanceForce.y = 0;                 
                Vector3 finalMoveDirection = (directionToTarget + avoidanceForce * 0.5f).normalized;

                characterController.Move(finalMoveDirection * moveSpeed * Time.deltaTime);
                               
                if (directionToTarget.magnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToTarget), Time.deltaTime * moveSpeed);
                }
            }
            else 
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                if (directionToTarget.magnitude > 0.01f)
                {
                    transform.rotation = Quaternion.LookRotation(directionToTarget);
                }
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();
    }

    protected virtual void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }
        ObjectPoolManager.Instance.ReturnPooledObject(this.gameObject);
    }
}