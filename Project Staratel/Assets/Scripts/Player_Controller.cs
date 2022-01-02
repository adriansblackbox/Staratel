using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public bool _isGrounded;
    public bool _isJumping;
    public bool _resetVelocity;
    public bool _canJump = true;
    public bool _isInvincible = false;
    public bool _touchingBound = false;
    private float _moveInput;
    private float _lastImageXpos;
    private float _lastImageYpos;
    public float _speedBoostTime;
    public float _timeBeforeVulnerable;
    private float _timeBeofreNextStep;
    private Rigidbody2D _playerRB;
    private Material _matWhite;
    private Material _matDefault;
    private Animator _animator;
    private GameObject _mainCamera;
    private SpriteRenderer playerSR;
    private Vector2 direction;

    public float BoostTime;
    public float KnockBackForce = 20f;
    public float Speed;
    public float JumpForce;
    public float InvincibilityTime;
    public float GroundCheckerRadius;
    public float DistanceBetweenAfterImages = 0.1f;
    public float TimeBetweenSteps = 0.3f;
    public Transform GroundChecker;
    public LayerMask Ground;
    public Collider2D HurtBox;

    void Start()
    {
        playerSR = GetComponent<SpriteRenderer>();
        _playerRB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        _matDefault = playerSR.material;
        _mainCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {   
        GroundCheck();
        VulnerableCheck();
        Jump();
    }
    private void FixedUpdate() {
        HorizontalMovement();
        DashState();
    }
    //=======================================================
    // Ground check checks to see if the player is touching
    // the Ground, while vulberable check adjusts player
    // vulnerablility dependant on _timeBeforeVulnerable
    // timer
    //=======================================================
    private void GroundCheck(){
        _isGrounded = Physics2D.OverlapCircle(GroundChecker.position, GroundCheckerRadius, Ground);
    }
    private void VulnerableCheck(){
        if(_timeBeforeVulnerable >= 0 && _isInvincible){ 
            _timeBeforeVulnerable -= Time.deltaTime;
        }else{_isInvincible = false;}
    }
    //=======================================================
    private void HorizontalMovement(){
        _moveInput = Mathf.Lerp(_moveInput, Input.GetAxisRaw("Horizontal"), Time.deltaTime * 30f);
        // flip player sprite depending on movement input
        if(_moveInput > 0) transform.eulerAngles = new Vector3(0,0,0);
        else if (_moveInput < 0) transform.eulerAngles = new Vector3(0,180,0);
        // apply movement input to horizontal velocity only if the
        // velocity has been reset after having dashed
        if(_resetVelocity){
            _animator.SetFloat("Speed", Mathf.Abs(_moveInput*Speed));
            _playerRB.velocity = new Vector2(_moveInput*Speed, _playerRB.velocity.y);
            _playerRB.gravityScale = 5f;
        }
        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0f && _isGrounded && _speedBoostTime <=0){
            FootSteps();
        }
    }
    private void FootSteps(){
        if(_timeBeofreNextStep < 0){
            _timeBeofreNextStep = TimeBetweenSteps;
            FindObjectOfType<Audio_Manager>().PlaySound("step");
        }else{
            _timeBeofreNextStep -= Time.deltaTime;
        }
    }

    void Jump(){
        if(Input.GetKeyDown(KeyCode.Mouse0) && _canJump){
            FindObjectOfType<Audio_Manager>().PlaySound("dash");
            _animator.SetBool("isJumping", true);
            /*
            * boolean _isJumping is used for _animator state. 
            * boolean can jump is used for whether or not the player can jump again. 
            * _resetVelocity ensures that the player will come to a dead stop after the dash
            * _speedBoostTime is adjusted for how much time the player is in the dash state
            */
            _isJumping = true;
            _canJump = false;
            _resetVelocity = false;
            _speedBoostTime = BoostTime;
            // after iamge initial image
            AfterImagePool.Instance.getFromPool();
            _lastImageXpos = transform.position.x;
            _lastImageYpos = transform.position.y;
            Vector3 _target =  _mainCamera.transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            Vector3 _difference = _target - transform.position;
            direction = _difference/_difference.magnitude;
            direction.Normalize();
        }
    }
    private void DashState(){
        if(_speedBoostTime > 0){
            // make the player weightless so that the player's dash isn't arced
            _playerRB.gravityScale = 0;
            _playerRB.velocity = direction*JumpForce;
            _speedBoostTime -= Time.deltaTime;
            // reveal the after images from the pool depending on editor variables
            if(Mathf.Abs(transform.position.x - _lastImageXpos) > DistanceBetweenAfterImages ||
               Mathf.Abs(transform.position.y - _lastImageYpos) > DistanceBetweenAfterImages){
                AfterImagePool.Instance.getFromPool();
                _lastImageXpos = transform.position.x;
                _lastImageYpos = transform.position.y;
            }
        // once we have run out of dash time, and the player's velocity needs to be reset,
        // the players Y velocity is stopped
        }else if(!_resetVelocity){
            _playerRB.velocity = new Vector2( _playerRB.velocity.x/5f, _playerRB.velocity.y/5f);
            _resetVelocity = true;
        }
        // if the player touches the Ground again, the _animator is then adjusted, and the player
        // can jump again
        if(_isGrounded && _speedBoostTime <= 0){
            _isJumping = false;
            _canJump = true;
            _animator.SetBool("isJumping", false);
        }
    }
    //=======================================================
    // flashWhite and flashDefault are used when the player
    // has been hurt, and is in an invincible state
    //=======================================================
    public void flashWhite(){
        if(!FindObjectOfType<Game_Manager>().gameOver){
            playerSR.material = _matWhite;
            Invoke("flashDefault", 0.1f);
        }
    }
    void flashDefault(){
        playerSR.material = _matDefault;
        if(_isInvincible){
            Invoke("flashWhite", 0.1f);
        }
    }
    //=======================================================
    // collision functions
    //=======================================================
    private void OnTriggerEnter2D(Collider2D other) {
        // _touchingBound is used for the parallax effect
        if(other.gameObject.CompareTag("PlayerBounds")){
            _touchingBound = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("PlayerBounds")){
            _touchingBound = false;
        }
    }
    private void OnTriggerStay2D(Collider2D other) {
        if(((other.gameObject.CompareTag("Enemy_01") && other.gameObject.GetComponent<Enemy_01>()._groundMode) ||
        (other.gameObject.CompareTag("Enemy_02")) || other.gameObject.CompareTag("Enemy Projectile")) && !_isInvincible){
            _isInvincible = true;
            _playerRB.constraints = RigidbodyConstraints2D.FreezePosition;
            _timeBeforeVulnerable = InvincibilityTime;
            FindObjectOfType<Audio_Manager>().PlaySound("player hurt");
            StartCoroutine("HurtFX");
        }
    }
    IEnumerator HurtFX(){
        FindObjectOfType<Camera_Shake>().shakeCamera(0.4f, 0.7f);
        yield return new WaitForSeconds(0.4f);
         _playerRB.constraints = RigidbodyConstraints2D.None;
         _playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
        FindObjectOfType<PlanetHealth>().Health -= 25f;
        _timeBeforeVulnerable = InvincibilityTime;
        if( FindObjectOfType<PlanetHealth>().Health > 0) flashWhite();
        yield return null;
    }
    //=======================================================

    //=======================================================
    // animation keyframe functions
    //=======================================================
        public void loopRun(){
        _animator.Play("Run", -1,  .5f);
    }
    public void loopSpin(){
        _animator.Play("Jump", -1,  (1f/48f)*21f);
    }
}
