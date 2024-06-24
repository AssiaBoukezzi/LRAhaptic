using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;


public class StartExp : MonoBehaviour
{

    public GameObject start;
    public TextMeshProUGUI textMeshPro;
    public DateTime startTime;
    public TimeSpan Time;
    private int cpt = 0;
    private LineRenderer line;

    private string filePath;

    void Start()
    {
        // Initialize the file path
        filePath = Application.dataPath + "/participantInput.txt";
    }

    void SavePerformanceToFile(string time)
    {
        try
        {
            File.AppendAllText(filePath, time + "\n");
            Debug.Log("Time saved to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save name: " + e.Message);
        }
    }


    private void OnTriggerEnter(Collider collision)
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
            SavePerformanceToFile(secondsElapsed.ToString());
        }

        
    }
}
