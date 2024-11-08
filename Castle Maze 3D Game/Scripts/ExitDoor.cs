using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour
{

    public BoxCollider doorArea;

    private bool entered = false;


    void Update()
    {
        if(entered == true && Input.GetKeyDown(KeyCode.E))
        {
            entered = false;
            GameObject.Find("GameManager").GetComponent<GameManager>().EndGame();
        }
    }


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            entered = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            entered = false;
        }
    }
}
