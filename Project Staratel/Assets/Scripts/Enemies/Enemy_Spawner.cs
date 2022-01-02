using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public GameObject enemy01;
    public GameObject enemy02;
    public GameObject[] spawnPoints;
    public Transform mainCamera;
    public Transform gameView;
    public GameObject burst;
    public GameObject littleBurst;
    public float defaultTimeBetweenSpawn;
    public float defaultfallingSpeed;
    public float fallingSpeed;
    public List<GameObject> targetEnemy;
    public List<int> flyingSpaces;
    private float timeBetweenSpawn;
    public int randSpawnPoint;
    private float spawnTimer = 0;
    private float numOfEnemiesSpawned;
    private float numOfEnemiesLimmit = 25;
    private float secondWaited;
    // Start is called before the first frame update
    void Start()
    {
        numOfEnemiesSpawned = 0f;
        timeBetweenSpawn = defaultTimeBetweenSpawn;
        fallingSpeed = defaultfallingSpeed;
        secondWaited = 0f;
        if(targetEnemy.Count > 0) targetEnemy.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // Difficulty over time is handeled here
        if(FindObjectOfType<Game_Manager>().gameRunning){
            secondWaited += Time.deltaTime;
            if(secondWaited >= 1f){
                // each second until time between spawns is .75 of a second,
                // it will be subtracted by .05 of a second.
                if(timeBetweenSpawn > 0.5)
                    timeBetweenSpawn -= 0.01f;

                if(fallingSpeed < 7f)
                    fallingSpeed += 0.001f;
                else
                    Debug.Log("Full speed");
                secondWaited = 0;
            }
            spawnEnemy_01();
            if(numOfEnemiesSpawned >= numOfEnemiesLimmit)
                spawnEnemy_02();
        // resets the nubmer of enemies spawned
        }else{
            numOfEnemiesSpawned = 0;
            numOfEnemiesLimmit = 25;
        }
        if(FindObjectOfType<Game_Manager>().gameOver == true){
            Start();
        }
    }
    private void spawnEnemy_01(){
        if(spawnTimer <= 0 && mainCamera.position.y <= gameView.position.y + 1){
            randSpawnPoint = Random.Range(0, 7);
            Instantiate(enemy01, spawnPoints[randSpawnPoint].transform.position, Quaternion.identity);
            numOfEnemiesSpawned++;
            spawnTimer = timeBetweenSpawn;
        }else{
            spawnTimer -= Time.deltaTime;
        }
    }
    private void spawnEnemy_02(){
        randSpawnPoint = Random.Range(0, 7);
        if(!flyingSpaces.Contains(randSpawnPoint) && flyingSpaces.Count <= 2){
            Instantiate(enemy02, spawnPoints[randSpawnPoint].transform.position, Quaternion.identity);
            numOfEnemiesSpawned = 0;
            numOfEnemiesLimmit = Random.Range(3, 8);
        }
    }
    public void burstEffect(Vector3 pos, Quaternion identity, string burstType){
        if(burstType == "big burst")
            Instantiate(burst, pos, identity);
        if(burstType == "little burst")
            Instantiate(littleBurst, pos, identity);
    }
}
