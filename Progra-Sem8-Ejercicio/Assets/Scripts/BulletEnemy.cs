using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public float damage;

    private Vector3 moveDirection;
    private bool isActive = false;

    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }

    public void Initialize(Vector3 direction, float bulletDamage)
    {
        moveDirection = direction.normalized;
        damage = bulletDamage;
        isActive = true;

        // Cancelar cualquier Invoke anterior
        CancelInvoke("Deactivate");

        // Programar desactivación
        Invoke("Deactivate", lifetime);

        Debug.Log($"Bala inicializada con dirección: {direction} y daño: {bulletDamage}");
    }

    void OnEnable()
    {
       
    }

    void OnDisable()
    {        
        CancelInvoke("Deactivate");
        isActive = false;
        moveDirection = Vector3.zero;
    }

    void Update()
    {
        
        if (isActive && moveDirection != Vector3.zero)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;                
        if (other.CompareTag("Enemy")) return;
        
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            Debug.Log($"Bala causó {damage} de daño a {other.name}");
            Deactivate();
        }
        
        else if (other.CompareTag("Player"))
        {
            Debug.Log($"Bala chocó con {other.name}");
            Deactivate();
        }
    }

    void Deactivate()
    {
        if (!isActive) return;

        Debug.Log("Bala desactivada");

        // Cancelar la invocación para evitar llamadas múltiples
        CancelInvoke("Deactivate");
                
        isActive = false;
        moveDirection = Vector3.zero;
               
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnPooledObject(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}