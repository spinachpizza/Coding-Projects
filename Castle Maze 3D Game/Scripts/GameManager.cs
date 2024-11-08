using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject UI;
    public GameObject DungeonRooms;
    public GameObject DungeonItems;
    public GameObject Enemies;
    public GameObject player;
    public GameObject ghost;


    public static bool gameEnded;



    private int DayTracker = 1;
    private int Quota = 120;


    private bool paused = false;

    void Start()
    {
        unPauseGame();

        gameEnded = false;

        FirstGameSetup();
    }


    public void EndGame()
    {
        int currentMoney = player.GetComponent<PlayerMoney>().getBalance();
        if(currentMoney >= Quota)
        {
            gameEnded = true;
            PauseGame();
            UI.GetComponent<GameUI>().EndGameScreen();

        } else 
        {
            UI.GetComponent<GameUI>().DisplayQuotaNotReached();
        }
    }

    public void PlayerDeath()
    {
        gameEnded = true;
        StartCoroutine(DisplayDeadScreen());
    }

    private IEnumerator DisplayDeadScreen()
    {
        yield return new WaitForSeconds(2f);
        PauseGame();
        UI.GetComponent<GameUI>().EndLostScreen();
    }


    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void unPauseGame()
    {
        Time.timeScale = 1;
    }
    
    public bool isPaused()
    {
        return paused;
    }

    public void FirstGameSetup()
    {
        //Create layout of dungeon
        DungeonRooms.GetComponent<DungeonLayout>().SetupDungeon();

        //Spawn loot in dungeon
        int randomAmount = Random.Range(150,450);
        DungeonItems.GetComponent<LootSpawner>().SpawnLoot(Quota + randomAmount);

        //Update UI
        UI.GetComponent<GameUI>().UpdateDayText(DayTracker, Quota);

        player.GetComponent<PlayerMovement>().ResetPosition();
    }


    public void ResetGame()
    {
        //Increase day tracker and quota amount
        DayTracker ++;
        Quota = (int) Mathf.Floor(Quota * 1.3f);

        //Update UI
        UI.GetComponent<GameUI>().UpdateDayText(DayTracker, Quota);

        //Rebuild layout of dungeon to be different
        DungeonRooms.GetComponent<DungeonLayout>().ResetLayout();
        DungeonRooms.GetComponent<DungeonLayout>().SetupDungeon();

        //Remove and replace items throughout maze
        DungeonItems.GetComponent<LootSpawner>().ResetLoot();
        int randomAmount = Random.Range(150,350);
        DungeonItems.GetComponent<LootSpawner>().SpawnLoot(Quota + randomAmount);

        //Teleport player back to spawn
        player.GetComponent<PlayerMovement>().ResetPosition();

        //Reset gold
        player.GetComponent<PlayerMoney>().ResetBalance();

        //Reset health
        player.GetComponent<PlayerHealth>().Reset();

        //Close End Screen
        UI.GetComponent<GameUI>().HideEndScreen();

        //Reset enemies
        Enemies.GetComponent<EnemySpawner>().ResetEnemies();

        //Reset Ghost
        ghost.GetComponent<Ghost>().ResetGhost();

        gameEnded = false;

        unPauseGame();
    }

}
