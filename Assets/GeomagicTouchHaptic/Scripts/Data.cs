using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Data : MonoBehaviour
{
    //public static Data Instance { get; private set; }
    public string filePath = "participants_data_positions.csv"; // Chemin du fichier CSV
    public string filePath2 = "participants_data_perform.csv"; // Chemin du fichier CSV
    private List<string[]> rowData = new List<string[]>(); // Liste pour stocker les lignes de données
    private List<string[]> rowData2 = new List<string[]>(); // Liste pour stocker les lignes de données
    private string participant;
    private int numTrial;
    private int nbTarget;
    private float time; 
    
    // Start is called before the first frame update
    void Start()
    {
        // Ajouter les en-têtes des colonnes
        string[] headers = new string[] {
            "Numero du participant",
            "Numero d'essaie",
            "Condition",
            "Position X",
            "Position Y",
            "Position Z",
            "Cible touché",
            "temps"
        };

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

        rowData.Add(headers);
        rowData2.Add(headers2);

    }

    // Fonction pour ajouter des données
    public void AddData(string participantNumber, int trialNumber, string condition, float posX, float posY, float posZ,
                        int targetHit, float timeBetweenPoints)
    {
        string[] row = new string[] {
            participantNumber,
            trialNumber.ToString(),
            condition,
            posX.ToString(),
            posY.ToString(),
            posZ.ToString(),
            targetHit.ToString(),
            timeBetweenPoints.ToString()
        };
        rowData.Add(row);
    }

    // Fonction pour ajouter des données
    public void AddData2(string participantNumber, int trialNumber, string condition,
                        int targetsHit, int targetScore, int trajectoryScore, float totalTime)
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
        string[][] output = new string[rowData.Count][];
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
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

    // Update is called once per frame
    void Update()
    {

        
    }
}
