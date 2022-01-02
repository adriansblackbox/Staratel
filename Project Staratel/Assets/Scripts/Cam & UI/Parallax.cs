using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public GameObject Player;
    public float Speed;
    private Rigidbody2D _playerRB;
    public float yOrigin;
    // Start is called before the first frame update
    void Awake(){
        Player = GameObject.FindWithTag("Player");
        _playerRB = Player.GetComponent<Rigidbody2D>();
        //positionOrigin = this.transform.position;
        yOrigin = this.transform.position.y;
    }

    // Update is called once per frame
    void Update(){
        if(Mathf.Abs(_playerRB.velocity.x) > 0 && !Player.GetComponent<Player_Controller>()._touchingBound)
            this.transform.position = new Vector3((this.transform.position.x - ((_playerRB.velocity.x/10) *Speed) * Time.deltaTime), this.transform.position.y, this.transform.position.z);
        if(!Player.GetComponent<Player_Controller>()._isGrounded && this.transform.position.y < yOrigin + 5)
           this.transform.position = new Vector3(this.transform.position.x, (this.transform.position.y - ((_playerRB.velocity.y/25) *Speed) * Time.deltaTime), this.transform.position.z);
        else if(this.transform.position.y != yOrigin)  
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, yOrigin, this.transform.position.z), Time.deltaTime * 20f);
    }
}
