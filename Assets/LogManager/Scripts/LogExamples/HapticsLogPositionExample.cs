//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using UnityEngine;
using System;
using ManagedPhantom;

//---------------------------------------------------------------------------
// HAPTIC MANAGER
//---------------------------------------------------------------------------

/// <summary>
/// Needle Insertion Prototype
/// </summary>
public class HapticsLogPositionExample : MonoBehaviour
{
    //---------------------------------------------------------------------------
    // CLASS INSTANCE
    //---------------------------------------------------------------------------

    /// <summary>
    /// class instance object -singleton-
    /// </summary>
	public static HapticsLogPositionExample instance;

    //---------------------------------------------------------------------------
    // SIMULATOR CONSTANTS
    //---------------------------------------------------------------------------

    /// <summary>
    /// 
    /// </summary>
    private const int WAIT_TO_START_NEW_MOVEMENT_BUTTON = 1;

    /// <summary>
    /// 
    /// </summary>
    private const int WAIT_TO_END_CURRENT_MOVEMENT_BUTTON = 2;

    //---------------------------------------------------------------------------
    // SIMULATOR VARIABLES
    //---------------------------------------------------------------------------

    /// <summary>
    /// State variable for haptic thread
    /// </summary>
    private int _state = WAIT_TO_START_NEW_MOVEMENT_BUTTON;
    
    //---------------------------------------------------------------------------
    // HAPTIC INFORMATION
    //---------------------------------------------------------------------------

    /// <summary>
    /// PHANTOM instance
    /// </summary>
    private SimplePhantomUnityController Phantom = null;

    /// <summary>
    /// The gimbal position [mm]
    /// </summary>
    private Vector3 HandPosition = Vector3.zero;

    /// <summary>
    /// The gimbal rotation
    /// </summary>
    private Quaternion HandRotation = Quaternion.identity;

    /// <summary>
    /// Force feedback to apply to device
    /// </summary>
    public Vector3 Force = Vector3.zero;

    //---------------------------------------------------------------------------
    // SYSTEM CONSTANTS
    //---------------------------------------------------------------------------

    /// <summary>
    /// Unit conversion from mm to Unity
    /// </summary>
	public float UnitLength = 0.01f;

    //---------------------------------------------------------------------------
    // OBJECT ATTRIBUTES
    //---------------------------------------------------------------------------

    /// <summary>
    /// Current position of needle
    /// </summary>
    private Vector3 MyPosition = Vector3.zero;

    /// <summary>
    /// Current rotation of needle
    /// </summary>
    private Quaternion MyRotation = Quaternion.identity;

    //---------------------------------------------------------------------------
    // FUNCTIONS
    //---------------------------------------------------------------------------

    /// <summary>
    /// Run only once when it is first activated
    /// </summary>
    private void Awake()
    {
        if (Phantom == null)
        {
            try
            {
                // Instantiation of Phantom
                Phantom = new SimplePhantomUnityController();

                // It specifies the method to be executed repeatedly
                Phantom.AddSchedule(PhantomUpdate, Hd.Priority.HD_DEFAULT_SCHEDULER_PRIORITY);
            }
            catch (UnityException)
            {
                Debug.Log("EXCEPTION >> Error trying to conect to PHANTOM device.\nVerify connection and try again!");
            }
        }
    }

    /// <summary>
    /// When enabled
    /// </summary>
    private void OnEnable()
    {
        Init();

        // To start the iterative process
        if (Phantom != null)
        {
            while (!Phantom.IsAvailable) ;

            Debug.Log("INITIALIZING DEVICE...");
            Phantom.Start();

            // Get information about the device -debug purpose only
            Debug.Log("INFORMATION PHANTOM device :\n" +
                "Usable Workspace Max    = " + Phantom.UsableWorkspaceMaximum + "\n" +
                "Usable Workspace Min    = " + Phantom.UsableWorkspaceMinimum + "\n" +
                "Workspace Available Max = " + Phantom.WorkspaceMaximum + "\n" +
                "Workspace Available Min = " + Phantom.WorkspaceMinimum + "\n" +
                "Instant. Update Rate    = " + Phantom.GetInstantaneousUpdateRate() + "\n" +
                "Max nominal Force       = " + Phantom.GetForceLimit() + "\n" +
                "Max force clamping enab = " + Phantom.IsEnabledMaxForceClamping() + "\n" +
                "SW force limit enab     = " + Phantom.IsEnabledSwForceLimit() + "\n");
        }
    }

    /// <summary>
    /// When disabled
    /// </summary>
    private void OnDisable()
    {
        Debug.Log("CLOSING DEVICE...");
        try
        {
            if (Phantom != null)
            {
                while (!Phantom.IsAvailable) Debug.Log("...");

                // Exit the use of PHANTOM
                Phantom.Close();
                Phantom = null;
                Debug.Log("DEVICE CLOSED");
            }
            else
                Debug.Log("DEVICE NOT CONNECTED");
        }
        catch (Exception e)
        {
            Debug.Log("EXCEPTION ON OnDisable");
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// Process at the start of the simulation
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Initialization of the manager
    /// </summary>
    private void Init()
    {
        // Save singleton instance
        if (instance == null)
            instance = this;
        else
            Debug.Log("Multiple instances of HapticManager");

        // Initialization of hand position and orientation
        HandPosition = Vector3.zero;
        HandRotation = Quaternion.identity;

        // Initialize object attributes;
        MyPosition = transform.position;
        MyRotation = transform.rotation;
    }

    /// <summary>
    /// Process each frame
    /// </summary>
    private void Update()
    {
        // Set device position (unity length) and orientation
        transform.localPosition = MyPosition;
        transform.localRotation = MyRotation;
    }

    /// <summary>
    /// Method that is repeatedly called in PHANTOM's cycle (default rate 1 [kHz])
    /// </summary>
    /// <returns><c>true</c>, if update was phantomed, <c>false</c> otherwise.</returns>
    bool PhantomUpdate()
    {
        // Get the position of the hand (gimbal part) [mm]
        HandPosition = Phantom.GetPosition();

        // Get the hand posture (orientation)
        HandRotation = Phantom.GetRotation();

        if (_state == WAIT_TO_START_NEW_MOVEMENT_BUTTON)
        {
            // USER indicates the start of a new gesture movement
            if (Phantom.GetButton() == Buttons.Button1)
            {
                LoggerManager.instance.LogNewMovement();
                
                _state = WAIT_TO_END_CURRENT_MOVEMENT_BUTTON;
            }
            // else // JUST do NOTHING... user is getting ready to record new gesture movements
        }
        else if (_state == WAIT_TO_END_CURRENT_MOVEMENT_BUTTON)
        {
            // USER indicates the end of current gesture movement
            if (Phantom.GetButton() == Buttons.Button2)
            {
                LoggerManager.instance.EndCurrentMovement();

                _state = WAIT_TO_START_NEW_MOVEMENT_BUTTON;
            } else
            {
                // Log current position (USER is executing the gesture movement)
                LoggerManager.instance.LogEntryPosition(new Vector3(HandPosition.x, HandPosition.y, HandPosition.z));
            }
        }
        // else // NOTHING... initialization has not begun

        // update attached object position/rotation
        MyRotation = HandRotation;
        MyPosition = new Vector3(HandPosition.x, HandPosition.y, HandPosition.z) * UnitLength;

        return true;
    }
}
