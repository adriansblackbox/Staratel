using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Shake : MonoBehaviour
{
    public GameObject Camera;
    public Transform gameView;
    //private Vector3 originalPos;
    private float _elapsedTime;
    private float _magnitude;
    // Start is called before the first frame update
    void Start(){
        _magnitude = 0;
        _elapsedTime = 0;
    }
    public void shakeCamera(float duration, float _magnitude){
        if(_elapsedTime <= 0){
            _elapsedTime = duration;
            this._magnitude = _magnitude;
        }else if(_magnitude > this._magnitude){
            _elapsedTime = duration;
            this._magnitude = _magnitude;
        }
    }
    private void Update() {
        if(_elapsedTime > 0){
            float xOffset = Random.Range(-.5f, .5f)*_magnitude;
            float yOffset = Random.Range(-.5f, .5f)*_magnitude;
            Vector3 newPosition = new Vector3(xOffset, yOffset, Camera.transform.position.z);
            Camera.transform.position = Vector3.Lerp(Camera.transform.position, newPosition, 300f*Time.deltaTime);
            _elapsedTime -= Time.deltaTime;
        }else if(_magnitude != 0){
            Camera.transform.position = gameView.position;
            _magnitude = 0;
        }
    }
}
