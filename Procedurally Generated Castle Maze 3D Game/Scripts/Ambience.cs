using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{

    public Transform player;
    private bool soundPlaying = false;
    public AudioSource[] sounds;
    [SerializeField] private int chance = 300;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("RandomSounds", 15f, 1f);
    }


    private void RandomSounds()
    {
        int num = Random.Range(0,chance);
        int randomSound = Random.Range(0,sounds.Length);
        if(num == 0 && !soundPlaying)
        {
            //Teleport audio to random place around player
            TeleportToPlayer();

            soundPlaying = true;
            sounds[randomSound].Play();
            StartCoroutine(ResetPlaying(randomSound));
        }
    }

    private IEnumerator ResetPlaying(int randomSound)
    {
        yield return new WaitForSeconds(sounds[randomSound].clip.length);
        soundPlaying = false;
    }

    private void TeleportToPlayer()
    {
        //Random position
        float teleportRadius = 15f;
        Vector2 randomOffset = Random.insideUnitCircle * teleportRadius;
        transform.position = new Vector3(player.position.x + randomOffset.x, player.position.y, player.position.z + randomOffset.y);
    }
}
