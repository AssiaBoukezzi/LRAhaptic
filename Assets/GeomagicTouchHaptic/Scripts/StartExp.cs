using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.UI;
using DTW;


public class StartExp : MonoBehaviour
{
    //sauvegarde des données
    public string filePath2 = "_participants_data_perform_pre_post_test.csv"; // Chemin du fichier CSV
    public string filePath4 = "_participants_data_perform.csv"; // Chemin du fichier CSV
    public List<string[]> rowData2 = new List<string[]>(); // Liste pour stocker les lignes de données
    public List<string[]> rowData4 = new List<string[]>(); // Liste pour stocker les lignes de données


    

    //pour demarrer le prichain essai
    public Button submitButton;
    public Text messageText; 
    public TextMeshProUGUI countDown;
    public TextMeshProUGUI description;
    public GameObject background;
    public TextMeshProUGUI numTrial;
    private int nbT = 14;

    //Fin des essais
    public TextMeshProUGUI messageTextFin;
    public GameObject boutton;

    public GameObject start;
    public TextMeshProUGUI textMeshPro;
    public DateTime startTime;
    public TimeSpan Time;
    public static int essai = 0;
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

        if(ParticipantInput.condition == "a")
        {

            // Ajouter les en-têtes des colonnes        
            string[] headers4 = new string[] {
                "Numero du participant",
                "Numero d'essaie",
                "Condition",
                "Nombre cibles touchées",
                "Score cibles",
                "Score trajectoire",
                "Temps total"
            };

            rowData4.Add(headers4);


        }
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

    public void AddData4(string participantNumber, int trialNumber, string condition,
                        int targetsHit, int targetScore, float trajectoryScore, float totalTime)
    {
        string[] row4 = new string[] {
            participantNumber,
            trialNumber.ToString(),
            condition,
            targetsHit.ToString(),
            targetScore.ToString(),
            trajectoryScore.ToString(),
            totalTime.ToString()
        };
        rowData4.Add(row4);
    }

    // Fonction pour écrire les données dans un fichier CSV
    public void WriteToFile(string filePath, List<string[]> rowData)
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

    public static bool startBeforeEnd = false;
    private float scoreDTW = 0;


    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "START")
        {
            print("start");
            DetectCollisionTwo.cpt = 0;
            //Destroy(start);
            start.gameObject.SetActive(false);
            startTime = DateTime.Now;
            
            startBeforeEnd = true;
        }
        if(collision.gameObject.tag == "END" && startBeforeEnd)
        {
            startBeforeEnd = false;

            //calcule du temps total
            Time = DateTime.Now - startTime;
            float secondsElapsed = (float)Time.TotalSeconds;

            //calcul du DTW
            SimpleDTW dtw = new SimpleDTW(TwoHapticsCoulombForceAttraction3.trajOptimal,TwoHapticsCoulombForceAttraction3.leftPos);
            dtw.computeDTW();
            scoreDTW = (float)dtw.getSum(); //donne la valeur du dtw
            print("DTW = "+ scoreDTW);

            string score;
            if(scoreDTW <= 20)
            {
                score = "Très bien";
            }
            else if(scoreDTW <= 75)
            {
                score = "Bien";
            }
            else if(scoreDTW <= 150)
            {
                score = "Mauvais";
            }
            else
            {
                score = "Très mauvais";
            }
            textMeshPro.text = "Temps = <color=#FF0000>" + secondsElapsed.ToString() + "s</color> \n  Cibles : <color=#FF0000>" + DetectCollisionTwo.cpt + "</color> \n Score : <color=#FF0000>" + score + "</color>";
            
            if(ParticipantInput.condition == "a")
            {
                nbT = 10;
                if(essai < 11 && essai > 0)
                {
                    AddData2(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed);        
                    StartCoroutine(CountDownTrial());
                }
                else if(essai == 0)
                {

                    DestroyObjectsByName(objectName, "Line");
                    start.gameObject.SetActive(true);
                }
                else
                {
                    end = true;
                    AddData2(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed);
                    WriteToFile(ParticipantInput.userName+filePath2, rowData2);
                    textMeshPro.text = + " Merci pour votre participation";
                    background.gameObject.SetActive(true);
                    boutton.gameObject.SetActive(false);
                
                }
                
                
            }
            else if(essai == 1 || essai == 2 || essai == 13 || essai == 14)
            {
                AddData2(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed);        
                StartCoroutine(CountDownTrial());
            }
            else if(essai == 0)
            {
                DestroyObjectsByName(objectName, "Line");
                start.gameObject.SetActive(true);
            }
            else if(essai < 13 && essai > 2)
            {
                AddData4(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed);        
                StartCoroutine(CountDownTrial());
            }
            else
            {
                end = true;
                AddData2(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed);
                AddData4(ParticipantInput.userName, essai, ParticipantInput.condition, DetectCollisionTwo.cpt, 0, scoreDTW, secondsElapsed); 
                WriteToFile(ParticipantInput.userName+filePath2, rowData2);
                WriteToFile(ParticipantInput.userName+filePath4, rowData4);
                textMeshPro.text = "Merci pour votre participation";
                background.gameObject.SetActive(true);
                boutton.gameObject.SetActive(false);
                
            }
            essai++;
            
        }
        if(collision.gameObject.tag == "nouvelEssai")
        {
            background.gameObject.SetActive(false);
            boutton.gameObject.SetActive(false);
        }

        
    }
    IEnumerator CountDownTrial()
    {
        //description.text = "Vous avez fini l'essai numéro "+ essai +", le prochaine commence dans : ";
        
        yield return new WaitForSeconds(1); // Attendre 2 seconde
        background.gameObject.SetActive(true);
        boutton.gameObject.SetActive(true);
        numTrial.text = "Vous avez fini l'essai "+ essai + "/" +nbT;
        /*countDown.text = "5";
        yield return new WaitForSeconds(1); // Attendre une seconde
        countDown.text = "4";
        yield return new WaitForSeconds(1); // Attendre une seconde
        countDown.text = "3";
        yield return new WaitForSeconds(1); // Attendre une seconde
        countDown.text = "2";
        yield return new WaitForSeconds(1); // Attendre une seconde
        countDown.text = "1";
        yield return new WaitForSeconds(1); // Attendre une seconde
        countDown.text = "";
        //description.text = "";
        background.gameObject.SetActive(false);*/
        DestroyObjectsByName(objectName, "Line");
        start.gameObject.SetActive(true);
        //textMeshPro.text = "Temps = \n  Cibles : \n Score = ";
    }

    private string objectName = "NewLineRenderer"; // Nom des objets à détruire

    void DestroyObjectsByName(string name, string name2)
    {
        
        // Trouver tous les objets de la scène
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        // Parcourir tous les objets
        
        foreach (GameObject obj in allObjects)
        {
            // Si l'objet a le nom spécifié, le détruire
            if ((obj.name == name || obj.name == name2) && obj.GetComponent<LineRenderer>().positionCount != 0 )
            {
                obj.gameObject.SetActive(false);
            }
        }

        Debug.Log("All objects named '" + name + "' have been destroyed.");
    }
                
}