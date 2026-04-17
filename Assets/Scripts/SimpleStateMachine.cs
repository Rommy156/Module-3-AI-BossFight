using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SimpleStateMachine : EnemyAttack
{
    //define states
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
    //set default state
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

        // Always face player except during charge 
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
    //animation event to apply damage
    public void OnAttackHit()
    {
        if (!IsPlayerInHitRange()) return;

        anim.SetBool("IsHit", true);
        successfulHits++;

        // checks if player is within hit radius
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
    //sound trigger for mutant to investigate
    public void SoundTrigger(Vector3 pos)
    {
        soundHeard = true;
        soundPosition = pos;
        state = State.Approach;
    }

    //called when enemy takes damage
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
    //enable hitbox during attack animation, called by animation event
    public void EnableHitbox()
    {
        hitbox.SetActive(true);
    }
    //disable hitbox after attack animation, called by animation event
    public void DisableHitbox()
    {
        hitbox.SetActive(false);
    }
    //default state where enemy circles around player, transitions to approach if player is seen or after certain time
    void Circle()
    {
        if (Time.time - circleTime >= circleTimeThreshold)
        {
            StartCircle();
        }
        agent.SetDestination(character.position);
    }

    //initialize Circle state, checks if player is in range to attack or approach
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
    //move towards player, transitions to attack if in range or approach2
    //if too many hits taken
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
    //first attack in combo, if successful hit transitions to attack2,
    //otherwise back to circling
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
    // second combo attack logic
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

    //charge/dash movement behavior
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

    // checks if player is within vision cone
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