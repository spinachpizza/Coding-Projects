using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header ("Text Elements")]
    public TMP_Text goldText;
    public TMP_Text dayText;
    public TMP_Text quotaText;
    public TMP_Text quotaNotReachedText;

    public GameObject EndScreen;
    public GameObject LostScreen;

    public Slider staminaBar;

    public GameObject GameManager;

    void Start()
    {
        EndScreen.SetActive(false);
        LostScreen.SetActive(false);
        quotaNotReachedText.gameObject.SetActive(false);
    }


    public void UpdateGoldText(int number)
    {
        string text = "Gold: " + number.ToString();
        goldText.text = text;
    }

    public void UpdateDayText(int day, int quota)
    {
        dayText.text = "Day " + day.ToString();
        quotaText.text = "Quota: " + quota.ToString();
    }


    public void EndGameScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        EndScreen.SetActive(true);
    }

    public void DisplayQuotaNotReached()
    {
        quotaNotReachedText.gameObject.SetActive(true);
        StartCoroutine(removeQuotaDisplay());
    }

    private IEnumerator removeQuotaDisplay()
    {
        yield return new WaitForSeconds(2f);
        quotaNotReachedText.gameObject.SetActive(false);
    }


    public void UpdateStaminaBar(float stamina)
    {
        int rounded = (int) Mathf.Round(stamina * 100);
        staminaBar.value = rounded;
    }


    public void EndLostScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        LostScreen.SetActive(true);
    }


    public void HideEndScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EndScreen.SetActive(false);
    }

    public void NextGameButton()
    {
        GameManager.GetComponent<GameManager>().ResetGame();
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
