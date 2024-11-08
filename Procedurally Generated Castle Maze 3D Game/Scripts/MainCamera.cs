using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public float sensitivity = 100f;


    public Transform player;

    public LayerMask itemMask;
    public AudioSource pickupSound;

    float xRotation;


    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        //Only fire raycast if item is nearby so they aren't being continually fired
        if(CheckCloseItem())
        {
            CheckLookingAtItem();
        }
            
        if(!GameManager.gameEnded)
        {
            MoveCamera();
        }
    }


    private void MoveCamera()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        player.Rotate(Vector3.up * mouseX);
    }




    private bool CheckCloseItem()
    {
        return Physics.CheckSphere(transform.position, 4f, itemMask);
    }


    private void CheckLookingAtItem()
    {
        RaycastHit hit;
        //Fire raycast
        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2.5f, itemMask))
        {
            //If raycast hits an object allow it to be picked up
            if(Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.gameObject.GetComponent<Collectable>().pickupItem();
                pickupSound.Play();
            }
        }
    }
}
