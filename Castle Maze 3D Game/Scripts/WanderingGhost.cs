using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    Wandering,
    Waiting,
    Following,
    Lost,
}

public class WanderingGhost : Enemy
{
    [Header ("Wandering movement")]
    public float intervalTime = 1f;
    private int currentJ;
    private int currentI;
    private int[,] layoutMatrix;
    private int matrixSize;
    private Vector3 newPos;


    [Header ("Respawn Variables")]
    private Vector3 spawnpoint;
    private int spawnJ;
    private int spawnI;
    private float despawnTime = 30f;


    public GameObject DungeonLayout;
    private State state;


    void Awake()
    {
        DungeonLayout = GameObject.Find("Dungeon Rooms");
        layoutMatrix = DungeonLayout.GetComponent<DungeonLayout>().GetLayoutMatrix();
        matrixSize = layoutMatrix.GetLength(0);

        state = State.Wandering;
    }



    protected override void Update()
    {
        base.Update();

        if(!GameManager.gameEnded)
        {
            if(state == State.Wandering)
            {
                rbody.MovePosition(Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.fixedDeltaTime));

                if(transform.position == newPos)
                {
                    state = State.Waiting;
                    Wander();
                }

            } else if(state == State.Lost)
            {
                //Do random movements to try find the player
                rbody.MovePosition(Vector3.MoveTowards(transform.position, newPos, moveSpeed * Time.fixedDeltaTime));

                if(transform.position == newPos)
                {
                    state = State.Waiting;
                    RandomMovement();
                }
            }

            CheckForPlayer();
        }
    }


    //Initial wander function setup spawnpoint etc
    public void Wander(int a, int b, Vector3 c)
    {
        //Assign spawn variable index
        spawnJ = a;
        spawnI = b;

        //Assign current index variable
        currentJ = a;
        currentI = b;

        spawnpoint = c;
        Wander();
    }



    
    public void Wander()
    {
        Vector3 direction = new Vector3(0,0,0);

        //Give algorithm limited attempts to find available movement else terminate
        int attempts = 8;
        bool found = false;

        for(int count = 0; count < attempts; count++)
        {
            //Find next available room and move to it, checking that it is valid each time
            int randomDirection = Random.Range(0,4);
            if(randomDirection == 0 && validIndex(currentJ, currentI+1))
            {
                currentI ++;
                found = true;
                direction = new Vector3(0,0,12f);
            } else if(randomDirection == 1 && validIndex(currentJ, currentI-1))
            {
                currentI --;
                found = true;
                direction = new Vector3(0,0,-12f);
            } else if(randomDirection == 2 && validIndex(currentJ+1, currentI))
            {
                currentJ ++;
                found = true;
                direction = new Vector3(12f,0,0);
            } else if(randomDirection == 3 && validIndex(currentJ-1, currentI))
            {
                currentJ --;
                found = true;
                direction = new Vector3(-12f,0,0);
            }

            //If direction found then terminate loop
            if(found)
            {
                count = attempts;
            }
        }

        newPos = transform.position + direction;

        //Allow movement after interval time
        StartCoroutine(ResetTimer());
    }


    private bool validIndex(int j, int i) 
    {
        //Checks if index is in bounds
        if((i < matrixSize - 1 && i > 0) && (j < matrixSize - 1 && j > 0))
        {
            //Checks if index contains a room
            if(layoutMatrix[j,i] == 1)
            {
                return true;

            } else {
                return false;
            }

        } else {
            return false;
        }
    }


    private IEnumerator ResetTimer()
    {
        //Creates a pause between movements based on intervalTime
        yield return new WaitForSeconds(intervalTime);
        
        state = State.Wandering;
        transform.LookAt(newPos);
    }

    private void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //If player is within range ghost will move towards it
        if(PlayerInAttackRange())
        {
            //Play heartbeat sound when following player
            if(state != State.Following)
            {
                player.GetComponent<PlayerHealth>().PlayHeartBeat();
            }


            //Stop wandering movement
            StopCoroutine(ResetTimer());
            state = State.Following;



            //Move Towards player
            float speed = moveSpeed * 1.8f;
            transform.LookAt(player.transform.position);
            rbody.MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime));

            //Stop despawn timer
            StopCoroutine(despawnTimer());

        //Only runs this if ghost is not wandering around and not near the player
        } else if(state == State.Following && distanceToPlayer > moveRange) {

            state = State.Waiting;
            StartCoroutine(ResetRandomTimer());

            //Stop heartbeat sound when not near player
            player.GetComponent<PlayerHealth>().StopHeartBeat();

            StartCoroutine(despawnTimer());
        }
    }


    private IEnumerator despawnTimer()
    {
        yield return new WaitForSeconds(despawnTime);
        while(state == State.Lost)
        {
            if(canDespawn())
            {
                //Teleport back to spawnpoint and update index variables
                transform.position = spawnpoint;
                currentJ = spawnJ;
                currentI = spawnI;
                state = State.Wandering;
            }
            yield return new WaitForSeconds(3f);
        }
    }

    private bool canDespawn()
    {
        //Check that player is not too close to the spawnpoint else it cannot spawn
        float distanceToSpawnpoint = Vector3.Distance(spawnpoint, player.transform.position);
        if(distanceToSpawnpoint > 25f)
        {
            return true;
        } else {
            return false;
        }
    }


    private void RandomMovement()
    {
        int num = Random.Range(0,4);

        Vector3 direction = new Vector3(0,0,0);

        if(num == 0)
        {
            direction = new Vector3(0,0,12f);

        } else if(num == 1)
        {
            direction = new Vector3(0,0,-12f);

        } else if(num == 2)
        {
            direction = new Vector3(12f,0,0);

        } else if(num == 3)
        {
            direction = new Vector3(-12f,0,0);

        }

        newPos = transform.position + direction;

        StartCoroutine(ResetRandomTimer());
    }

    private IEnumerator ResetRandomTimer()
    {
        //Creates a pause between movements based on intervalTime
        yield return new WaitForSeconds(intervalTime);
        
        state = State.Lost;
        transform.LookAt(newPos);
    }

}
