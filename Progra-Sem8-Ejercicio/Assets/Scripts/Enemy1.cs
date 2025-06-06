using UnityEngine;

public class Enemy1 : Enemy
{
    public float attackRange = 1.5f; 
    public float attackCooldown = 1.0f;
    private float lastAttackTime;

    public override void Start()
    {
        base.Start();
        health = 30f; 
        moveSpeed = 1.5f; 
        attackDamage = 10f; 
    }

    public override void Attack()
    {
        if (target == null) return;

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (Vector3.Distance(transform.position, target.position) < attackRange)
            {
                IDamageable dmg = target.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(attackDamage);
                    lastAttackTime = Time.time;
                }
            }
        }
    }
}