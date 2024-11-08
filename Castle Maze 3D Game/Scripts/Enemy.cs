using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Attack")]
    public bool inRange = false;
    public bool attackCooldown = false;
    public int dmg = 1;


    [Header("Movement")]
    public float moveSpeed = 3f;
    public float moveRange = 10f;


    public GameObject player;

    private BoxCollider attackBox;
    public Rigidbody rbody;

    public AudioSource DeathScreech;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        player = GameObject.Find("Player");

        attackBox = GetComponent<BoxCollider>();
        rbody = GetComponent<Rigidbody>();
    }



    protected virtual void Update()
    {
        if(player != null)
        {
            if(inRange && !attackCooldown)
            {
                StartCoroutine("Attack");
            }
        }
    }


    public bool PlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceToPlayer > 0.8f && distanceToPlayer < moveRange)
        {
            return true;
        } else {
            return false;
        }
    }


    protected virtual void Movement()
    {
        Vector3 targetPos = player.transform.position;
        targetPos.y = 0f;
        rbody.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime));
    }


    protected virtual void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }


    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        if(inRange)
        {
            attackCooldown = true;
            player.GetComponent<PlayerHealth>().TakeDamage(dmg, transform.position);

            if(!DeathScreech.isPlaying)
            {
                DeathScreech.Play();
            }
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(0.7f);
        attackCooldown = false;
    }


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
