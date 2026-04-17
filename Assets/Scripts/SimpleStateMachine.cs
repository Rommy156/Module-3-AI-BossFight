using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SimpleStateMachine : EnemyAttack
{
    enum State { Circle, Approach, Attack1, Attack2, Charge }

    [Header("Scene References")]
    public Transform character;
    public Transform[] patrolWaypoints;
    public TextMeshProUGUI stateText;

    [Header("Vision")]
    public float viewRadius = 2f;
    [Range(0, 360)]
    public float viewAngle = 60f;

    [Header("Ranges")]
    public float attackRange = .1f;
    public float approachRange = 2f;

    [Header("Combat")]
    public float attackCooldown = 2f;
    public int hitsToCombo = 2;
    public int hitsTakenToRetreat = 1;
    public float chaseGraceTime = 0f;
    public GameObject hitbox;

    private int successfulHits = 0;
    private int hitsTaken = 0;
    private bool hasAttackedThisState = false;

    [Header("Config")]
    private float idleTimeThreshold = 2f;
    private float circleTimeThreshold = 2f;
    private float rotationSpeed = 3f;
    private float attack1TimeThreshold = 5.0f;
    private float attack2TimeThreshold = 3.0f;
    private float retreatTimeThreshold = 3.0f;

    public float hitRange = 1.5f;
    public float hitAngle = 60f;

    private Vector3 chargeDir;
    private bool isCharging = false;

    private State state;
    private NavMeshAgent agent;
    private Animator anim;
    private bool canSeePlayer;

    float approachTime;
    float circleTime;
    float attack1Time;
    float attack2Time;
    float retreatTime;

    bool soundHeard;
    Vector3 soundPosition;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        state = State.Circle;
        hitbox.SetActive(false);
    }

    void Update()
    {
        canSeePlayer = IsInViewCone();

        switch (state)
        {
            case State.Circle: Circle(); break;
            case State.Approach: Approach(); break;
            case State.Attack1: Attack1(); break;
            case State.Attack2: Attack2(); break;
            case State.Charge: Approach2(); break;
        }

        // Always face player except during charge (so dash stays straight)
        if (!isCharging)
        {
            Vector3 dir = (character.position - transform.position).normalized;
            dir.y = 0;

            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
        }

        if (stateText != null)
            stateText.text = "State: " + state;
    }

    public void OnAttackHit()
    {
        if (!IsPlayerInHitRange()) return;

        anim.SetBool("IsHit", true);
        successfulHits++;

        bool IsPlayerInHitRange()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, 1f);

            foreach (var col in hits)
            {
                if (col.CompareTag("Player"))
                    return true;
            }
            return false;
        }
    }
    public void SoundTrigger(Vector3 pos)
    {
        soundHeard = true;
        soundPosition = pos;
        state = State.Approach;
    }
    public void OnTakeDamage(int dmg)
    {
        hitsTaken++;
        Debug.Log("Enemy took damage: " + hitsTaken);
    }

    void ResetAnimatorBools()
    {
        anim.SetBool("IsApproaching", false);
        anim.SetBool("CanAttack1", false);
        anim.SetBool("CanAttack2", false);
        anim.SetBool("IsReatreating", false);
    }

    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }

    void Circle()
    {
        if (Time.time - circleTime >= circleTimeThreshold)
        {
            StartCircle();
        }

        agent.SetDestination(character.position);
    }

    void StartCircle()
    {
        state = State.Circle;
        circleTime = Time.time;

        ResetAnimatorBools();

        float dist = Vector3.Distance(transform.position, character.position);

        if (canSeePlayer && dist < attackRange)
        {
            StartAttack1();
            anim.SetBool("CanAttack1", true);
        }
        else if (canSeePlayer)
        {
            StartApproach();
            anim.SetBool("IsApproaching", true);
        }
    }

    void Approach()
    {
        agent.SetDestination(character.position);

        if (Time.time - approachTime >= idleTimeThreshold)
        {
            StartApproach();
        }
    }

    void StartApproach()
    {
        state = State.Approach;
        approachTime = Time.time;

        ResetAnimatorBools();
        anim.SetBool("IsApproaching", true);

        float dist = Vector3.Distance(transform.position, character.position);

        if (dist <= attackRange)
        {
            StartAttack1();
        }

        if (hitsTaken >= hitsTakenToRetreat && state != State.Charge)
        {
            StartApproach2();
        }
    }

    void Attack1()
    {
        agent.SetDestination(transform.position);

        if (!hasAttackedThisState)
        {
            hasAttackedThisState = true;
            Debug.Log("Attack 1");
        }

        if (Time.time - attack1Time >= attack1TimeThreshold)
        {
            if (successfulHits >= 1)
            {
                StartAttack2();
                anim.SetBool("CanAttack2", true);
            }
            else
            {
                StartCircle(); 
                anim.SetBool("IsCircling", true);
            }
            return;
        }
    }

    void StartAttack1()
    {
        state = State.Attack1;
        attack1Time = Time.time;
        hasAttackedThisState = false;

        ResetAnimatorBools();
        anim.SetBool("CanAttack1", true);
    }

    void Attack2()
    {
        agent.SetDestination(transform.position);

        if (!hasAttackedThisState)
        {
            hasAttackedThisState = true;
            if (Time.time - attack2Time >= attack2TimeThreshold)
            {
                StartAttack2();
                anim.SetBool("CanAttack2", true);
            }
            else
            {
                StartApproach();
                anim.SetBool("IsApproaching", true);
            }
            return;
        }


    }

    void StartAttack2()
    {
        state = State.Attack2;
        attack2Time = Time.time;
        hasAttackedThisState = false;

        ResetAnimatorBools();
        anim.SetBool("CanAttack2", true);
    }

    void Approach2()
    {
        float elapsed = Time.time - retreatTime;

        agent.Move(chargeDir * agent.speed * 3f * Time.deltaTime);

        if (elapsed >= retreatTimeThreshold)
        {
            isCharging = false;
            hitsTaken = 0;
            anim.SetBool("IsCircling", true);
            StartCircle();
        }
    }

    void StartApproach2()
    {
        state = State.Charge;
        retreatTime = Time.time;

        ResetAnimatorBools();
        anim.SetBool("IsReatreating", true);

        chargeDir = (character.position - transform.position).normalized;
        chargeDir.y = 0;

        isCharging = true;
    }

    bool IsInViewCone()
    {
        Vector3 toPlayer = character.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > viewRadius) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer.normalized);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, viewRadius))
            return hit.transform == character;

        return false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange / 2);
    }
}