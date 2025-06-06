using UnityEngine;

public class MinionEnemy : Enemy
{
    public float minionAttackRange = 1.0f;
    public float minionAttackCooldown = 1.5f;
    private float lastMinionAttackTime;

    public override void Start()
    {
        base.Start(); 
        health = 10f; 
        moveSpeed = 3f; 
        attackDamage = 5f; 
    }

    public override void Attack()
    {
        if (target == null) return;

        if (Time.time >= lastMinionAttackTime + minionAttackCooldown)
        {
            
            if (Vector3.Distance(transform.position, target.position) < minionAttackRange)
            {
                IDamageable dmg = target.GetComponent<IDamageable>();
                if (dmg != null)
                {
                    dmg.TakeDamage(attackDamage);
                    lastMinionAttackTime = Time.time;
                }
            }
        }
    }   
}