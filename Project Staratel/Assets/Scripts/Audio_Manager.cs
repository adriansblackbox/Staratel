using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    public AudioClip playerDash, playerStep, playerHurt, enemyHit, playerHit, wallHit, playerDeath, dashReplenish;
    public AudioSource audioSourceStatic;
    public AudioSource audioSourceDynamic;
    public AudioSource musicSource;
    private void Update(){
    }
    public void PlaySound(string clip){
        switch(clip){
            case "enemy hit":
                audioSourceStatic.pitch = Random.Range(1f, 1.1f);
                audioSourceStatic.PlayOneShot(playerHit);
                audioSourceStatic.PlayOneShot(enemyHit);
            break;
            case "dash":
                audioSourceDynamic.pitch = Random.Range(1f, 1.3f);
                audioSourceDynamic.PlayOneShot(playerDash);
            break;
            case "step":
                audioSourceDynamic.pitch = Random.Range(1f, 3f);
                audioSourceDynamic.PlayOneShot(playerStep);
            break;
            case "player hurt":
                audioSourceStatic.pitch = 1f;
                audioSourceStatic.PlayOneShot(playerHurt);
            break;
            case "wall hit":
                audioSourceStatic.PlayOneShot(wallHit);
            break;
            case "player death":
                audioSourceStatic.pitch = 1f;
                audioSourceStatic.PlayOneShot(playerDeath);
            break;
            case "dash replenish":
                audioSourceStatic.pitch = 1f;
                audioSourceStatic.PlayOneShot(dashReplenish);
            break;
        }
    }
}
