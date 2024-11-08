using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private GameObject player;

    private PlayerMoney playerMoney;
    
    public int valueLow, valueHigh;
    private int value;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player");
        playerMoney = player.GetComponent<PlayerMoney>();

        value = Random.Range(valueLow, valueHigh);
    }


    public void pickupItem()
    {
        playerMoney.AddMoney(value);
        Destroy(this.gameObject);
    }

    public int GetValue()
    {
        return value;
    }

}
