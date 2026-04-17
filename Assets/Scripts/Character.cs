using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Events;

public class Character : GenericCharacter
{
    public float moveSpeed = 5.0f;
    public float rotateSpped = 3f;
    public float gravity = -9.81f;
    public float throwForce = 1.0f;
    public Transform cam;

    public InputActionReference moveInput;
    public InputActionReference Evade;
    public InputActionReference Attack;

    CharacterController controller;
    PlayerInput playerInput;
    Animator animator;
    SoundObject soundObject;
    AudioSource audioSource;


    public float speedSmoothTime;
    float speedSmoothVelocity;
    float currentSpeed;
    public float velocityY;

    Vector3 dir;

    void Awake()
    {
        soundObject = GetComponent<SoundObject>();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        cam = Camera.main.transform;
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (moveInput != null)
        {
            isMoving();
        }
        if (Evade)
        {
            isEvading();

        }
        if (Attack)
        {
            isAttacking();
        }
    }

    void isMoving()
    {

        // Ground check
        if (controller.isGrounded && dir.y < 0)
        {
            dir.y -= Time.deltaTime * gravity; // keeps player grounded
        }

        // Input
        Vector2 input = playerInput.currentActionMap["Move"].ReadValue<Vector2>();

        // Camera-relative directions
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;


        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Direction (NO normalization bug)
        Vector3 move = forward * input.y + right * input.x;

        // Apply speed correctly
        Vector3 moveVelocity = move * moveSpeed;
        Vector3 velocity = (move * currentSpeed);

        // Gravity
        dir.y += gravity * Time.deltaTime;

        // Combine movement + gravity
        moveVelocity.y = velocity.y;

        // Move character
        controller.Move(moveVelocity * Time.deltaTime);

        // Rotation
        Vector3 horizontalVelocity = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
        if (horizontalVelocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                15f * Time.deltaTime
            );

            animator.SetBool("IsMoving", true);

        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
        Vector3 pos = transform.position;

        if (soundObject != null && soundObject.OnSoundTrigger != null)
        {
            soundObject.OnSoundTrigger.Invoke(pos);
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    } 
  
    void isEvading()
        {
             bool IsEvading = playerInput.currentActionMap["Evade"].IsPressed();
            if (IsEvading && moveInput)
            {
                animator.SetBool("IsEvading", true);
                Vector3 evadeDirection = new Vector3(moveInput.action.ReadValue<Vector2>().x, 0, moveInput.action.ReadValue<Vector2>().y).normalized;
                Vector3 evadeVelocity = evadeDirection * moveSpeed * 200;
                Vector3 velocity = Vector3.zero;
                velocity.y += gravity * Time.deltaTime;
                evadeVelocity.y = velocity.y;
            }
            else
            {
                animator.SetBool("IsEvading", false);
            }
    }

        void isAttacking()
        {
            bool isAttacking = playerInput.currentActionMap["Attack"].IsPressed();
            if (isAttacking )
            {
                animator.SetBool("IsAttacking", true);
        }
            else
            {
                animator.SetBool("IsAttacking", false);
            }
        }
}