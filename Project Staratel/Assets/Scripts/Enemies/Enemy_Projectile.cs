using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Projectile : MonoBehaviour
{
    public float projectileSpeed;
    
    private void Update() {
        transform.localPosition += transform.right * projectileSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D other) {
        
        if(other.gameObject.CompareTag("Bouncy Walls") || 
           other.gameObject.CompareTag("BoundsDeath") || 
           other.gameObject.CompareTag("Ground")){
            Destroy(gameObject);
        }
    }
}
