using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    [Header ("Gravity")]
    public float gravity = -9.81f;
    Vector3 velocity;

    [Header ("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    bool grounded;

    [Header ("Jump")]
    public float jumpHeight = 3f;



    CharacterController controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = CheckGrounded();
        if(grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        if(Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }


    private bool CheckGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }


    public bool isGrounded()
    {
        return grounded;
    }

}
