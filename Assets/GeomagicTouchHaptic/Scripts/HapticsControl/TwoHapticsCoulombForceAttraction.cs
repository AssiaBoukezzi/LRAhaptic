//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using UnityEngine;
using System;
using GeomagicTouchPhantom;
using System.Collections.Generic;

//---------------------------------------------------------------------------
// HAPTIC MANAGER
//---------------------------------------------------------------------------

/// <summary>
/// Needle Insertion Prototype
/// </summary>
public class TwoHapticsCoulombForceAttraction : MonoBehaviour
{
    //---------------------------------------------------------------------------
    // CLASS INSTANCE
    //---------------------------------------------------------------------------

    /// <summary>
    /// class instance object -singleton-
    /// </summary>
	public static TwoHapticsCoulombForceAttraction instance;

    //---------------------------------------------------------------------------
    // HAPTIC INFORMATION
    //---------------------------------------------------------------------------

    /// <summary>
    /// PHANTOM instance
    /// </summary>
    private PhantomUnityController Phantoms = null;

    /// <summary>
    /// Struct containing information attached to one device
    /// </summary>
    public struct PhantomDeviceInfo
    {
        /// <summary>
        /// Device configuration name
        /// </summary>
        public string Name;

        /// <summary>
        /// Device handler
        /// </summary>
        public uint hHdAPI;

        /// <summary>
        /// Device position
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// Device rotation
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// Force to be applied to the attached device
        /// </summary>
        public Vector3 force;

        /// <summary>
        /// Scene object attached to this device's position and rotation
        /// </summary>
        public GameObject tool;
    }

    /// <summary>
    /// Left device structure
    /// </summary>
    public PhantomDeviceInfo LeftPhantomDevice;

    /// <summary>
    /// Right device structure
    /// </summary>
    public PhantomDeviceInfo RightPhantomDevice;

    /// <summary>
    /// The gimbal position [mm]
    /// </summary>
    private Vector3 HandPosition_Left = Vector3.zero;

    /// <summary>
    /// The gimbal position [mm]
    /// </summary>
    private Vector3 HandPosition_Right = Vector3.zero;

    /// <summary>
    /// The gimbal rotation
    /// </summary>
    private Quaternion HandRotation_Left = Quaternion.identity;

    /// <summary>
    /// The gimbal rotation
    /// </summary>
    private Quaternion HandRotation_Right = Quaternion.identity;

    /// <summary>
    /// Force feedback to apply to device
    /// </summary>
    public Vector3 Force_Left = Vector3.zero;

    /// <summary>
    /// Force feedback to apply to device
    /// </summary>
    public Vector3 Force_Right = Vector3.zero;

    //---------------------------------------------------------------------------
    // SYSTEM CONSTANTS
    //---------------------------------------------------------------------------

    /// <summary>
    /// Unit conversion from mm to Unity
    /// </summary>
	public float UnitLength = 0.01f;

    public float PlaneY = -0.8f;

    //---------------------------------------------------------------------------
    // OBJECT ATTRIBUTES
    //---------------------------------------------------------------------------

    //---------------------------------------------------------------------------
    // FUNCTIONS
    //---------------------------------------------------------------------------

    /// <summary>
    /// Runs only once when it is first activated
    /// </summary>
    private void Awake()
    {
        if (Phantoms == null)
        {
            try
            {
                LeftPhantomDevice.Name = "PHANToM 1";
                RightPhantomDevice.Name = "PHANToM 2";

                Debug.Log("Initializing phantoms");

                // Instantiation of Phantoms
                Phantoms = new PhantomUnityController();

                try { 
                    LeftPhantomDevice.hHdAPI = Phantoms.InitDevice(LeftPhantomDevice.Name);
                    RightPhantomDevice.hHdAPI = Phantoms.InitDevice(RightPhantomDevice.Name);
                } catch (UnityException)
                {
                    Phantoms = null;
                }
            }
            catch (UnityException)
            {
                Debug.Log("EXCEPTION >> Error trying to conect to PHANTOM devices.\nVerify connection and try again!");
            }
        }
    }

    /// <summary>
    /// When enabled
    /// </summary>
    private void OnEnable()
    {
        Init();

        if (Phantoms == null)
        {
            //TODO this is awful! :S
            Debug.Log("ERROR INITIALIZING DEVICE...");
            return;
        }

        Debug.Log("INITIALIZING DEVICE...");
        Phantoms.Start();

        // It specifies the method to be executed repeatedly
        Phantoms.AddSchedule(PhantomUpdate, HdAPI.Priority.HD_DEFAULT_SCHEDULER_PRIORITY);
    }

    /// <summary>
    /// When disabled
    /// </summary>
    private void OnDisable()
    {
        Debug.Log("CLOSING DEVICE...");
        try
        {
            if (Phantoms != null)
            {
                //Phantoms.exitHandler();
                Phantoms.Close();
                Phantoms = null;
                Debug.Log("DEVICES CLOSED");
            }
            else
                Debug.Log("DEVICES NOT CONNECTED");
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

        // Attach gameobjects to devices
        LeftPhantomDevice.tool = GameObject.Find("Device_1");
        RightPhantomDevice.tool = GameObject.Find("Device_2");

        // Initialization of hand position and orientation
        LeftPhantomDevice.position = Vector3.zero;
        RightPhantomDevice.position = Vector3.zero;
        LeftPhantomDevice.rotation = Quaternion.identity;
        RightPhantomDevice.rotation = Quaternion.identity;

    }

    public GameObject prefab ;
    public GameObject co_embody;
    public float control = 0.5f;
    /// <summary>
    /// Process each frame
    /// </summary>
    private void Update()
    {
        LeftPhantomDevice.tool.transform.localPosition = LeftPhantomDevice.position;
        RightPhantomDevice.tool.transform.localPosition = RightPhantomDevice.position;
        LeftPhantomDevice.tool.transform.localRotation = LeftPhantomDevice.rotation;
        RightPhantomDevice.tool.transform.localRotation = RightPhantomDevice.rotation;

        print(LeftPhantomDevice.position);

        Vector3 pos = new Vector3(LeftPhantomDevice.position.x, 0, LeftPhantomDevice.position.z);
        
        float x = 0;
        float y = 0;
        float z = 0;
        

        if(LeftPhantomDevice.position.x < RightPhantomDevice.position.x)
        {
            x = LeftPhantomDevice.position.x + (Math.Abs(LeftPhantomDevice.position.x - RightPhantomDevice.position.x) * control);
        }
        else
        {
            x = LeftPhantomDevice.position.x - (Math.Abs(LeftPhantomDevice.position.x - RightPhantomDevice.position.x) * control);
        }

        if(LeftPhantomDevice.position.y < RightPhantomDevice.position.y)
        {
            y = LeftPhantomDevice.position.y + (Math.Abs(LeftPhantomDevice.position.y - RightPhantomDevice.position.y) * control);
        }
        else
        {
            y = LeftPhantomDevice.position.y - (Math.Abs(LeftPhantomDevice.position.y - RightPhantomDevice.position.y) * control);
        }

        if(LeftPhantomDevice.position.z < RightPhantomDevice.position.z)
        {
            z = LeftPhantomDevice.position.z + (Math.Abs(LeftPhantomDevice.position.z - RightPhantomDevice.position.z) * control);
        }
        else
        {
            z = LeftPhantomDevice.position.z - (Math.Abs(LeftPhantomDevice.position.z - RightPhantomDevice.position.z) * control);
        }

        Vector3 posDiff = new Vector3(x + (Math.Abs(LeftPhantomDevice.position.x - RightPhantomDevice.position.x) * control), y + (Math.Abs(LeftPhantomDevice.position.y - RightPhantomDevice.position.y) * control), z + (Math.Abs(LeftPhantomDevice.position.z - RightPhantomDevice.position.z) * control));
        Vector3 xyz = new Vector3(x, y, z);

        if (HandPosition_Left.y <= 0.01f)
        {
            GameObject newObject = Instantiate(prefab, pos, Quaternion.identity);
        }

        if(control == 0)
        {
            co_embody.transform.position = LeftPhantomDevice.position;
        }
        else if(control == 1)
        {
            co_embody.transform.position = RightPhantomDevice.position;
        }
        else
        {
            co_embody.transform.position = xyz;
        }
        
        
        

    }

public Vector3 Force_R;
public int state=0;


public void setForce(Vector3 Force)
{
    RightPhantomDevice.force = new Vector3(Force.x, Force.y, Force.z);
    Force_R = RightPhantomDevice.force;

}

Vector3 CurrentPosition;
Vector3 CurrentPosition2;
private float FirstPlanePosition = 0;
private float FirstPlaneStiffness = 0.25f;
private float ForceStiffness;
private float SkinLayerStiffness = 31.5f;
private float FirstLayerDamping = 4f;
public float SkinLayerCutting = 1.22f;
public float FIRST_LAYER_TOP = 0.10f;
public float SecondPlanePosition = -1.0f;
public float SecondPlaneStiffness = 0.33f;
public float DEVICE_FORCE_SCALE = 0.4f;

private Vector3 membraneForce2;


private LineRenderer currentDrawing;
private int index;
private int currentColorIndex = 0;
public Transform tip;
public Material drawingMaterial;
public float penWidth = 0.01f;
public Color[] penColors;






    /// <summary>
    /// Method that is repeatedly called in PHANTOM's cycle (default rate 1 [kHz])
    /// </summary>
    /// <returns><c>true</c>, if update was phantomed, <c>false</c> otherwise.</returns>
    bool PhantomUpdate()
    {

        
        HdAPI.hdBeginFrame(LeftPhantomDevice.hHdAPI);
        HandPosition_Left = Phantoms.GetPosition();
        HandRotation_Left = Phantoms.GetRotation();

        HdAPI.hdBeginFrame(RightPhantomDevice.hHdAPI);
        HandPosition_Right = Phantoms.GetPosition();
        HandRotation_Right = Phantoms.GetRotation();

        Vector3 pos_diff = new Vector3(HandPosition_Left.x - HandPosition_Right.x, HandPosition_Left.y - HandPosition_Right.y, HandPosition_Left.z - HandPosition_Right.z);
        LeftPhantomDevice.force = ForceField(pos_diff);


        CurrentPosition = HandPosition_Left * UnitLength;
        float DopStiffness = FirstPlanePosition + 0.075f - CurrentPosition.y;

        if(DopStiffness > 0)
        {
            Vector3 HandVelocity = Phantoms.GetVelocity();
            float Velocity = HandVelocity.y;
            Velocity = Mathf.Clamp(Velocity, -0.1f, 0.1f);

            ForceStiffness = (2.5f + SkinLayerStiffness) * DopStiffness + FirstLayerDamping * (-Velocity) * DopStiffness;

            ForceStiffness *= DEVICE_FORCE_SCALE;

            float membraneDamping = 0.003f;
            float membraneStiffness = 0.04f;
            float distanceCoeficient = 0.08f;
            float ClampValue = 0.4f;

            membraneForce2 = -membraneDamping * HandVelocity;
            if (membraneForce2.magnitude > ClampValue)
            {
                membraneForce2.Normalize();
                membraneForce2 *= ClampValue;
            }

            Vector3 ForceS = Vector3.zero;
            ForceS.x += membraneForce2.x - DopStiffness * distanceCoeficient;
            ForceS.y += membraneForce2.y;
            ForceS.z += membraneForce2.z - DopStiffness * distanceCoeficient;

            ClampValue = (float)Phantoms.GetContinuousForceLimit();
            membraneForce2 = membraneStiffness * (Vector3.zero - HandPosition_Left);
            if (membraneForce2.magnitude > ClampValue)
            {
                membraneForce2.Normalize();
                membraneForce2 *= ClampValue;
            }

            //ForceS.x += membraneForce2.x;
            ForceS.x = 0;
            ForceS.y += membraneForce2.y;
            //ForceS.z += membraneForce2.z;
            ForceS.z = 0;
            LeftPhantomDevice.force = ForceS + ForceField(pos_diff);
        }

        if (HandPosition_Left.y <= FirstPlanePosition)
        {
            float penetrationDistance = Mathf.Abs(HandPosition_Left.y);

            if (HandPosition_Left.y > SecondPlanePosition + 0.04f)
            {
                LeftPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * FirstPlaneStiffness), 0);
                HandPosition_Left *= UnitLength;
                LeftPhantomDevice.position = new Vector3(HandPosition_Left.x, HandPosition_Left.y, HandPosition_Left.z);
                LeftPhantomDevice.rotation = new Quaternion(HandRotation_Left.x, HandRotation_Left.y, HandRotation_Left.z, HandRotation_Left.w);
            }
            else
            {
                LeftPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * SecondPlaneStiffness), 0);
                HandPosition_Left *= UnitLength;
                LeftPhantomDevice.position = new Vector3(HandPosition_Left.x, SecondPlanePosition * UnitLength, HandPosition_Left.z);
                LeftPhantomDevice.rotation = new Quaternion(HandRotation_Left.x, HandRotation_Left.y, HandRotation_Left.z, HandRotation_Left.w);
            }
        }
        else
        {
            LeftPhantomDevice.force += Vector3.zero;
            HandPosition_Left *= UnitLength;
            LeftPhantomDevice.position = new Vector3(HandPosition_Left.x, HandPosition_Left.y, HandPosition_Left.z);
            LeftPhantomDevice.rotation = new Quaternion(HandRotation_Left.x, HandRotation_Left.y, HandRotation_Left.z, HandRotation_Left.w);
        }




        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        RightPhantomDevice.force = - 1.0f * LeftPhantomDevice.force;

        CurrentPosition2 = HandPosition_Right * UnitLength;
        float DopStiffness2 = FirstPlanePosition + 0.075f - CurrentPosition2.y;

        if (DopStiffness2 > 0)
        {
            Vector3 HandVelocity2 = Phantoms.GetVelocity();
            float Velocity2 = HandVelocity2.y;
            Velocity2 = Mathf.Clamp(Velocity2, -0.1f, 0.1f);

            ForceStiffness = (2.5f + SkinLayerStiffness) * DopStiffness2 + FirstLayerDamping * (-Velocity2) * DopStiffness2;

            ForceStiffness *= DEVICE_FORCE_SCALE;

            float membraneDamping = 0.003f;
            float membraneStiffness = 0.04f;
            float distanceCoeficient = 0.08f;
            float ClampValue = 0.4f;

            membraneForce2 = -membraneDamping * HandVelocity2;
            if (membraneForce2.magnitude > ClampValue)
            {
                membraneForce2.Normalize();
                membraneForce2 *= ClampValue;
            }

            Vector3 ForceS2 = Vector3.zero;
            ForceS2.x += membraneForce2.x - DopStiffness2 * distanceCoeficient;
            ForceS2.y += membraneForce2.y;
            ForceS2.z += membraneForce2.z - DopStiffness2 * distanceCoeficient;

            ClampValue = (float)Phantoms.GetContinuousForceLimit();
            membraneForce2 = membraneStiffness * (Vector3.zero - HandPosition_Right);
            if (membraneForce2.magnitude > ClampValue)
            {
                membraneForce2.Normalize();
                membraneForce2 *= ClampValue;
            }

            //ForceS.x += membraneForce2.x;
            ForceS2.x = 0;
            ForceS2.y += membraneForce2.y;
            //ForceS.z += membraneForce2.z;
            ForceS2.z = 0;
            RightPhantomDevice.force = ForceS2 - 1.0f * LeftPhantomDevice.force;
        }

        if (HandPosition_Right.y <= FirstPlanePosition)
        {
            float penetrationDistance = Mathf.Abs(HandPosition_Right.y);

            if (HandPosition_Right.y > SecondPlanePosition)
            {
                RightPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * FirstPlaneStiffness), 0);
                HandPosition_Right *= UnitLength;
                RightPhantomDevice.position = new Vector3(HandPosition_Right.x, HandPosition_Right.y, HandPosition_Right.z);
                RightPhantomDevice.rotation = new Quaternion(HandRotation_Right.x, HandRotation_Right.y, HandRotation_Right.z, HandRotation_Right.w);
            }
            else
            {
                RightPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * SecondPlaneStiffness), 0);
                HandPosition_Right *= UnitLength;
                RightPhantomDevice.position = new Vector3(HandPosition_Right.x, SecondPlanePosition * UnitLength, HandPosition_Right.z);
                RightPhantomDevice.rotation = new Quaternion(HandRotation_Right.x, HandRotation_Right.y, HandRotation_Right.z, HandRotation_Right.w);
            }
        }
        else
        {
            RightPhantomDevice.force += Vector3.zero;
            HandPosition_Right *= UnitLength;
            RightPhantomDevice.position = new Vector3(HandPosition_Right.x, HandPosition_Right.y, HandPosition_Right.z);
            RightPhantomDevice.rotation = new Quaternion(HandRotation_Right.x, HandRotation_Right.y, HandRotation_Right.z, HandRotation_Right.w);
        }

        HdAPI.hdMakeCurrentDevice(LeftPhantomDevice.hHdAPI);
        Phantoms.SetForce(LeftPhantomDevice.force);

        HdAPI.hdMakeCurrentDevice(RightPhantomDevice.hHdAPI);
        Phantoms.SetForce(RightPhantomDevice.force);

        HdAPI.hdEndFrame(RightPhantomDevice.hHdAPI);
        HdAPI.hdEndFrame(LeftPhantomDevice.hHdAPI);
        

        return true;

   
    }

    

    /// <summary>
    /// Auxiliar function to calculate attraction force between devices
    /// </summary>
    /// <param name="pos">position difference between devices</param>
    /// <returns>the force to be applied to the haptic devices</returns>
    private Vector3 ForceField (Vector3 pos)
    {
        float dist = pos.magnitude;
        int cpt = 0;

        Vector3 forceVec = Vector3.zero;

        // if two charges overlap...
        if (dist < 12 * 1000.0)
        {
            // Attract the charge to the center of the sphere.
            forceVec = new Vector3(-0.2f * pos.x, -0.2f * pos.y, -0.2f * pos.z);
            cpt++;
        }
        else
        {
            Vector3 unitPos = pos.normalized;
            forceVec = -1200.0f * unitPos / (dist * dist);
        }

        return forceVec;
    }

}
