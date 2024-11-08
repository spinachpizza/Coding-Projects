using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GhostState
{
    Following,
    Waiting,
}


public class Ghost : Enemy
{
    public AudioSource teleportSound;

    public int spawnChance = 180;

    private Vector3 spawnposition = new Vector3(-50f, 1f, 300f);

    private GhostState state; 

    protected override void Start()
    {
        base.Start();

        state = GhostState.Waiting;

        InvokeRepeating("UpdateSecond", 15f, 2.0f);
    }

    protected override void Update()
    {
        base.Update();

        if(!GameManager.gameEnded)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if(player != null)
            {
                if(PlayerInAttackRange())
                {
                    Movement();
                    LookAtPlayer();

                } else if(state == GhostState.Following && distanceToPlayer > moveRange)
                {
                    player.GetComponent<PlayerHealth>().StopHeartBeat();
                    transform.position = spawnposition;
                    state = GhostState.Waiting;
                }
            }
        }
    }


    //Updates every second
    private void UpdateSecond()
    {
        int RandomChance = Random.Range(0,spawnChance);

        if(RandomChance == 0 && state == GhostState.Waiting)
        {
            TeleportToPlayer();
        }
    }


    public void ResetGhost()
    {
        CancelInvoke();
        transform.position = spawnposition;
        InvokeRepeating("UpdateSecond", 15f, 1.0f);
    }



    private void TeleportToPlayer()
    {
        teleportSound.Play();

        //Play heartbeat sound when following player
        player.GetComponent<PlayerHealth>().PlayHeartBeat();
        
        state = GhostState.Following;

        //Random position
        float teleportDistance = 12f;
        Vector2 randomOffset = Random.insideUnitCircle.normalized * teleportDistance;
        transform.position = new Vector3(player.transform.position.x + randomOffset.x, transform.position.y, player.transform.position.z + randomOffset.y);
    }



    protected override void Movement()
    {
        Vector3 targetPos = player.transform.position;
        targetPos.y -= 0.3f;
        rbody.MovePosition(Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime));
    }
}
