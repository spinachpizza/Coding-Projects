using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header ("Health")]
    [SerializeField] private int MaxHealth = 3;
    private int CurrentHealth;

    [Header ("Audio")]
    public AudioSource Heartbeat;
    public AudioSource Breathing;
    private float startVolume;
    private float fadeDuration = 2f;


    [Header ("Objects")]
    public GameObject GameManager;
    public GameObject Light;


    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
        startVolume = Heartbeat.volume;

        Light.SetActive(false);
    }



    public void TakeDamage(int amount, Vector3 coords)
    {
        CurrentHealth -= amount;

        if(CurrentHealth <= 0)
        {
            GameManager.GetComponent<GameManager>().PlayerDeath();

            coords.y += 0.5f;

            transform.LookAt(coords);

            Light.SetActive(true);


        }
    }

    public void Reset()
    {
        CurrentHealth = MaxHealth;
        Light.SetActive(false);
    }


    public void AddHealth(int amount)
    {
        CurrentHealth += amount;
        if(CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }


    public void PlayHeartBeat()
    {
        Heartbeat.volume = 1;
        if(!Heartbeat.isPlaying)
        {
            Heartbeat.Play();
        }
    }

    public void StopHeartBeat()
    {
        StartCoroutine("FadeOut");
    }

    private IEnumerator FadeOut()
    {
        float currentTime = 0;
        
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            Heartbeat.volume = Mathf.Lerp(startVolume, 0, currentTime / fadeDuration);
            yield return null;
        }

        // Ensure audio volume is zero
        Heartbeat.volume = 0;
        Heartbeat.Stop();

        Breathing.Play();
    }
}
