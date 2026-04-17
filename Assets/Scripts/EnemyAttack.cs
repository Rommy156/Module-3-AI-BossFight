using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 10;
    public float hitCooldown = 1f;

    private float lastHitTime;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - lastHitTime < hitCooldown) return;

        lastHitTime = Time.time;

        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.TakeDamage(damage);
        }
    }
}