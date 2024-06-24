using UnityEngine;

public class ScaleChange : MonoBehaviour
{
    public GameObject leftHand;

    // Cette fonction permet de changer l'échelle du GameObject à une échelle spécifiée à tout moment.
    public void ChangeScaleMoins(Vector3 scale)
    {
        transform.localScale -= scale;
    }

    public void ChangeScalePlus(Vector3 scale)
    {
        transform.localScale += scale;
    }

    public void ChangeScaleZero()
    {
        transform.localScale = Vector3.zero;
    }

    private bool max;
    //bool min = false;
    private Vector3 position;
    //float pos_init;
    //float z= 0.0f;
    private Vector3 taille;
    //Vector3 diff = new Vector3(0.005f, 0.005f, 0.0f);
    private int cpt;
    //bool plusMax = false;
    private float debut;
    private float fin;
    private int cpt2;
    private int cpt3;


    private float posInit;
    private float scaleInit;
    private float posMin;
    private float posMax;
    private float scaleInitX;
    public float scaleMax = 0.09f;

    private void Start()
    {
        posInit = leftHand.transform.position.x;
        scaleInitX = transform.localScale.x;
        scaleInit = transform.localScale.z;
        posMin = posInit - scaleInit;
        posMax = posMin + 2 * scaleMax;
    }

    void Update()
    {
        if (leftHand.transform.position.x > posMin && leftHand.transform.position.x < posMax)
        {
            if(leftHand.transform.position.x < posMin + scaleMax)
            {
                transform.localScale = new Vector3(leftHand.transform.position.x - posMin + scaleInitX, 0.0005f, leftHand.transform.position.x - posMin);
            }
            else
            {
                transform.localScale = new Vector3(-leftHand.transform.position.x + posMax + scaleInitX, 0.0005f, -leftHand.transform.position.x + posMax);
            }
        }
        else
        {
            transform.localScale = Vector3.zero;
        }
    }
}