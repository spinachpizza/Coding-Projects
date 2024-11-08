using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSpawner : MonoBehaviour
{
    public GameObject[] items = new GameObject[5];

    public GameObject Dungeon;

    private string[] incompatibleRooms = new string[4] {"11.3","12.3","13.3","14.3"};
    private string[] semicompatibleRooms = new string[16] {"1.1","2.1","3.1","4.1","5.1","6.1","11.1","11.2","12.1","12.2","13.1","13.2","14.1","14.2","14.4","15.1"};

    private int currentValue = 0;


    public void SpawnLoot(int minValue)
    {
        int ChildCount = Dungeon.transform.childCount;
        int numOfRooms = Dungeon.GetComponent<DungeonLayout>().getNumRooms();


        //Loop through every room (child of dungeon)
        for(int i = 0; i<ChildCount; i++)
        {
            Transform child = Dungeon.transform.GetChild(i);
            string name = child.name;

            //Random rotation
            float rotation = Random.Range(0,360);

            //Random item index in array
            int item = Random.Range(0,items.Length);

            //Random chance for item spawn
            int randomChance = Random.Range(0,15);

            //If room is compatible for item placement then place random item
            int level = CompatibleLevel(name);
            if(name != "EntranceRoom(Clone)" && level != 0 && randomChance == 0)
            {
                //Setup spawn position and spawn rotation
                Vector3 spawnPosition = SpawnCoords(level) + child.position;
                Vector3 rotationVector = new Vector3(0f,rotation,items[item].transform.eulerAngles.z);

                GameObject obj = Instantiate(items[item], spawnPosition, Quaternion.Euler(rotationVector), this.gameObject.transform);
                currentValue += obj.GetComponent<Collectable>().GetValue();
            }

            if(i == ChildCount - 1 && currentValue < minValue)
            {   
                i = Random.Range(0,ChildCount - 1);
            } 
        }
    }

    private int CompatibleLevel(string name)
    {
        //Compatible levels
        //0 - incompatible
        //1 - semi-compatible, only middle room cell
        //2 - fully compatible, all room cells

        //Loop through incomatible rooms array and search for it
        for(int i = 0; i < incompatibleRooms.Length; i++)
        {
            string formatNameArr = "RV" + incompatibleRooms[i] + "(Clone)";
            if(formatNameArr == name)
            {
                return 0;
            }
        }

        //Loop through semi compatible room array and search for it
        for(int i = 0; i < semicompatibleRooms.Length; i++)
        {
            string formatNameArr = "RV" + semicompatibleRooms[i] + "(Clone)";
            if(formatNameArr == name)
            {
                return 1;
            }
        }

        return 2;
    }


    private Vector3 SpawnCoords(int level)
    {
        float x = 0f;
        float z = 0f;

        //If the whole room is availble for spawning (blocks incompatible rooms)
        if(level == 2)
        {
            //Random spawn place
            x = Random.Range(-5.5f,5.5f);
            z = Random.Range(-5.5f,5.5f);

        } else if(level == 1) 
        {
            //Random spawn place
            x = Random.Range(-1.5f,1.5f);
            z = Random.Range(-1.5f,1.5f);
        }

        return new Vector3(x,0,z);
    }


    public void ResetLoot()
    {
        int ChildCount = transform.childCount;

        currentValue = 0;

        for(int i = 0; i < ChildCount - 1; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
