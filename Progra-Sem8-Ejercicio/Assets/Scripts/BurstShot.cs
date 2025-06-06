using UnityEngine;
using System.Collections;
using UnityEngine;

public class BurstShot : IBossPattern
{
    public void ExecutePattern(Boss boss)
    {
        boss.StartCoroutine(BurstAttack(boss));
    }

    private IEnumerator BurstAttack(Boss boss)
    {
        if (boss.GetBulletPrefab() == null || boss.Target == null)
        {
            Debug.LogError("BurstShot: bulletPrefab o Target no asignado en el Boss.");
            yield break;
        }

        int bursts = 4;
        int bulletsPerBurst = 5;
        float burstDelay = 0.6f;
        float bulletDelay = 0.1f;

        for (int b = 0; b < bursts; b++)
        {
            // Calcular dirección hacia el jugador al inicio de cada ráfaga
            Vector3 directionToPlayer = (boss.Target.position - boss.transform.position).normalized;

            for (int i = 0; i < bulletsPerBurst; i++)
            {
                // Dispersión de las balas
                float spreadAngle = (i - 2) * 15f; // -30, -15, 0, 15, 30 grados
                Vector3 spreadDirection = RotateVector(directionToPlayer, spreadAngle);

                GameObject bullet = ObjectPoolManager.Instance.GetPooledObject(boss.GetBulletPrefab());
                if (bullet != null)
                {
                    bullet.transform.position = boss.transform.position + Vector3.up * 0.5f;
                    bullet.transform.rotation = Quaternion.LookRotation(spreadDirection);
                    bullet.SetActive(true);

                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                    if (bulletRb != null)
                    {
                        bulletRb.velocity = spreadDirection * 12f;
                    }
                }
                yield return new WaitForSeconds(bulletDelay);
            }
            yield return new WaitForSeconds(burstDelay);
        }
    }

    private Vector3 RotateVector(Vector3 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector3(
            vector.x * cos - vector.z * sin,
            vector.y,
            vector.x * sin + vector.z * cos
        );
    }
}