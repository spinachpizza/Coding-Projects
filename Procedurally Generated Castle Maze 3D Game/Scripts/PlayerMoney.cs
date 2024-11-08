using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [Header ("Money")]
    private int balance = 0;

    [Header ("Objects")]
    public GameObject GameUI;

    public void AddMoney(int amount)
    {
        balance += amount;
        GameUI.GetComponent<GameUI>().UpdateGoldText(balance);
    }

    public void DeductMoney(int amount)
    {
        balance -= amount;
        if(balance < 0)
        {
            balance = 0;
        }
        GameUI.GetComponent<GameUI>().UpdateGoldText(balance);
    }

    public void ResetBalance()
    {
        balance = 0;
        GameUI.GetComponent<GameUI>().UpdateGoldText(balance);
    }

    public int getBalance()
    {
        return balance;
    }
}
