using System.Collections;
using UnityEngine;

public class DashPlayer : IBossPattern
{
    public void ExecutePattern(Boss boss)
    {
        boss.StartCoroutine(BossDash(boss));
    }

    private IEnumerator BossDash(Boss boss)
    {
        if (boss.Target == null) yield break;

        Vector3 startPosition = boss.transform.position;
        Vector3 direction = (boss.Target.position - boss.transform.position).normalized;
        direction.y = 0;

        float dashDistance = 8f;
        float dashSpeed = 20f;
        float dashTime = dashDistance / dashSpeed;

       Debug.Log("Boss preparándose para embestir...");
        yield return new WaitForSeconds(0.8f);

        float elapsed = 0f;
        CharacterController controller = boss.GetComponent<CharacterController>();

        while (elapsed < dashTime)
        {
            if (controller != null)
            {
                controller.Move(direction * dashSpeed * Time.deltaTime);
            }
            else
            {
                boss.transform.position += direction * dashSpeed * Time.deltaTime;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pequeña pausa después del dash
        yield return new WaitForSeconds(0.5f);
    }
}