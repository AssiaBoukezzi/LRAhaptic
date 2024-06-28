using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ParticipantInput : MonoBehaviour
{
    //pour le numero de partipant 
    public InputField nameInputField;
    public InputField conditionInputField;
    public Button submitButton;
    public Text messageText; 
    public GameObject start;

    //Pour demarrer un nouvel essai
    public Button submitButton2;
    public Text messageText2; 
    public TextMeshProUGUI textMeshPro; //reinitialiser les resultats 


    private string filePath;
    private string objectName = "NewLineRenderer"; // Nom des objets à détruire

    void Start()
    {
        // Initialize the file path
        filePath = Application.dataPath + "/participantInput.txt";

        // Attach listener to the submit button
        submitButton.onClick.AddListener(OnSubmitButtonClick);
        submitButton2.onClick.AddListener(OnSubmitButtonClick2);
    }

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

    public static string userName;
    public static string condition;

    void OnSubmitButtonClick()
    {
        // Get the name from the input field
        userName = nameInputField.text;
        condition = conditionInputField.text;
        //Data.Instance.participant = userName;

        // Save the name to a file
        SaveNameToFile(userName);

        // Clear the input field and hide it along with the submit button
        nameInputField.text = "";
        nameInputField.gameObject.SetActive(false);
        conditionInputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);


        // Display a message (optional)
        if (messageText != null)
        {
            messageText.text = "Name saved successfully!";
        }
    }

    void OnSubmitButtonClick2()
    {

        submitButton2.gameObject.SetActive(false);
        messageText2.gameObject.SetActive(false);
        DestroyObjectsByName(objectName, "Line");
        start.gameObject.SetActive(true);
        textMeshPro.text = "Temps = \n  Cibles : ";

        
    }

    void SaveNameToFile(string name)
    {
        try
        {
            File.AppendAllText(filePath, name + "\n");
            Debug.Log("Name saved to: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save name: " + e.Message);
        }
    }
}