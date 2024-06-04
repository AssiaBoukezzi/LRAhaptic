using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class StartExp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //startTime = Time.time;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject start;
    public TextMeshProUGUI textMeshPro;
    public DateTime startTime;
    public TimeSpan Time;
    private int cpt = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "START")
        {
            print("start");
            Destroy(start);
            startTime = DateTime.Now;
        }
        if(collision.gameObject.tag == "END" && cpt == 0)
        {
            Time = DateTime.Now - startTime;
            float secondsElapsed = (float)Time.TotalSeconds;
            Debug.Log("Temps écoulé : " + secondsElapsed);
            cpt++;
            textMeshPro.text = "Temps = <color=#FF0000>" + secondsElapsed.ToString() + "s</color> \n  Cibles : <color=#FF0000>" + DetectCollisionTwo.cpt + "</color>";
        }
    }
}
