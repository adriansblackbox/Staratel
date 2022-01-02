using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class After_Image : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    private SpriteRenderer playerSR;
    public Transform player;
    public Material white;

    private Color color;
    public float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    public float alphaSet = 0.8f;
    public float alphaDecay = 5f;

    private void OnEnable() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        playerSR = player.GetComponent<SpriteRenderer>();

        this.transform.localScale = player.transform.localScale;

        alpha = alphaSet;
        SpriteRenderer.sprite = playerSR.sprite;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        timeActivated = Time.time;
    }

    private void Update() {
        alpha -= alphaDecay * Time.deltaTime;
        color = new Color(1f,1f,1f,alpha);
        SpriteRenderer.color = color;
        SpriteRenderer.material = white;

        if(Time.time >= (timeActivated + activeTime)){
            AfterImagePool.Instance.addToPool(gameObject);
        }
    }
}
