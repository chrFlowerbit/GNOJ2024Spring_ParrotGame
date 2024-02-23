using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;
using System.ComponentModel;

public class PlayerMovementCombat : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public float walkSpeed;
    public float airMinSpeed;

    public float speedIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.E;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    [Header("Ground Check")]
    [SerializeField] bool isAttacking;
    [SerializeField] float attackTime,attackTimeMax;
    [SerializeField] GameObject beakAttackCollider;
    [SerializeField] float attackDamae;
    bool isDashing;
    float isDashingTime, isDashingTimeMax = .3f;


    Vector3 moveDirection;

    Rigidbody rb;

    [HideInInspector]
    public MovementState state;
    [HideInInspector]
    public enum MovementState
    {
        walking,
        crouching,
        air
    }

    bool keepMomentum;
    public bool crouching;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
        isAttacking = false;
        isDashing = false;
        attackTime = 0;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (state == MovementState.walking || state == MovementState.crouching)
            rb.drag = groundDrag;
        else
            rb.drag = groundDrag;

        if (isAttacking)
        {
            attackTime-= Time.deltaTime;
            if(attackTime<0) {
                isAttacking = false;
                beakAttackCollider.SetActive(false);
                

            }
        }
        if (isDashing)
        {
            isDashingTime -= Time.deltaTime;
            if (isDashingTime < 0)
            {
                isDashing = false;
               // beakAttackCollider.SetActive(false);


            }
        }
    }







    public bool CanTakeDamage() {
        //Debug.Log(!(isAttacking || isDashing));
        return !(isAttacking || isDashing); }


    private void FixedUpdate()
    {
        MovePlayer();





    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
       // Debug.Log(horizontalInput);
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump //&& grounded
            )
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            transform.GetComponent<BoxCollider>().size = new Vector3(crouchYScale, crouchYScale, crouchYScale);
            if (state == MovementState.air)
                rb.AddForce(Vector3.down * jumpForce, ForceMode.Impulse);

            //crouching = true;
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            transform.GetComponent<BoxCollider>().size = new Vector3(1, 1, 1);
            //crouching = false;
        }


        //dash
        if (Input.GetKeyDown(dashKey))
        {
            Dash();


        }

        if (Input.GetMouseButtonDown(0))
        {
            Bite();
        }
        //else beakAttackCollider.gameObject.SetActive(false);








    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            bool didIKIlled = false;
           HealthManager enemy= other.GetComponentInParent<HealthManager>();
            // other.TryGetComponent<HealthManager>(out HealthManager enemy );
            Debug.Log(enemy);
            if(enemy != null)
            {
                didIKIlled= enemy.TakeDamage(attackDamae);
                Debug.Log("Attack");

            }
            if(didIKIlled)
            {
                HealthManager healthManager = this.GetComponent<HealthManager>();
                healthManager.Heal(10);
            }

        }




    }


    private void Bite()
    {
        beakAttackCollider.gameObject.SetActive(true);
        moveDirection = orientation.forward * 0.25f + orientation.right * 0;
        float dashForce = 5f;

        Vector3 dashDir = moveDirection.normalized * dashForce;

        if (dashDir != Vector3.zero) 
        {
            Quaternion dashRotation = Quaternion.LookRotation(dashDir, Vector3.up);
            rb.rotation = dashRotation; 
        }

        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);


        attackTime = attackTimeMax;
        isAttacking = true;

    }

    private void Dash()
    {
        isDashing = true;
        isDashingTime = isDashingTimeMax;


        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Debug.Log(moveDirection.ToString());
        float dashForce = 15f;

        Vector3 dashDir;

        dashDir = moveDirection.normalized* dashForce;

        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
        isDashingTime = isDashingTimeMax;
        

    }

    private void StateHandler()
    {       
        if (crouching)
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
              
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;
            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //// on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        //// in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

}
