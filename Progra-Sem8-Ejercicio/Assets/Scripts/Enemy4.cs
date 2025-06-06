using UnityEngine;

public class Enemy4 : Enemy
{
    public GameObject bombPrefab;
    public float dropCooldown = 4f;
    public float dropRange = 8f;
    public float bombDamage = 20f;
    private float lastDrop;

    public override void Start()
    {
        base.Start();
        health = 20f;
        moveSpeed = 1.8f;
        attackDamage = bombDamage;
    }
   
    void OnEnable()
    {        
        lastDrop = 0f;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }        
        gameObject.layer = LayerMask.NameToLayer("Enemy"); 
    }

    public override void Attack()
    {
        if (target == null) return;

        if (Time.time > lastDrop + dropCooldown)
        {
            if (Vector3.Distance(transform.position, target.position) < dropRange)
            {
                GameObject bomb = ObjectPoolManager.Instance.GetPooledObject(bombPrefab);
                if (bomb != null)
                {
                    bomb.transform.position = transform.position;
                    bomb.transform.rotation = Quaternion.identity;

                    Bomb bombComponent = bomb.GetComponent<Bomb>();
                    if (bombComponent != null)
                    {
                        bombComponent.CurrentExplosionDamage = bombDamage;
                    }

                    bomb.SetActive(true);
                    lastDrop = Time.time;
                }
            }
        }
    }
        
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        
        Debug.Log($"Enemy4 recibió {damage} de daño. Vida restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }
    
    protected override void Die()
    {        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }       
        ReturnToPool();
    }

    private void ReturnToPool()
    {        
        health = 20f; 
        lastDrop = 0f;
        ObjectPoolManager.Instance.ReturnPooledObject(gameObject);
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            
            Bullet bulletComponent = other.GetComponent<Bullet>();
            float bulletDamage = bulletComponent != null ? bulletComponent.damage : 10f;
            
            TakeDamage(bulletDamage);
            
            ObjectPoolManager.Instance.ReturnPooledObject(other.gameObject);
        }
    }
}