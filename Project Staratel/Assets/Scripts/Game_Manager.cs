using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{

    public GameObject mainCamera;
    public Transform gameView;
    public Transform menuView;
    public Transform instructionsView;
    public GameObject startButton;
    public GameObject restartButton;
    public GameObject mainMenuBotton;
    public GameObject instructionsButton;
    public GameObject planetLife;
    public GameObject pauseUI;
    public bool gameRunning;
    public bool gameOver;
    public bool restartGame;
    public float playerLives;
    public GameObject player;
    public bool gamePaused;

    private void Start() {
        gameRunning = false;
        gameOver = false;
        restartGame = false;
        restartButton.SetActive(false);
        planetLife.SetActive(false);
        pauseUI.SetActive(false);
        // Starts game with full lives
        playerLives = 3;
        player.GetComponent<Player_Controller>().enabled = false;
    }
    private void Update() {

        // Camera moving is handled here depending on the game state
        if(mainCamera.transform.position != gameView.position && gameRunning){
            moveCameraDown();
        }else if(restartGame){
            moveCameraUp();
        }
        // Reset to the beginning of the game
        if(restartGame && mainCamera.transform.position.y >= menuView.position.y - 1){
            restartGame = false;
            startButton.SetActive(true);
            instructionsButton.SetActive(true);
        }
        // once player runs out of livesm, game over is initiated
        if(gameRunning){
            if(FindObjectOfType<PlanetHealth>().Health <= 0){
                FindObjectOfType<Audio_Manager>().PlaySound("player death"); 
                GameOver();
            }
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(gamePaused){
                    FindObjectOfType<Audio_Manager>().musicSource.Play();
                    Time.timeScale = 1f;
                    gamePaused = false;
                    restartButton.SetActive(false);
                }else{
                    FindObjectOfType<Audio_Manager>().musicSource.Pause();
                    Time.timeScale = 0f;
                    gamePaused = true;
                    restartButton.SetActive(true);
                }
            }
        }
    }

    // When the start button is pressed:
    public void StartGame(){
        GetComponent<AudioSource>().Play();
        FindObjectOfType<Audio_Manager>().musicSource.Play();
        player.GetComponent<Player_Controller>().enabled = true;
        startButton.SetActive(false);
        instructionsButton.SetActive(false);
        planetLife.SetActive(true);
        pauseUI.SetActive(true);
        gameRunning = true;
    }

    // When the player dies:
    public void GameOver(){
        pauseUI.SetActive(false);
        FindObjectOfType<Audio_Manager>().musicSource.Stop();
        FindObjectOfType<PlanetHealth>().Health = 0f;
        restartButton.SetActive(true);
        gameOver = true;
        gameRunning = false;
        player.GetComponent<Player_Controller>().enabled = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GameObject[] _allEnemy01 = GameObject.FindGameObjectsWithTag("Enemy_01");
        foreach(GameObject _enemy01 in _allEnemy01){
            _enemy01.GetComponent<Enemy_01>().enabled = false;
            _enemy01.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        GameObject[] _allEnemy02 = GameObject.FindGameObjectsWithTag("Enemy_02");
        foreach(GameObject _enemy02 in _allEnemy02){
            _enemy02.GetComponent<Enemy_02>().enabled = false;
        }
    }

    // When the player clicks restart
    public void RestartGame(){
        GetComponent<AudioSource>().Play();
        FindObjectOfType<PlanetHealth>().Health = 100;
        planetLife.SetActive(false);
        restartGame = true;
        gameOver = false;
        restartButton.SetActive(false);
        FindObjectOfType<Score_System>().points = 0;
        FindObjectOfType<Score_System>().pointMultipliyer = 0;
        // resets player's lives
        playerLives = 3;
        player.transform.position = new Vector2(0.0f, -4.5f);
        GameObject[] _allEnemy01 = GameObject.FindGameObjectsWithTag("Enemy_01");
        foreach(GameObject _enemy01 in _allEnemy01){
            Destroy(_enemy01);
        }
        GameObject[] _allEnemy02 = GameObject.FindGameObjectsWithTag("Enemy_02");
        foreach(GameObject _enemy02 in _allEnemy02){
            Destroy(_enemy02);
        }
        if(gamePaused){
            Time.timeScale = 1f;
            gamePaused = false;
            gameRunning = false;
            FindObjectOfType<Audio_Manager>().musicSource.Stop();
            player.GetComponent<Player_Controller>().enabled = false;
        }
    }

    // functions for handeling camera movement
    public void moveCameraDown(){
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, gameView.position, 3f * Time.deltaTime);
    }
     public void moveCameraUp(){
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, menuView.position, 6f * Time.deltaTime);
    }
    public void moveCameraRight(){
        GetComponent<AudioSource>().Play();
        mainCamera.transform.position = instructionsView.position;
        instructionsButton.SetActive(false);
        mainMenuBotton.SetActive(true);
        startButton.SetActive(false);
    }
    public void moveCameraRightToLeft(){
        GetComponent<AudioSource>().Play();
        mainCamera.transform.position = menuView.position;
        instructionsButton.SetActive(true);
        mainMenuBotton.SetActive(false);
        startButton.SetActive(true);
    }
}