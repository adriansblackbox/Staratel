using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetHealth : MonoBehaviour
{
    public Image bar1Image;
    public Image bar2Image;
    private float _totalHealth = 100f;
    private float lerpHealth;
    public float Health;
    // Start is called before the first frame update
    void Start()
    {
        bar1Image = transform.Find("bar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Health = Mathf.Clamp(Health, 0f, 100f);
        lerpHealth = Mathf.Lerp(lerpHealth, Health, Time.deltaTime * 20f);
        bar1Image.fillAmount = lerpHealth / _totalHealth;
        bar2Image.fillAmount = Mathf.Lerp(bar2Image.fillAmount, bar1Image.fillAmount, Time.deltaTime * 5f);
    }
}
