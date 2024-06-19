using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ParticipantInput : MonoBehaviour
{
    public InputField nameInputField;
    public Button submitButton;
    public Text messageText; 

    private string filePath;

    void Start()
    {
        // Initialize the file path
        filePath = Application.dataPath + "/participantInput.txt";

        // Attach listener to the submit button
        submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    void OnSubmitButtonClick()
    {
        // Get the name from the input field
        string userName = nameInputField.text;

        // Save the name to a file
        SaveNameToFile(userName);

        // Clear the input field and hide it along with the submit button
        nameInputField.text = "";
        nameInputField.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);

        // Display a message (optional)
        if (messageText != null)
        {
            messageText.text = "Name saved successfully!";
        }
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