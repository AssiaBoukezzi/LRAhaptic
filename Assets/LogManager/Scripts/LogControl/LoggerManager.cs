/// ----------------------------///
///     RICCA Aylen 2016        ///
///     aricca8@gmail.com       ///
///     Internship IBISC        ///
/// ----------------------------///

//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

//---------------------------------------------------------------------------
// LOGGER MANAGER
//---------------------------------------------------------------------------

/// <summary>
/// Logger Manager class to manage log events
/// </summary>
public class LoggerManager : MonoBehaviour
{
    /// <summary>
    /// Class instance object -singleton-
    /// </summary>
    public static LoggerManager instance;

    /// <summary>
    /// Class in charge of XML management
    /// </summary>
    private XMLManager xmlManager;

    /// <summary>
    /// Consumer-Producer thread based queue for haptic info
    /// </summary>
    private ProducerConsumerQueue HapticWorkerQueue;

    /// <summary>
    /// 
    /// </summary>
    private const int START_MOVEMENT_STATE = 1;

    /// <summary>
    /// 
    /// </summary>
    private const int IDLE_STATE = 2;

    /// <summary>
    /// 
    /// </summary>
    private const int NEUTRAL_MOVEMENT_STATE = 0;

    /// <summary>
    /// 
    /// </summary>
    private const int END_MOVEMENT_STATE = -1;

    /// <summary>
    /// Lock to manage the state of the haptic
    /// </summary>
    private readonly object _lockState = new object();

    /// <summary>
    /// 
    /// </summary>
    private int _state = NEUTRAL_MOVEMENT_STATE;

    /// <summary>
    /// 
    /// </summary>
    private int _loggerState = NEUTRAL_MOVEMENT_STATE;

    /// <summary>
    /// File Descriptor key of the current scene logging information about haptic
    /// </summary>
    private int _keyFileHaptics = -1;

    /// <summary>
    /// Time count for logging data (sampled from haptic device at 1MHz)
    /// </summary>
    private int _time = -1;

    /// <summary>
    /// Time count for logging data (sampled from haptic device at 1MHz)
    /// </summary>
    private int _movementCount = 0;

    public Material RedMaterial;
    public Material GreenMaterial;

    private List<Vector3> _positionList;

    /// <summary>
    /// Process at the start of the simulation
    /// </summary>
    void Start()
    {
        // Save singleton instance
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Multiple instances of Logger Manager");

        // init variables
        _time = 0;
        _movementCount = 0;
        _loggerState = _state = NEUTRAL_MOVEMENT_STATE;

        // Instantiate xml reader-writer
        xmlManager = new XMLManager();

        _positionList = new List<Vector3>();

        Debug.Log("Created file for logs");
        // Get file descriptor for haptics information
        _keyFileHaptics = xmlManager.openLogFileForHapticsPosition("logHaptics_data");
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        lock (_lockState)
        {
            _loggerState = _state;
            _state = NEUTRAL_MOVEMENT_STATE;
        }

        switch (_loggerState)
        {
            case START_MOVEMENT_STATE:
                Debug.Log("Start gesture movement");

                // add entry to XML for new movement
                xmlManager.openFileEntry(_keyFileHaptics, _movementCount++);

                // Create Producer-Consumer Queue for logging haptic position info
                HapticWorkerQueue = new ProducerConsumerQueue(xmlManager, true, _keyFileHaptics);
                break;
            case END_MOVEMENT_STATE:
                Debug.Log("Stop gesture movement");

                // end movement position and dispose thread
                foreach (var position in _positionList)
                {
                    HapticWorkerQueue.EnqueueTask(_keyFileHaptics, _time++, position);
                }

                _positionList.Clear();
                _positionList = new List<Vector3>();
                _time = 0;

                HapticWorkerQueue.Dispose();

                // wait for writer to finish
                StartCoroutine("showGreenFlagWhenReady");
                break;
            default:
                break;
        }


        // Close file for logging movements
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Close Log file of movements");
            xmlManager.closeFile(_keyFileHaptics);
        }
    }

    /// <summary>
    /// Coroutine to load the scene for the goodbye note
    /// </summary>
    /// <returns>Opens goodbye scene with fininsh message</returns>
    IEnumerator showGreenFlagWhenReady()
    {
        GetComponent<Renderer>().material = RedMaterial;

        while (HapticWorkerQueue == null || !HapticWorkerQueue.IsDisposed)
            yield return null;

        GetComponent<Renderer>().material = GreenMaterial;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    public void LogEntryPosition(Vector3 position)
    {
        _positionList.Add(position);
    }

    /// <summary>
    /// 
    /// </summary>
    public void LogNewMovement()
    {
        lock (_lockState)
            _state = START_MOVEMENT_STATE;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EndCurrentMovement()
    {
        lock (_lockState)
            _state = END_MOVEMENT_STATE;
    }
}