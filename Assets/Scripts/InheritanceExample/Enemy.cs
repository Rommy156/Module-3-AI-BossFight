using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : GenericCharacter
{
    public float attack = 2f;
    public float defense = 2f;

    [Header("Phase Settings")]
    public float phase2Threshold = 35f;

    private bool isPhase2 = false;

    Animator animator;
    public  int currentHealth; 

    private SimpleStateMachine fsm;

    void Awake()
    {
        fsm = GetComponent<SimpleStateMachine>();
    }

    public override void Move()
    {
        base.Move();
        FightMovement();
    }
    public override void TakeDamage(float damage)
    {
        // apply defense
        float finalDamage = Mathf.Max(damage - defense, 0);

        base.TakeDamage(finalDamage);

        //  Notify FSM (VERY IMPORTANT)
        if (fsm != null)
        {
            fsm.OnTakeDamage(1); // counts hits for retreat logic
        }

        //  Phase check based on HEALTH (not damage)
        if (!isPhase2 && currentHealth <= phase2Threshold)
        {
            TriggerFightPhase2();
        }
    }

    void FightMovement()
    {
        // You can modify behavior based on phase
        if (isPhase2)
        {
            // more aggressive movement
        }
        else
        {
            // normal behavior
        }
    }

    void TriggerFightPhase2()
    {
        isPhase2 = true;

        Debug.Log("Enemy Phase 2!");

        // Example buffs
        attack *= 1.5f;
        defense *= 1.2f;

         animator.SetTrigger("Phase2");
    }
}