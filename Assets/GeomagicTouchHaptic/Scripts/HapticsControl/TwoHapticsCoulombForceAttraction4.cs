//---------------------------------------------------------------------------
// INCLUDES
//---------------------------------------------------------------------------
using UnityEngine;
using System;
using GeomagicTouchPhantom;
using System.Collections.Generic;
using System.IO;

//---------------------------------------------------------------------------
// HAPTIC MANAGER
//---------------------------------------------------------------------------

/// <summary>
/// Needle Insertion Prototype
/// </summary>
public class TwoHapticsCoulombForceAttraction4 : MonoBehaviour
{
    //---------------------------------------------------------------------------
    // CLASS INSTANCE
    //---------------------------------------------------------------------------

    /// <summary>
    /// class instance object -singleton-
    /// </summary>
	public static TwoHapticsCoulombForceAttraction4 instance;

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

    private LineRenderer line;
    private LineRenderer newLigne;
    private Vector3 previousPosition;
    public GameObject point;
    public GameObject LineRender;
    public Material materialLine;

    private Vector3 posOld;
    private Vector3 posAct;

    public GameObject trajectoir;
    private LineRenderer lineRend;

    public string fileName = "forces.csv"; // name file.csv

    private List<Vector3> points;

    /// <summary>
    /// Process at the start of the simulation
    /// </summary>

    public List<Vector3> trajectoryPositions = new List<Vector3>
    {
        new Vector3(1, 1, 1),
        new Vector3(2, 2, 2),
        new Vector3(3, 3, 3)
    };

    private void Start()
    {
        points = LoadCSVData();
        int p=0;
        foreach (var point in points)
        {
            //print(p+" les points sont : "+point);
            //p++;
        }    

        //StartCoroutine(FollowTrajectory());    
    }

    //public float moveDuration = 0.1f;

    

    List<Vector3> LoadCSVData()
    {
        List<Vector3> points = new List<Vector3>();

        string filePath = Path.Combine(Application.dataPath, fileName);

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                // Skip the header line
                reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] values = line.Split(';');
                    if (values.Length == 3)
                    {
                        if (float.TryParse(values[0], out float x) &&
                            float.TryParse(values[1], out float y) &&
                            float.TryParse(values[2], out float z))
                        {
                            points.Add(new Vector3(x, y, z));
                        }
                    }
                }
            }
        }
        else
        {
            print("File not found: " + filePath);
        }

        return points;
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
    public GameObject tip;
    public float control = 0.5f;
    bool visual = false;
    bool visual2 = false;
    bool createLine = false;


    public Camera myCamera;

    Vector3 speed = Vector3.zero;
    /// <summary>
    /// Process each frame
    /// </summary>
    private int i=0;
    int j=0;
    private void Update()
    {
        HdAPI.hdBeginFrame(LeftPhantomDevice.hHdAPI);
        HandPosition_Left = Phantoms.GetPosition();
        HandRotation_Left = Phantoms.GetRotation();
        
        LeftPhantomDevice.tool.transform.localPosition = LeftPhantomDevice.position;
        RightPhantomDevice.tool.transform.localPosition = RightPhantomDevice.position;
        LeftPhantomDevice.tool.transform.localRotation = LeftPhantomDevice.rotation;
        RightPhantomDevice.tool.transform.localRotation = RightPhantomDevice.rotation;

        

//print(j);
        if(j<points.Count)
        {
            //Vector3 pos_diff = new Vector3(LeftPhantomDevice.position.x - points[j].x, LeftPhantomDevice.position.y - points[j].y, LeftPhantomDevice.position.z - points[j].z)*199;
            //print(j +" force = "+ForceField(pos_diff, Vector3.zero));
            LeftPhantomDevice.force = points[j];
            //print(points[j] +" : " + HandPosition_Left);
            print(j +" force = " + LeftPhantomDevice.force);
            j++;
        }
        else{
            LeftPhantomDevice.force = Vector3.zero;
        }
        //print("force = " +LeftPhantomDevice.force);
        //LeftPhantomDevice.force += new Vector3(0.1f, 0.1f, 0.1f);
        

        Phantoms.SetForce(LeftPhantomDevice.force);
        HdAPI.hdEndFrame(LeftPhantomDevice.hHdAPI);
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
public float FirstPlanePosition = 4f;
private float FirstPlaneStiffness = 0.25f;
private float ForceStiffness;
private float SkinLayerStiffness = 31.5f;
private float FirstLayerDamping = 4f;
public float SkinLayerCutting = 1.22f;
public float FIRST_LAYER_TOP = 0.10f;
public float SecondPlanePosition = 3.0f;
public float SecondPlaneStiffness = 0.33f;
public float DEVICE_FORCE_SCALE = 0.4f;

private Vector3 membraneForce2;


private LineRenderer currentDrawing;
private int index;
private int currentColorIndex = 0;
public Material drawingMaterial;
public float penWidth = 0.01f;
public Color[] penColors;
private Buttons button1RightState;





    /// <summary>
    /// Method that is repeatedly called in PHANTOM's cycle (default rate 1 [kHz])
    /// </summary>
    /// <returns><c>true</c>, if update was phantomed, <c>false</c> otherwise.</returns>
    bool PhantomUpdate()
    {

        //PI);
        

        return true;        
   
    }

    public float gainAmort = -1f; // gaint et force parfait apres essaie
    public float force = 0.2f;
    

    /// <summary>
    /// Auxiliar function to calculate attraction force between devices
    /// </summary>
    /// <param name="pos">position difference between devices</param>
    /// <returns>the force to be applied to the haptic devices</returns>
    private Vector3 ForceField (Vector3 pos, Vector3 speed)
    {
        float dist = pos.magnitude;
        int cpt = 0;


        Vector3 forceVec = Vector3.zero;
        Vector3 forceVecUn = Vector3.zero;

        

        //speed = posAct - posOld;

        // if two charges overlap...
        if (dist < 12 * 1000.0)
        {
            //speedx = pos.x-posxOld
            // Attract the charge to the center of the sphere.
            //            forceVec = new Vector3(-0.2f * pos.x + gainAmort*speedx, -0.2f * pos.y + gainAmort * speedx, -0.2f * pos.z + gainAmort * speedx);
            forceVecUn = new Vector3(-force * pos.x, -force * pos.y, -force * pos.z);
            forceVec = forceVecUn + gainAmort * speed;
            cpt++;
            //posxOld = pos.x;
        }
        else
        {
            Vector3 unitPos = pos.normalized;
            forceVec = -1200.0f * unitPos / (dist * dist);
        }

        return forceVec;
    }

}
