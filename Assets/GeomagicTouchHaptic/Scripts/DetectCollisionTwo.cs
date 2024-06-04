using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionTwo : MonoBehaviour
{
    public static int cpt = 0;
    bool exit = true;
    void OnCollisionEnter(Collision collision)
    {
        // detect billet object collision
        if (collision.gameObject.tag == "BILLET" && exit)
        {

            cpt++;
            print("cpt = " + cpt);
            exit = false;

        }
        
    }
    void OnCollisionExit(Collision collision)
    {
        print("exit");
        if (collision.gameObject.tag == "BILLET")
        {

            exit = true;

        }
    }


}
