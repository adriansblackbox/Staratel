using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    public GameObject afterImagePrefab;
    private Queue<GameObject> availableObjects = new Queue<GameObject>();
    public static AfterImagePool Instance {get; private set;}

    private void Awake() {
        Instance = this;
        growPool();
    }

    private void growPool(){
        for (int i = 0; i < 10; i++){
            var instanceToAdd = Instantiate(afterImagePrefab);
            instanceToAdd.transform.SetParent(transform);
            addToPool(instanceToAdd);
        }
    }
    public void addToPool(GameObject instance){
        instance.SetActive(false);
        availableObjects.Enqueue(instance);
    }

    public GameObject getFromPool(){
        if(availableObjects.Count == 0) growPool();
        var instance = availableObjects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
