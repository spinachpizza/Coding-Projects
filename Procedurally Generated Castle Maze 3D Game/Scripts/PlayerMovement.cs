using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Default,
    Walking,
    Sprinting,
    Crouched,
}

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement")]
    public float moveSpeed = 12f;
    public AudioSource footsteps;
    private PlayerState state;

    [Header ("HeadBob")]
    [SerializeField] private float walkBobSpeed = 12f;
    [SerializeField] private float walkBobAmount = 0.4f;
    [SerializeField] private float sprintBobSpeed = 16f;
    [SerializeField] private float sprintBobAmount = 0.8f;
    private float initialYpos;
    private float timer;

    [Header ("Stamina")]
    [SerializeField] private float maxStamina = 3f;
    private float currentStamina;
    [SerializeField] private float decayRate = 0.5f;
    [SerializeField] private float regainRate = 0.2f;
    private bool exhausted = false;



    private readonly Vector3 spawnpoint = new Vector3(7f, 1.2f, 300f);


    private Vector3 moveDirection;
    private CharacterController controller;
    private Camera playerCamera;


    [Header ("Objects")]
    public GameObject UI;
    private GameUI gameUI;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        gameUI = UI.GetComponent<GameUI>();

        initialYpos = playerCamera.transform.localPosition.y;
        currentStamina = maxStamina;
    }


    
    void FixedUpdate()
    {
        float speed;
        //Reduce speed if player is in the air
        if(GetComponent<PlayerGravity>().isGrounded())
        {
            speed = moveSpeed;

        } else {
            speed = moveSpeed / 1.5f;
        }
        

        if(!GameManager.gameEnded)
        {
            Movement(speed);

            HandleHeadBob();

            AdjustStamina();

            PlayFootsteps();  
        }
    }


    private void Movement(float speed)
    {
        //Inputs
        float x  = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        state = PlayerState.Walking;

        //Reduced diagonal movement
        if(x != 0 && z != 0)
        {
            speed = moveSpeed * 0.75f;
            
        } else {

            speed = moveSpeed;
        }

        //Sprinting only allowed if on ground
        if(Input.GetKey(KeyCode.LeftShift) && GetComponent<PlayerGravity>().isGrounded() && !exhausted)
        {
            speed = moveSpeed * 1.5f;
            state = PlayerState.Sprinting;

        }


    
        if(state == PlayerState.Sprinting)
        {
            //Sideways movement restricted while sprinting
            if(z > 0)
            {
                moveDirection = transform.right * x * 0.4f + transform.forward * Mathf.Abs(z);

            //Backwards movement slowed while sprinting
            } else {

                moveDirection = transform.forward * 0.4f;
            }

        } else if(state == PlayerState.Walking)
        {
            moveDirection = transform.right * x + transform.forward * z;
        }


        //If player is in the air, sideways movement is restricted
        if(!GetComponent<PlayerGravity>().isGrounded())
        {
            moveDirection = transform.right * x * 0.3f + transform.forward * z;
        }

        //Move player
        controller.Move(moveDirection * speed * Time.deltaTime);
    }

    private void HandleHeadBob()
    {
        if(!GetComponent<PlayerGravity>().isGrounded()) return;

        float targetYpos = initialYpos;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            float amount = 0f;
            //Head bob amount changes if walking or sprinting
            if(state == PlayerState.Sprinting)
            {
                timer += Time.deltaTime * sprintBobSpeed;
                amount = sprintBobAmount;

            } else if(state == PlayerState.Walking)
            {
                timer += Time.deltaTime * walkBobSpeed;
                amount = walkBobAmount;
            }
            
            targetYpos = initialYpos + Mathf.Sin(timer) * amount;

        } 

        playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            targetYpos,
            playerCamera.transform.localPosition.z
        );
    }

    private void AdjustStamina()
    {
        if(state == PlayerState.Sprinting)
        {
            currentStamina -= Time.deltaTime * decayRate;
            if(currentStamina <= 0)
            {
                exhausted = true;
                StartCoroutine(ExhaustedTimer());
            }

        } else if(currentStamina < maxStamina && !exhausted)
        {
            currentStamina += Time.deltaTime * regainRate;
            if(currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
        }

        if(currentStamina > 0 && currentStamina < maxStamina)
        {
            gameUI.UpdateStaminaBar(currentStamina);
        }
    }


    private IEnumerator ExhaustedTimer()
    {
        yield return new WaitForSeconds(2f);
        exhausted = false;
    }


    private void PlayFootsteps()
    {
        FootstepPitch();
        //Check player is moving and on the ground
        if(controller.velocity.magnitude > 0f && GetComponent<PlayerGravity>().isGrounded())
        {
            if(!footsteps.isPlaying)
            {
                footsteps.Play();
                footsteps.volume = 0.4f;
            }

        } else if(footsteps.isPlaying){
            footsteps.volume = 0f;
            footsteps.Stop();
        }
    }


    private void FootstepPitch()
    {
        if(state == PlayerState.Sprinting)
        {
            footsteps.pitch = 1f;
        } else {
            footsteps.pitch = 0.75f;
        }
    }

    public void ResetPosition()
    {
        controller.enabled = false;
        transform.position = spawnpoint;
        transform.rotation = Quaternion.Euler(0f,90f,0f);
        controller.enabled = true;
    }
}
