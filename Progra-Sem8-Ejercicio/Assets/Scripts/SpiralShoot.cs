using System.Collections;
using UnityEngine;

public class SpiralShoot : IBossPattern
{
    public void ExecutePattern(Boss boss)
    {
        boss.StartCoroutine(SpiralAttack(boss));
    }

    private IEnumerator SpiralAttack(Boss boss)
    {
        if (boss.GetBulletPrefab() == null)
        {
            Debug.LogError("SpiralShoot: bulletPrefab no asignado en el Boss.");
            yield break;
        }

        int waves = 3;
        int bulletsPerWave = 16;
        float waveDelay = 0.3f;

        for (int w = 0; w < waves; w++)
        {
            for (int i = 0; i < bulletsPerWave; i++)
            {
                float angle = (i * 360f / bulletsPerWave) + (w * 22.5f); 
                Vector3 dir = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ).normalized;

                GameObject bullet = ObjectPoolManager.Instance.GetPooledObject(boss.GetBulletPrefab());
                if (bullet != null)
                {
                    bullet.transform.position = boss.transform.position + Vector3.up * 0.5f;
                    bullet.transform.rotation = Quaternion.LookRotation(dir);
                    bullet.SetActive(true);

                    
                    Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                    if (bulletRb != null)
                    {
                        bulletRb.velocity = dir * 8f;
                    }
                }
            }
            yield return new WaitForSeconds(waveDelay);
        }
    }
}