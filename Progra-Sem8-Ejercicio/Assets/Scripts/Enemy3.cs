using System.Collections;
using UnityEngine;

public class Enemy3 : Enemy
{
    public float dashSpeed = 15f; 
    public float dashCooldown = 3f;
    public float dashRange = 7f; 
    public float dashDamage = 15f; 
    private float lastDash;
    private bool isDashing = false;

    
    private Vector3 dashDirection;
    private float dashTimeRemaining;
    private float dashDuration = 0.3f;

    public override void Start()
    {
        base.Start();
        health = 25f; 
        moveSpeed = 3f; 
        attackDamage = dashDamage; 
    }

    public override void Attack()
    {
        if (target == null || isDashing) return;

        if (Time.time > lastDash + dashCooldown)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget < dashRange)
            {
                StartDash();
                lastDash = Time.time;
            }
        }
    }

    void StartDash()
    {
        isDashing = true;

        // Calcular la dirección del dash solo en el plano XZ (horizontal)
        Vector3 targetDirection = target.position - transform.position;
        targetDirection.y = 0; // Eliminar componente vertical
        dashDirection = targetDirection.normalized;

        dashTimeRemaining = dashDuration;

        Debug.Log($"Enemy3 iniciando dash hacia: {dashDirection}");
    }

    public override void Move()
    {
        if (isDashing)
        {           
            if (dashTimeRemaining > 0)
            {                
                Vector3 dashMovement = dashDirection * dashSpeed * Time.deltaTime;
                                
                characterController.Move(dashMovement);

                dashTimeRemaining -= Time.deltaTime;
            }
            else
            {
               isDashing = false;
                CheckDashHit();
                Debug.Log("Enemy3 dash terminado");
            }
        }
        else
        {            
            base.Move();
        }
    }

    void CheckDashHit()
    {
        // Verificar si golpeó al jugador después del dash
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, 1.5f, LayerMask.GetMask("Player"));

        foreach (Collider playerCol in hitPlayers)
        {
            IDamageable dmg = playerCol.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(attackDamage);
                Debug.Log($"Enemy3 golpeó al jugador con dash por {attackDamage} de daño");
                break; 
            }
        }
    }
       
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, dashRange);

            if (isDashing)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(transform.position, dashDirection * dashSpeed);
            }
        }
    }
}