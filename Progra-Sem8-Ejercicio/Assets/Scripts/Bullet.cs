using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; 
    public float damage = 10f;
    public float lifeTime = 3f; 
    private float currentLifeTime; 

    void OnEnable() 
    {
        currentLifeTime = lifeTime; 
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
               
        currentLifeTime -= Time.deltaTime;
        if (currentLifeTime <= 0)
        {            
            ObjectPoolManager.Instance.ReturnPooledObject(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);            
            ObjectPoolManager.Instance.ReturnPooledObject(this.gameObject);
        }         
    }
}