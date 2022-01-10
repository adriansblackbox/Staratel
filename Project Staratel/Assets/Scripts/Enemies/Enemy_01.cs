using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_01 : MonoBehaviour
{
    // Start is called before the first frame update
    private bool inRangeOfAttack = false;
    private bool isGrounded = false;
    private Rigidbody2D enemyRB;
    private Vector3 target;
    private Vector3 difference;
    private GameObject mainCamera;
    private Game_Manager _gameManager;
    private GameObject player;
    private float timesBounced;
    private Rigidbody2D playerRB;
    private Enemy_Spawner enemySpawner;
    private bool _fallingMode;
    public bool _groundMode;
    private bool _projectileMode;
    private bool _targetLocked;


    public float projectileSpeed;
    private float fallingSpeed;
    public float landingTime;
    public float SuckRate = 2f;
    public Transform groundChecker;
    public float groundCheckerRadius;
    public LayerMask ground;
    public Sprite vulnerableSkin;
    public Sprite fallingSkin;
    public Sprite groundSkin;
    void Start()
    {
        enemySpawner = FindObjectOfType<Enemy_Spawner>();
        enemyRB = GetComponent<Rigidbody2D>();
        _gameManager = FindObjectOfType<Game_Manager>();
        mainCamera = GameObject.FindWithTag("MainCamera");
        player = GameObject.FindWithTag("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        // These booleans are used to determine which state the enemy is in
        _fallingMode = true;
        _projectileMode = false;
        _groundMode = false;
        _targetLocked = false;
        GetComponent<TrailRenderer>().enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fallingSpeed = enemySpawner.fallingSpeed;
        if(_fallingMode) fallingMode();
        else if(!_projectileMode){
            groundMode();
        }else
            transform.up = Vector3.Lerp(transform.up, -1f * GetComponent<Rigidbody2D>().velocity, Time.deltaTime * 5f);
        // incharge of destroying enemy if bounced more that 2 times
        if(timesBounced > 1)
            enemyDeath(true);
    }
    private void Update() {
        //===================================================================================================
        // adjusting the target that the player will shoot at. If grounded, the player can aim their shot
        // if in an areal state, the player will shoot at grounded enemies. If there are no grounded enemies,
        // the player wil shoot straight down
        //===================================================================================================
        if(player.GetComponent<Player_Controller>()._isGrounded){ target = 
            mainCamera.transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            transform.position.z));
        }else if(enemySpawner.targetEnemy.Count > 0){ 
            target = enemySpawner.targetEnemy[0].transform.position;
        }
        //===================================================================================================
        if(inRangeOfAttack && _fallingMode)
            InitiateAttack();
        else if(!isGrounded)
            this.gameObject.GetComponent<SpriteRenderer>().sprite = fallingSkin;
    }
    private void fallingMode(){
        //Constant Falling speed of editors choise
        enemyRB.velocity = new Vector2(0, -1*fallingSpeed);
        groundCheck();
        if(isGrounded) _fallingMode = false;
    }
    private void groundMode(){
        enemyRB.velocity = Vector2.zero;
        this.GetComponent<Parallax>().enabled = true;
        this.GetComponent<Parallax>().yOrigin = this.transform.position.y;
        // handles when ground behavior should be active (has to wait for the amount of landing time to move)
        if(landingTime <= 0){
            _groundMode = true;
            this.gameObject.GetComponent<SpriteRenderer>().sprite = groundSkin;
            FindObjectOfType<PlanetHealth>().Health -= Time.deltaTime * SuckRate;
        }else{
            landingTime -= Time.deltaTime;
        }
    }

    //Checking is enemy has landed
    private void groundCheck(){
        isGrounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckerRadius, ground);
    }
    //This function is called when the player has the option to "kick" this enemy
    private void InitiateAttack(){
        FindObjectOfType<Audio_Manager>().PlaySound("enemy hit");
        FindObjectOfType<Audio_Manager>().PlaySound("dash replenish");
        enemySpawner.burstEffect(transform.position, Quaternion.identity, "little burst");
        difference = target - transform.position;
        float distance = difference.magnitude;
        Vector2 direction = difference/distance;
        player.GetComponent<Player_Controller>()._speedBoostTime = 0f;

        GetComponent<TrailRenderer>().enabled = true;
        //Replenishes player's jump
        if(!player.GetComponent<Player_Controller>()._isGrounded){
            _targetLocked = true;
            FindObjectOfType<Score_System>().AddMultiplyer();
            player.GetComponent<Player_Controller>()._canJump = true;
            player.GetComponent<Player_Controller>()._resetVelocity = true;
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(
                player.GetComponent<Rigidbody2D>().velocity.x,
                12f);
        }
        _projectileMode = true;
        _fallingMode = false;
        //adjusts tge score system to start the multiplyer
        FindObjectOfType<Score_System>().enemyInFlight = true;
        //the enemy becomes a projectile
        transform.gameObject.tag = "Projectile";
        //If the player is on the ground, or if there is a target on the ground, the enemies are aimed at the player's cursor
        if(player.GetComponent<Player_Controller>()._isGrounded ||enemySpawner.targetEnemy.Count > 0){
            direction.Normalize();
            enemyRB.velocity = direction * projectileSpeed;
        //If not, the player hits the enemies straight down
        }else
            enemyRB.velocity = new Vector3(0f, -1 * projectileSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Hit Box"))
            inRangeOfAttack = true;
        if(other.CompareTag("Ground") && !_projectileMode) enemySpawner.targetEnemy.Add(this.gameObject);
        if(!_groundMode){
            if(other.gameObject.CompareTag("BoundsDeath")) enemyDeath(false);
            if(other.gameObject.CompareTag("Ground") && _projectileMode){
                FindObjectOfType<Camera_Shake>().shakeCamera(.095f, .2f);
                enemyDeath(true); 
                FindObjectOfType<PlanetHealth>().Health += 2f;
            }
            if(other.gameObject.CompareTag("Projectile") && !this.gameObject.CompareTag("Projectile")) enemyDeath(true);
        }else{
            if(other.gameObject.CompareTag("Projectile") &&  other.GetComponent<Enemy_01>()._targetLocked){
                enemyDeath(true);
            }else if(other.gameObject.CompareTag("Projectile")){
                other.gameObject.GetComponent<Enemy_01>().enemyDeath(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
         if(other.gameObject.CompareTag("Hit Box")){
            inRangeOfAttack = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
          if(other.gameObject.CompareTag("Bouncy Walls") && !isGrounded){
            FindObjectOfType<Camera_Shake>().shakeCamera(.095f, .2f);
            enemySpawner.burstEffect(transform.position, Quaternion.identity, "little burst");
            FindObjectOfType<Audio_Manager>().PlaySound("wall hit");
            timesBounced++;
        }
    }

    public void enemyDeath(bool Burst){
        if(this.gameObject.CompareTag("Projectile")){
            FindObjectOfType<Score_System>().enemyInFlight = false;
        }
        if(Burst) enemySpawner.burstEffect(transform.position, Quaternion.identity, "big burst");
        FindObjectOfType<Score_System>().points += FindObjectOfType<Score_System>().pointMultipliyer;
        enemySpawner.targetEnemy.Remove(this.gameObject);
        Destroy(this.gameObject);
    }
}
