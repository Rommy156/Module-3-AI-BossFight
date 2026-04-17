using UnityEngine;


public class PlayerCombat : MonoBehaviour
{
    public float range = .5f;
    public float radius = 1.5f;
    public int damage = 1;
    bool hasHit;
    float lastHitTime;
    public float hitCooldown = 1f;


    // Assign the enemy GameObject that has SimpleStateMachine on it
    public SimpleStateMachine fsm;

    // Called from an Animation Event on the attack swing frame
    public void DealDamage()
    {
        Debug.Log("Damage frame!");

        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.up * range,
            radius
        );
        
        foreach (Collider hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);

                SimpleStateMachine fsm = hit.GetComponent<SimpleStateMachine>();
                if (fsm != null)
                {
                    fsm.OnTakeDamage(damage); 
                }
            }
        }
    }
    void OnEnable()
    {
        hasHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.TakeDamage(damage);
        }

        SimpleStateMachine enemy = GetComponentInParent<SimpleStateMachine>();
        if (enemy != null)
        {
            enemy.OnAttackHit();
        }
    }
    void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            // Show the attack sphere in the scene view
            Gizmos.DrawWireSphere(
                transform.position + transform.up * range,
                radius
            );
    }
}