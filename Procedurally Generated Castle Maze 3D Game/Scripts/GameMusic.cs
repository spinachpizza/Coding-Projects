using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMusic : MonoBehaviour
{
    public AudioSource Music;

    private bool playing = false;

    void Start() 
    {
        InvokeRepeating("RandomlyPlay", 5f, 1f);
    }


    private void RandomlyPlay()
    {
        int randomnum = Random.Range(0,60);
        if(playing == false && randomnum == 0)
        {
            PlayMusic();
        }
    }


    public void PlayMusic()
    {
        Music.Play();
        playing = true;

        StartCoroutine(WaitTillFinished());
    }

    private IEnumerator WaitTillFinished()
    {
        float musicLength = Music.clip.length;
        yield return new WaitForSeconds(musicLength);
        playing = false;
    }
}
