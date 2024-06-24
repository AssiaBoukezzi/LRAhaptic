using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instruction : MonoBehaviour
{
    private bool coroutineAllowed;
    private float speedModifier;
    private float tparam;
    private Vector3 position;
    void start ()
    {
        coroutineAllowed=true;
        speedModifier=0.5f;
        tparam= 0f;
    }
    void update()
    {
        
        if (coroutineAllowed)
        {
            StartCoroutine(BezierFollow());
        }
        
    }

    private IEnumerator BezierFollow()
    {
        coroutineAllowed = false;
       
        while (tparam < 1)
        {
            tparam += Time.deltaTime * speedModifier;

            // Equations from: https://en.wikipedia.org/wiki/B%C3%A9zier_curve

            position = Mathf.Pow(1 - tparam, 3) * new Vector3(0,0,0) + 3 * Mathf.Pow(1 - tparam, 2) * tparam * new Vector3(0,0.02f,0) + 3 * (1 - tparam) * Mathf.Pow(tparam, 2) * new Vector3(0.04f,0.02f,0) + Mathf.Pow(tparam, 3) * new Vector3(0.04f,0,0);

            transform.position = position;
           
            yield return null;
        }
        coroutineAllowed = true;
        //currentEndPosition = transform.position;
    }
}
