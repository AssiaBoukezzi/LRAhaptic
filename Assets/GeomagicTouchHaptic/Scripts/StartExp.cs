using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;


public class StartExp : MonoBehaviour
{
    //sauvegarde des données
    public string filePath2 = "_participants_data_perform.csv"; // Chemin du fichier CSV
    public List<string[]> rowData2 = new List<string[]>(); // Liste pour stocker les lignes de données

    

    //pour demarrer le prichain essai
    public Button submitButton;
    public Text messageText; 

    //Fin des essais
    public Text messageTextFin;

    public GameObject start;
    public TextMeshProUGUI textMeshPro;
    public DateTime startTime;
    public TimeSpan Time;
    public static int essai=0;
    private LineRenderer line;

    private string filePath;

    public static bool end = false; 

    void Start()
    {
        // Ajouter les en-têtes des colonnes        
        string[] headers2 = new string[] {
            "Numero du participant",
            "Numero d'essaie",
            "Condition",
            "Nombre cibles touchées",
            "Score cibles",
            "Score trajectoire",
            "Temps total"
        };

        rowData2.Add(headers2);
    }

    // Fonction pour ajouter des données
    public void AddData2(string participantNumber, int trialNumber, string condition,
                        int targetsHit, int targetScore, float trajectoryScore, float totalTime)
    {
        string[] row = new string[] {
            participantNumber,
            trialNumber.ToString(),
            condition,
            targetsHit.ToString(),
            targetScore.ToString(),
            trajectoryScore.ToString(),
            totalTime.ToString()
        };
        rowData2.Add(row);
    }

    // Fonction pour écrire les données dans un fichier CSV
    public void WriteToFile(string filePath)
    {
        string Pathh = Path.Combine(Application.dataPath, filePath);
        string[][] output = new string[rowData2.Count][];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData2[i];
        }

        int length = output.GetLength(0);
        using (StreamWriter writer = new StreamWriter(Pathh))
        {
            for (int i = 0; i < length; i++)
            {
                writer.WriteLine(string.Join(";", output[i]));
            }
        }
        Debug.Log("CSV file created at: " + Pathh);
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
            essai++;
        }
        if(collision.gameObject.tag == "END")
        {
            //calcule du temps total
            Time = DateTime.Now - startTime;
            float secondsElapsed = (float)Time.TotalSeconds;
            textMeshPro.text = "Temps = <color=#FF0000>" + secondsElapsed.ToString() + "s</color> \n  Cibles : <color=#FF0000>" + DetectCollisionTwo.cpt + "</color>";
            if(essai < 6)
            {
                if(essai > 1)
                {
                    AddData2(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, TwoHapticsCoulombForceAttraction3.scoreDTW, secondsElapsed);
                }
                
                submitButton.gameObject.SetActive(true);
                messageText.gameObject.SetActive(true);
            }
            else
            {
                end = true;
                WriteToFile(ParticipantInput.userName+filePath2);
                messageTextFin.gameObject.SetActive(true);
            }
            
        }

        
    }
}