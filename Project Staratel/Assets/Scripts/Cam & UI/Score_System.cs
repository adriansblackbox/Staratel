using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_System : MonoBehaviour
{
    public Text score;
    public Text multipliyer;
    public float points = 0;
    public float pointMultipliyer;
    public bool enemyInFlight;
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
            multipliyer.text = "";
        }else{
            score.text = points.ToString();
            multipliyer.text = "x" +  pointMultipliyer.ToString();
        }
        if(FindObjectOfType<Player_Controller>()._isGrounded){
            pointMultipliyer = 1f;
        }
    }
    public void AddMultiplyer(){
        if(pointMultipliyer < 5) pointMultipliyer++;
    }
}
