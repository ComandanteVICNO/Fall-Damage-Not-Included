using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

public class PlayerMovement : MonoBehaviour
{

    [Header("Referencias")]
    public Camera playerCam;
    public PlayerCamera pc;
    public AudioSource playerAudioSource;

    [Header("Movement")]

    private float moveSpeed;
    public float dashSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float wallRunSpeed;
    public float airDrag;
    public float groundDrag;
    

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;



    public float fallingCount = 0f;

    [Header("Crouching")]
    public float crouchSpeed;
    public float ceilingCheckDelay;
    public bool isCrouching;
    float getUpDelay;
    public float crouchYScale;
    private float startYScale;
    


    [Header("KeyBindings")]
    public KeyCode jumpkey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;
    public KeyCode slideKey = KeyCode.X;

    [Header("FootSteps")]
    public bool useFootsteps = true;
    public float walkStepSpeed = 0.4f;
    public float crouchStepSpeed = 0.9f;
    public float sprintStepSpeed = 0.3f;
    public float wallrunningStepSpeed = 0.3f;
    public float footstepTimer = 0;



    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    bool checkCeiling;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public float slideYScale;

    private bool sliding;
    public bool restricted;

    [Header("Sounds")]
    //Walking footsteps
    public AudioClip walking01;
    public AudioClip walking02;
    public AudioClip walking03;
    public AudioClip walking04;
    public AudioClip walking05;

    //Running footsteps
    public AudioClip running01;
    public AudioClip running02;
    public AudioClip running03;
    public AudioClip running04;
    public AudioClip running05;
    public AudioClip running06;

    //landing
    public AudioClip landingLight01;
    public AudioClip landingLight02;
    public AudioClip landingLight03;
    public AudioClip landingHeavy;

    public AudioClip slideSound;
    public AudioClip emptySound;
    public AudioClip grapplingSound;
    public AudioClip grapplingPulling;
    public AudioClip dashSound;


    [Header("Outras Coisas")]
    public Transform orientation;
    public float fovValue;
    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public int randomNumber;

    [Header("Movement State")]
    public MovementState state;

    public enum MovementState
    {
        dashing,
        freeze,
        unlimited,
        walking,
        sprinting,
        crouching,
        wallrunnng,
        air,
        sliding
    }
    public bool freeze;
    public bool unlimited;
    public bool activeGrapple;
    public bool dashing;

    public bool wallrunning;

    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true;
        readyToJump = true;
        fovValue = pc.originalFov;
        startYScale = transform.localScale.y;

    }

private void Update()
    {
        //Movimento
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        if ( grounded && fallingCount >= 5f)
        {
            
            playerAudioSource.pitch = 1f;
            playerAudioSource.PlayOneShot(landingHeavy);
        }
        else if(grounded && (fallingCount<5f && fallingCount >0)) 
        {
            randomNumber = UnityEngine.Random.Range(1, 4);

            switch (randomNumber)
            {
                case 1:
                    playerAudioSource.PlayOneShot(landingLight01);
                    break;
                case 2:
                    playerAudioSource.PlayOneShot(landingLight02);
                    break;
                case 3:
                    playerAudioSource.PlayOneShot(landingLight03);
                    break;

            }
        }

        checkCeiling = Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f, whatIsGround);
        

        MyInput();
        SpeedControl();
        StateHandler();
        FootstepsHandler();


        //handle drag
        if (grounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else if (activeGrapple)
        {
            rb.drag = 0f;
        }
        else
        {
            rb.drag = airDrag;
        }

        //Slide
        if (Input.GetKeyDown(slideKey) && Input.GetKey(sprintKey) && (horizontalInput != 0 || verticalInput != 0) && grounded)
        {
            StartSlide();
        }

        if (Input.GetKeyUp(slideKey) && sliding)
        {
            StopSlide();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
        if (sliding)
        {
            SlidingMovement();
        }
       
    }


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpkey) && readyToJump && grounded) 
        { 
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //start crouch
        if (Input.GetKeyDown(crouchKey) && grounded)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            getUpDelay = ceilingCheckDelay;
            isCrouching = true;
        }

        //stop crouch
        if (!Input.GetKey(crouchKey) && !checkCeiling)
        {


            getUpDelay -= Time.deltaTime;
            if (getUpDelay <= 0)
            {
                transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
                isCrouching = false;
            }
            
        }
    }

    private void MovePlayer()
    {
        if (restricted) return;

        if (activeGrapple) return;
        //calcular direção do movimento
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        //no chão
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force );

        //in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //desligar gravidade num slope
        rb.useGravity = !OnSlope();

    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        //Limitar velocidade num slope
        if(OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            //limitar velocidade
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;

        //Resetar velocidade no Y
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        JumpingSound();
    }
    private void ResetJump() 
    {
        readyToJump = true;
        exitingSlope = false;
    }

    private void FootstepsHandler()
    {
        if (state == MovementState.air) return;
        if (rb.velocity.magnitude < 0.1f) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (state == MovementState.walking)
            {
                randomNumber = UnityEngine.Random.Range(1, 6);
                switch (randomNumber)
                {
                    case 1:
                        playerAudioSource.PlayOneShot(walking01);
                        footstepTimer = walkStepSpeed;
                        break;
                    case 2:
                        playerAudioSource.PlayOneShot(walking02);
                        footstepTimer = walkStepSpeed;
                        break;
                    case 3:
                        playerAudioSource.PlayOneShot(walking03);
                        footstepTimer = walkStepSpeed;
                        break;
                    case 4:
                        playerAudioSource.PlayOneShot(walking04);
                        footstepTimer = walkStepSpeed;
                        break;
                    case 5:
                        playerAudioSource.PlayOneShot(walking05);
                        footstepTimer = walkStepSpeed;
                        break;
                }

            }
            else if (state == MovementState.crouching)
            {
                randomNumber = UnityEngine.Random.Range(1, 6);
                switch (randomNumber)
                {
                    case 1:
                        playerAudioSource.PlayOneShot(walking01);
                        footstepTimer = crouchStepSpeed;
                        break;
                    case 2:
                        playerAudioSource.PlayOneShot(walking02);
                        footstepTimer = crouchStepSpeed;
                        break;
                    case 3:
                        playerAudioSource.PlayOneShot(walking03);
                        footstepTimer = crouchStepSpeed;
                        break;
                    case 4:
                        playerAudioSource.PlayOneShot(walking04);
                        footstepTimer = crouchStepSpeed;
                        break;
                    case 5:
                        playerAudioSource.PlayOneShot(walking05);
                        footstepTimer = crouchStepSpeed;
                        break;
                }

            }
            else if (state == MovementState.sprinting)
            {
                randomNumber = UnityEngine.Random.Range(1, 7);
                switch (randomNumber)
                {
                    case 1:
                        playerAudioSource.PlayOneShot(running01);
                        footstepTimer = sprintStepSpeed;
                        break;
                    case 2:
                        playerAudioSource.PlayOneShot(running02);
                        footstepTimer = sprintStepSpeed;
                        break;
                    case 3:
                        playerAudioSource.PlayOneShot(running03);
                        footstepTimer = sprintStepSpeed;
                        break;
                    case 4:
                        playerAudioSource.PlayOneShot(running04);
                        footstepTimer = sprintStepSpeed;
                        break;
                    case 5:
                        playerAudioSource.PlayOneShot(running05);
                        footstepTimer = sprintStepSpeed;
                        break;
                    case 6:
                        playerAudioSource.PlayOneShot(running06);
                        footstepTimer = sprintStepSpeed;
                        break;
                }
            }
            else if (state == MovementState.wallrunnng)
            {
                randomNumber = UnityEngine.Random.Range(1, 7);
                switch (randomNumber)
                {
                    case 1:
                        playerAudioSource.PlayOneShot(running01);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                    case 2:
                        playerAudioSource.PlayOneShot(running02);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                    case 3:
                        playerAudioSource.PlayOneShot(running03);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                    case 4:
                        playerAudioSource.PlayOneShot(running04);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                    case 5:
                        playerAudioSource.PlayOneShot(running05);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                    case 6:
                        playerAudioSource.PlayOneShot(running06);
                        footstepTimer = wallrunningStepSpeed;
                        break;
                }
            } 


        }
        
    }

    public void StateHandler()
    {
        if (dashing)
        {
            fallingCount = 0f;
            state = MovementState.dashing;
        }

        else if (freeze)
        {
            fallingCount = 0f;
            state = MovementState.freeze;
            moveSpeed = 0f;
            rb.velocity = Vector3.zero;
        }
        else if (unlimited)
        {
            fallingCount = 0f;
            state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        }
        else if (wallrunning)
        {
            fallingCount = 0f;
            state = MovementState.wallrunnng;
            moveSpeed = wallRunSpeed;
        }
        
        else if (sliding)
        {
            fallingCount = 0f;
            state = MovementState.sliding;
            playerCam.DOFieldOfView(fovValue + 20f, 0.25f);
        }

        else if (isCrouching)
        {
            fallingCount = 0f;
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            fallingCount = 0f;
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
           
            playerCam.DOFieldOfView(fovValue + 10f, 0.25f);


            
        }
        // Walking
        else if (grounded && !Input.GetKey(crouchKey))
        {
            fallingCount = 0f;
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            
            playerCam.DOFieldOfView(fovValue, 0.25f);
        }
        else
        {
            fallingCount += Time.deltaTime;
            state = MovementState.air;
            
        }

    }
    
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void StartSlide()
    {
        sliding = true;

        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        playerAudioSource.PlayOneShot(slideSound);

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(!OnSlope() || rb.velocity.y > -0.1f) 
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

     

        if (slideTimer <= 0 && grounded)
        {
            StopSlide();
        }
    }

    private void StopSlide()
    {
        sliding = false;
        playerAudioSource.PlayOneShot(emptySound);
    }

    //Calculos que são necessarios para o grappling hook mas que sinceramente nao compreendo :) mas funcionam :)))
    
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x,0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private bool enableMovementOnNextTouch;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);

        playerAudioSource.PlayOneShot(grapplingPulling);

        Invoke(nameof(SetVelocity), 0.1f);
        Invoke(nameof(ResetRestrictions), 5f);

    }

    private Vector3 velocityToSet;

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrapple();
        }
    }

    private void ResetRestrictions()
    {
        activeGrapple = false;
    }

    

    public void JumpingSound()
    {
        randomNumber = UnityEngine.Random.Range(1, 6);
        switch (randomNumber)
        {
            case 1:
                playerAudioSource.PlayOneShot(walking01);
                break;
            case 2:
                playerAudioSource.PlayOneShot(walking02);
                break;
            case 3:
                playerAudioSource.PlayOneShot(walking03);

                break;
            case 4:
                playerAudioSource.PlayOneShot(walking04);

                break;
            case 5:
                playerAudioSource.PlayOneShot(walking05);

                break;
        }
    }

}
