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
public class TwoHapticsCoulombForceAttraction3 : MonoBehaviour
{
    //---------------------------------------------------------------------------
    // CLASS INSTANCE
    //---------------------------------------------------------------------------

    /// <summary>
    /// class instance object -singleton-
    /// </summary>
	public static TwoHapticsCoulombForceAttraction3 instance;

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

    private Vector3 posActUnity;
    private Vector3 posOldUnity;


    //public GameObject trajectoir;
    private LineRenderer lineRend;
    private LineRenderer lineRendTrajLimit;
    public string fileName = "lineRendererData.csv"; // name file.csv
    public string fileName2 = "lineRendererDataPlus.csv"; // ligne a droite
    public string fileName3 = "lineRendererDataMoins.csv"; // ligne a gauche

    //public GameObject trajPlus; 
    //public GameObject trajMoins;


    /// <summary>
    /// Process at the start of the simulation
    /// </summary>
    private void Start()
    {

        /*lineRend = trajectoir.GetComponent<LineRenderer>(); // recuperer les points de la trajectoire optimale
        SaveLineRendererDataToCSV(); // la sauvegarder dans une fichier CSV

        lineRendTrajLimit = trajPlus.GetComponent<LineRenderer>();
        List<Vector3> points = LoadCSVData(fileName2);
        lineRendTrajLimit.positionCount = points.Count;
        lineRendTrajLimit.SetPositions(points.ToArray());

        lineRendTrajLimit = trajMoins.GetComponent<LineRenderer>();
        points = LoadCSVData(fileName3);
        lineRendTrajLimit.positionCount = points.Count;
        lineRendTrajLimit.SetPositions(points.ToArray());*/

        points = LoadCSVData("forces.csv");

        
        posAct = HandPosition_Left;
        line = point.GetComponent<LineRenderer>();
        line.positionCount = 0;
        previousPosition = point.transform.position;
        newLigne = line;
        newLigne.material = materialLine;
        newLigne.transform.localScale = new Vector3(newLigne.transform.localScale.x, 0f, newLigne.transform.localScale.z);
    }

    List<Vector3> LoadCSVData(string file)
    {
        List<Vector3> points = new List<Vector3>();

        string filePath = Path.Combine(Application.dataPath, file);

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

    void SaveLineRendererDataToCSV()
    {
        if (lineRend == null)
        {
            Debug.LogError("LineRenderer not assigned.");
            return;
        }

        int numPoints = lineRend.positionCount;
        Vector3[] points = new Vector3[numPoints];
        lineRend.GetPositions(points);

        using (StreamWriter writer = new StreamWriter(fileName))
        {
            writer.WriteLine("x,y,z");  // Header
            foreach (Vector3 point in points)
            {
                string line = string.Format("{0};{1};{2}", point.x, point.y, point.z);
                writer.WriteLine(line);
            }
            Debug.Log("Line Renderer data saved to " + fileName);
        }
        using (StreamWriter writer = new StreamWriter(fileName2))
        {
            writer.WriteLine("x,y,z");  // Header
            foreach (Vector3 point in points)
            {
                string line = string.Format("{0};{1};{2}", point.x+0.06f, point.y, point.z);
                writer.WriteLine(line);
            }
            Debug.Log("Line Renderer data saved to " + fileName2);
        }
        using (StreamWriter writer = new StreamWriter(fileName3))
        {
            writer.WriteLine("x,y,z");  // Header
            foreach (Vector3 point in points)
            {
                string line = string.Format("{0};{1};{2}", point.x-0.06f, point.y, point.z);
                writer.WriteLine(line);
            }
            Debug.Log("Line Renderer data saved to " + fileName3);
        }

        
    }

    public string ConvertVector3ListToCSV(List<Vector3> list)
    {
        if (list == null || list.Count == 0)
        {
            return string.Empty;
        }

        var csvStringBuilder = new System.Text.StringBuilder();

        // Ajouter les en-têtes de colonnes
        csvStringBuilder.AppendLine("X;Y;Z");

        // Ajouter les données
        foreach (var vector in list)
        {
            csvStringBuilder.AppendFormat("{0};{1};{2}", vector.x, vector.y, vector.z).AppendLine();
        }

        return csvStringBuilder.ToString();
    }

    public void SaveCSVToFile(string csv, string filePath)
    {
        System.IO.File.WriteAllText(filePath, csv);
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
    Vector3 speedunity = Vector3.zero;

    List<Vector3> leftPos = new List<Vector3>();
    int k=0;

    private bool inputA = false;


    /// <summary>
    /// Process each frame
    /// </summary>
    private void Update()
    {
        

        LeftPhantomDevice.tool.transform.localPosition = LeftPhantomDevice.position;
        RightPhantomDevice.tool.transform.localPosition = RightPhantomDevice.position;
        LeftPhantomDevice.tool.transform.localRotation = LeftPhantomDevice.rotation;
        RightPhantomDevice.tool.transform.localRotation = RightPhantomDevice.rotation;

        

        Vector3 pos = new Vector3(tip.transform.position.x, -0.05f, tip.transform.position.z);
        
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

        
        
        if (button1RightState == Buttons.Button1 || button1RightState == Buttons.Button2)
        {
            myCamera.cullingMask |= (1 << LayerMask.NameToLayer("TransparentFX"));
        }
        else
        {
            myCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("TransparentFX"));
        }

        if (Input.GetKey(KeyCode.V))
        {
            //visual = true;
            myCamera.cullingMask |= (1 << LayerMask.NameToLayer("TransparentFX"));
        }

        Vector3 currentPosition = pos;

        

        if (tip.transform.position.y <= -0.048f && tip.transform.position.x > -0.576f && tip.transform.position.x < 0.4f && tip.transform.position.z > -0.276 && tip.transform.position.z < 0.28f)
        {
            //GameObject newObject = Instantiate(prefab, pos, Quaternion.identity);
            //leftPos.Add(LeftPhantomDevice.position);
            //print("k = "+k+"count = "+ leftPos.Count);
            /*if(k<leftPos.Count);
            {
                //print(k + " leftPos = " + leftPos[k]);
                k++;
            }*/

            //currentPosition.z = 0f;

            if (previousPosition == transform.position)
            {
                newLigne.SetPosition(0, currentPosition);
            }
            else
            {
                newLigne.positionCount++;
                newLigne.SetPosition(newLigne.positionCount - 1, currentPosition);
            }


            previousPosition = currentPosition;
            createLine = true;
        }
        else
        {
            if (createLine)
            {
                //string csv = ConvertVector3ListToCSV(leftPos);
                //string filePath = Application.dataPath + "/forces.csv";
                //SaveCSVToFile(csv, filePath);
                //Debug.Log("CSV file saved to: " + filePath);

                GameObject lineObject = new GameObject("NewLineRenderer");
                LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

                lineRenderer.positionCount = 0; // Nombre de points de la ligne
                lineRenderer.startWidth = 0.005f; // Largeur de début de la ligne
                lineRenderer.endWidth = 0.005f; // Largeur de fin de la ligne
                previousPosition = point.transform.position;
                newLigne = lineRenderer;
                newLigne.material = materialLine;
                newLigne.numCornerVertices = 90;
                newLigne.numCapVertices = 90;
                newLigne.transform.localScale = new Vector3(newLigne.transform.localScale.x, 0f, newLigne.transform.localScale.z);
                createLine = false;
            }

        }

        if (visual == true || visual2 == true)
        {
            if (control == 0)
            {
                co_embody.transform.position = LeftPhantomDevice.position;
                co_embody.transform.rotation = HandRotation_Left;

            }
            else if (control == 1)
            {
                co_embody.transform.position = RightPhantomDevice.position;
                co_embody.transform.rotation = HandRotation_Right;
            }
            else
            {
                co_embody.transform.position = xyz;
                co_embody.transform.rotation = HandRotation_Left;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            posActUnity = LeftPhantomDevice.position;
            posOldUnity = LeftPhantomDevice.position;
            startTime = DateTime.Now;
            inputA = true;
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
public float FirstPlanePosition = 4f;
public float FirstPlaneStiffness = 0.25f;
private float ForceStiffness;
public float SkinLayerStiffness = 31.5f;
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


int i=0;




    /// <summary>
    /// Method that is repeatedly called in PHANTOM's cycle (default rate 1 [kHz])
    /// </summary>
    /// <returns><c>true</c>, if update was phantomed, <c>false</c> otherwise.</returns>
    bool PhantomUpdate()
    {

        
        HdAPI.hdBeginFrame(LeftPhantomDevice.hHdAPI);
        HandPosition_Left = Phantoms.GetPosition();
        HandRotation_Left = Phantoms.GetRotation();

        Buttons button1LeftState = Phantoms.GetButton();
        Buttons button2LeftState = Phantoms.GetButton();

        HdAPI.hdBeginFrame(RightPhantomDevice.hHdAPI);
        HandPosition_Right = Phantoms.GetPosition();
        HandRotation_Right = Phantoms.GetRotation();

        button1RightState = Phantoms.GetButton();

        Vector3 pos_diff = new Vector3(HandPosition_Left.x - HandPosition_Right.x, HandPosition_Left.y - HandPosition_Right.y, HandPosition_Left.z - HandPosition_Right.z);
        posOld = posAct;
        posAct = HandPosition_Left;

        speed = posAct - posOld;
        if (button1RightState == Buttons.Button2)
        {
        
            LeftPhantomDevice.force = ForceField(pos_diff, speed);
            //leftPos.Add(LeftPhantomDevice.force);
            //print(" force = "+ LeftPhantomDevice.force);
        }
        


        /*
        if(visual == false)
        {
            LeftPhantomDevice.force = ForceField(pos_diff);
        }
        */

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

            
            if (button1RightState == Buttons.Button2)
            {
                LeftPhantomDevice.force = ForceS + ForceField(pos_diff, speed);
            }
            else
            {
                LeftPhantomDevice.force = ForceS;
            }

            //LeftPhantomDevice.force = ForceS;

            /*
            if (visual == false)
            {
                LeftPhantomDevice.force = ForceS + ForceField(pos_diff);
            }
            */

        }

        if (HandPosition_Left.y <= FirstPlanePosition)
        {
            float penetrationDistance = Mathf.Abs(HandPosition_Left.y);

            if (HandPosition_Left.y > SecondPlanePosition)
            {
                LeftPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * FirstPlaneStiffness), 0);
                HandPosition_Left *= UnitLength;
                LeftPhantomDevice.position = new Vector3(HandPosition_Left.x, HandPosition_Left.y, HandPosition_Left.z);
                LeftPhantomDevice.rotation = new Quaternion(HandRotation_Left.x, HandRotation_Left.y, HandRotation_Left.z, HandRotation_Left.w);
            }
            else
            {
                LeftPhantomDevice.force += new Vector3(0, (float)(penetrationDistance * SecondPlaneStiffness), 0)*10;
                HandPosition_Left *= UnitLength;
                LeftPhantomDevice.position = new Vector3(HandPosition_Left.x, SecondPlanePosition * UnitLength , HandPosition_Left.z);
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

        if (button1RightState == Buttons.Button1)
        {
            RightPhantomDevice.force = -1.0f * LeftPhantomDevice.force;
        }

        /*
        if(visual == false)
        {
            RightPhantomDevice.force = -1.0f * LeftPhantomDevice.force;
        }
        */
            //RightPhantomDevice.force = - 1.0f * LeftPhantomDevice.force;

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
            if (button1RightState == Buttons.Button1)
            {
                RightPhantomDevice.force = ForceS2 - 1.0f * LeftPhantomDevice.force;
            }
            else
                RightPhantomDevice.force = ForceS2;

            /*
            if(visual == false)
            {
                RightPhantomDevice.force = ForceS2 - 1.0f * LeftPhantomDevice.force;
            }
            */

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
                RightPhantomDevice.position = new Vector3(HandPosition_Right.x, SecondPlanePosition * 0.01f, HandPosition_Right.z);
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

        //Time = DateTime.Now - startTime;
        //float secondsElapsed = (float)Time.TotalSeconds;
        //print(secondsElapsed.ToString());
        //print("force = "+LeftPhantomDevice.force);

        

        if (inputA)
        {
            posOldUnity = posActUnity;
            posActUnity = LeftPhantomDevice.position;

            speedunity = posActUnity - posOldUnity;
            if (lineList < points.Count -4)
            {
                Time = (float)(DateTime.Now - startTime).TotalSeconds;
                //print("time = " +Time.ToString());
                lineList = (int)(Time / 0.03846f);
                desiredPosition = points[lineList];
                positionDiff = new Vector3((LeftPhantomDevice.position.x - desiredPosition.x)*1000, (LeftPhantomDevice.position.y - desiredPosition.y)*1000, (LeftPhantomDevice.position.z - desiredPosition.z)*1000);
                //positionDiff = (LeftPhantomDevice.position - desiredPosition)*1000;
                LeftPhantomDevice.force = ForceField(positionDiff, speedunity);
                print("ligne = "+lineList + " force = " + LeftPhantomDevice.force);

                //inputA = false;
            }
            else
            {
                inputA = false;
                LeftPhantomDevice.force = Vector3.zero;
            }
        }

        
        
        

        HdAPI.hdMakeCurrentDevice(LeftPhantomDevice.hHdAPI);
        Phantoms.SetForce(LeftPhantomDevice.force);

        HdAPI.hdMakeCurrentDevice(RightPhantomDevice.hHdAPI);
        Phantoms.SetForce(RightPhantomDevice.force);

        HdAPI.hdEndFrame(RightPhantomDevice.hHdAPI);
        HdAPI.hdEndFrame(LeftPhantomDevice.hHdAPI);
        

        return true;

   
    }

    private List<Vector3> points;
    private Vector3 desiredPosition;
    private Vector3 positionDiff = Vector3.zero;


    private int lineList=0;
    public DateTime startTime;
    public float Time;

    

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
