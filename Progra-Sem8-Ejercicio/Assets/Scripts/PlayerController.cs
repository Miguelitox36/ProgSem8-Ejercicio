using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject bulletPrefab; 
    public Transform firePoint;
    public float fireRate = 0.5f; 
    private float nextFireTime = 0f;

    void Update()
    {
        Move();
        RotateToMouse();
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime) 
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(h, 0, v).normalized; 
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);       
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        float distance;

        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = (point - transform.position).normalized;
            direction.y = 0; 
            transform.forward = direction;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("Bullet Prefab o Fire Point no asignado en PlayerController.");
            return;
        }

        GameObject bullet = ObjectPoolManager.Instance.GetPooledObject(bulletPrefab);

        if (bullet == null)
        {
            Debug.LogError("No se pudo obtener una bala del ObjectPool.");
            return;
        }

        
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        
        bullet.SetActive(true);

        Bullet bulletScript = bullet.GetComponent<Bullet>();        
    }
}