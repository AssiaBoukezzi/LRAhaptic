/// ----------------------------///
///     RICCA Aylen 2016        ///
///     aricca8@gmail.com       ///
///     Internship IBISC        ///
/// ----------------------------///

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
public class OneHapticMovementOnly : MonoBehaviour
{
    //---------------------------------------------------------------------------
    // CLASS INSTANCE
    //---------------------------------------------------------------------------

    /// <summary>
    /// class instance object -singleton-
    /// </summary>
	public static OneHapticMovementOnly instance;

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
                Phantom.AddSchedule(PhantomUpdate, Hd.Priority.HD_RENDER_EFFECT_FORCE_PRIORITY);
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

    public void setForce(Vector3 force)
    {
        Force = new Vector3(force.x, force.y, force.z);
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

        MyRotation = HandRotation;
        MyPosition = HandPosition * UnitLength;

        // Set the specified force
        Phantom.SetForce(Force);

        return true;
    }
}
