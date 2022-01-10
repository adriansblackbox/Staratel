using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_02 : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 flyToLocation;
    private float localTime;
    private bool needToFlyDown = true;
    public float reloadTimer = 2f;
    public float flyDownSpeed = 3f;
    public int flyingSpace;
    public GameObject projectile;
    public GameObject[] shootingPoints;
    public float amplitude = 0.5f;
    public float frequency = 1f;
    private Vector3 posOffset;
    private Vector3 tempPos;
    private float flyToOffset;
    private float reloadTime;

    public AudioClip shoot, dive, flap, death;
    public AudioSource audioSource;

    private void Awake() {
        flyToOffset = Random.Range(5, 9);
        flyToLocation = new Vector3(transform.position.x, transform.position.y - flyToOffset, transform.position.z);
        flyingSpace = FindObjectOfType<Enemy_Spawner>().randSpawnPoint;
        FindObjectOfType<Enemy_Spawner>().flyingSpaces.Add(flyingSpace);
        reloadTime = reloadTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= flyToLocation.y) needToFlyDown = false;
        if(needToFlyDown)getIntoPosition();
        else{
            bobUpandDown();
            shootAtPlayer();
        }
    }

    private void getIntoPosition(){
        transform.position = new Vector3(
            transform.position.x, 
            transform.position.y - flyDownSpeed*Time.deltaTime, 
            transform.position.z);
        posOffset = transform.position;
    }
    private void shootAtPlayer(){
        if(reloadTime <= 0){
            PlaySound("shoot");
            foreach(GameObject point in shootingPoints){
                Vector3 difference = GameObject.FindWithTag("PlayerTarget").transform.position - point.transform.position;
                float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                point.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
                Instantiate(projectile, point.transform.position, point.transform.rotation);
            }
            reloadTime = reloadTimer;
        }else
            reloadTime -= Time.deltaTime;
    }

    private void bobUpandDown(){
        tempPos = posOffset;
        localTime += Time.deltaTime;
        tempPos.y -= Mathf.Sin (localTime * Mathf.PI * frequency) * amplitude;
        transform.position = tempPos;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        //Enemy death by bounds or projectile
        if(other.gameObject.CompareTag("Projectile")){
            enemyDeath();
        }
    }
    private void enemyDeath(){
        FindObjectOfType<Enemy_Spawner>().flyingSpaces.Remove(flyingSpace);
        FindObjectOfType<Enemy_Spawner>().burstEffect(transform.position, Quaternion.identity, "big burst");
        Destroy(gameObject);
    }
    public void PlaySound(string clip){
        switch(clip){
            case "shoot":
                audioSource.PlayOneShot(shoot);
            break;
        }
    }
}

