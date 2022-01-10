using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    private Vector3 target;
    public GameObject targetCursor;
    public GameObject pointer;
    public Transform playerTransform;
    void Start()
    {
        Cursor.visible = true;
    }

    void Update()
    {
        pointer.transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z);
        if(FindObjectOfType<Game_Manager>().gameRunning && !FindObjectOfType<Game_Manager>().gamePaused){
            Cursor.visible = false;
            targetCursor.SetActive(true);
            target = transform.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            targetCursor.transform.position = new Vector3(target.x, target.y, targetCursor.transform.position.z);
            Vector3 difference = target - pointer.transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            pointer.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }else{
            Cursor.visible = true;
            targetCursor.SetActive(false);
        }
    }
}
