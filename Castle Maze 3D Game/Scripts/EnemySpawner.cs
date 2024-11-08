using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header ("Objects")]
    public GameObject WanderingGhost;
    public GameObject DungeonRooms;
    public GameObject player;

    private int GhostCount = 0;

    private int DayCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("GhostSpawn", 15f, 1f);
    }

    private void GhostSpawn()
    {
        //Exponential function to reduce chance of spawning later on
        int maxValue = (int) Mathf.Floor(Mathf.Pow(3,(GhostCount*2)) / DayCount) + 20;
        int num = Random.Range(0, maxValue);

        if(num == 7)
        {  
            int[] coords = RandomSpawnpoint();
            int j = coords[0];
            int i = coords[1];
            Vector3 spawnpoint = new Vector3(j*12f ,1f, i*12f + 12f);


            StartCoroutine(SpawnGhost(spawnpoint, j, i));
        }
    }

    private IEnumerator SpawnGhost(Vector3 spawnpoint, int j, int i)
    {
        bool spawned = false;
        while(!spawned)
        {
            //Check if player is too close to spawnpoint
            if(CanSpawn(spawnpoint))
            {
                GhostCount ++;
                spawned = true;
                GameObject ghost = Instantiate(WanderingGhost, spawnpoint, WanderingGhost.transform.rotation, this.gameObject.transform);
                ghost.GetComponent<WanderingGhost>().Wander(j,i,spawnpoint);

            } else {
                yield return new WaitForSeconds(0.5f);
            }
        }
       
    }

    
    private bool CanSpawn(Vector3 coords)
    {
        float distanceToPlayer = Vector3.Distance(coords, player.transform.position);

        if(distanceToPlayer > 10f)
        {
            return true;

        } else {
            return false;
        }
    }


    private int[] RandomSpawnpoint()
    {
        //Choose random spawn locations until suitable one is found
        int[,] matrix = DungeonRooms.GetComponent<DungeonLayout>().GetLayoutMatrix();
        int size = matrix.GetLength(0);

        bool found = false;
        while(!found)
        {
            int randJ = Random.Range(1,size);
            int randI = Random.Range(1,size);

            if(matrix[randJ,randI] == 1)
            {
                found = true;
                return new int[2] {randJ,randI};
            }
        }

        return null;
    }


    public void ResetEnemies()
    {
        CancelInvoke();
        DayCount++;

        int ChildCount = transform.childCount;

        for(int i = 0; i < ChildCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        InvokeRepeating("GhostSpawn", 15f, 1f);
    }
}
