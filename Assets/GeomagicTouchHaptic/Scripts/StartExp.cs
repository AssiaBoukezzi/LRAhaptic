using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;


public class StartExp : MonoBehaviour
{
    //pour demarrer le prichain essai
    public Button submitButton;
    public Text messageText; 

    //Fin des essais
    public Text messageTextFin;

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
            DetectCollisionTwo.cpt = 0;
            //Destroy(start);
            start.gameObject.SetActive(false);
            startTime = DateTime.Now;
        }
        if(collision.gameObject.tag == "END")
        {
            if(cpt < 5)
            {
                Time = DateTime.Now - startTime;
                float secondsElapsed = (float)Time.TotalSeconds;
                Debug.Log("Temps écoulé : " + secondsElapsed);
                cpt++;
                textMeshPro.text = "Temps = <color=#FF0000>" + secondsElapsed.ToString() + "s</color> \n  Cibles : <color=#FF0000>" + DetectCollisionTwo.cpt + "</color>";
                SavePerformanceToFile(secondsElapsed.ToString());
                submitButton.gameObject.SetActive(true);
                messageText.gameObject.SetActive(true);
            }
            else
            {
                messageTextFin.gameObject.SetActive(true);
            }
            
        }

        
    }
}
