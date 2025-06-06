using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float explosionDelay = 6f;
    public float explosionRadius = 3f;
    public float CurrentExplosionDamage = 20f; 

    private bool hasExploded = false;

    void Start()
    {
        ResetBomb();
        Invoke("Explode", explosionDelay);
    }

    void OnEnable()
    {
        ResetBomb();
        Invoke("Explode", explosionDelay);
    }

    void ResetBomb()
    {
        hasExploded = false;
        CancelInvoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;

        if (other.CompareTag("Player"))
        {
            CancelInvoke("Explode");
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Debug.DrawLine(transform.position, transform.position + Vector3.up * explosionRadius, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.forward * explosionRadius, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * explosionRadius, Color.red, 2f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    damageable.TakeDamage(CurrentExplosionDamage);
                }
            }
        }

        CancelInvoke();
        ObjectPoolManager.Instance.ReturnPooledObject(this.gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}