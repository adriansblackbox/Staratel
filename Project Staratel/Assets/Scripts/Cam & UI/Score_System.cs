using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_System : MonoBehaviour
{
    public Text score;
    public float points = 0;
    public float pointMultipliyer;
    public bool enemyInFlight;
    public float scorePitch;
    private float pitchTimer;
    // Start is called before the first frame update
    void Start()
    {
        pointMultipliyer = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(!FindObjectOfType<Game_Manager>().gameRunning){
            score.text = "";
        }else{
            score.text = points.ToString();
        }
        scorePitch = Mathf.Clamp(scorePitch, 1f, 2f);
        if(pitchTimer > 0f){
            pitchTimer -= Time.deltaTime;
        }else{
            scorePitch = 1f;
        }
    }
    public void AddPoints(int addPoints){
        GetComponent<AudioSource>().pitch = scorePitch;
        GetComponent<AudioSource>().Play();
        points += addPoints;
        pitchTimer = 1f;
        scorePitch += 0.1f;
    }
}
