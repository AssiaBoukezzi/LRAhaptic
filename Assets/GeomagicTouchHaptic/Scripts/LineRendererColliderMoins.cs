using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererColliderMoins : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float colliderWidth = 0.1f;
    public float distance = 0.2f;

    void Start()
    {
        CreateColliders();
    }

    void CreateColliders()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        int numPoints = lineRenderer.positionCount;
        for (int i = 0; i < numPoints - 1; i=i+5)
        {
            Vector3 startPoint = lineRenderer.GetPosition(i) + new Vector3(-0.1f, 0, 0);
            Vector3 endPoint = lineRenderer.GetPosition(i + 1)+new Vector3(-0.1f, 0, 0);
            CreateColliderBetweenPoints(startPoint, endPoint);
        }
    }

    void CreateColliderBetweenPoints(Vector3 startPoint, Vector3 endPoint)
    {
        GameObject colliderObject = new GameObject("LineSegmentCollider");
        colliderObject.transform.position = (startPoint + endPoint) / 2;
        colliderObject.transform.rotation = Quaternion.LookRotation(startPoint - endPoint);
        colliderObject.transform.rotation = Quaternion.Euler(0,-90,0);
        
        //float distance = Vector3.Distance(startPoint, endPoint);
        CapsuleCollider collider = colliderObject.AddComponent<CapsuleCollider>();
        collider.isTrigger = true;
        collider.height = distance;
        collider.radius = colliderWidth / 2;
        collider.direction = 2; // Z-axis

        colliderObject.AddComponent<LineSegmentTrigger2>();
    }
}


public class LineSegmentTrigger2 : MonoBehaviour
{
    

    bool exit = false;
    public GameObject alert;

    void Start()
    {
        alert = GameObject.Find("Alerte");
    }

    void OnTriggerEnter(Collider collision)
    {
        print(collision.name);
        if(collision.gameObject.tag == "a")
        {
            print("limite");
            Renderer renderer = alert.GetComponent<Renderer>();
            renderer.material.color = Color.red;
            exit = true;
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if(collision.gameObject.tag == "a" && exit)
        {
            print("limite exit");
            Renderer renderer = alert.GetComponent<Renderer>();
            renderer.material.color = Color.green;
            exit = false;
        }
    }
}
