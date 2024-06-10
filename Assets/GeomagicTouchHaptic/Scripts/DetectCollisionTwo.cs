using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionTwo : MonoBehaviour
{
    public static int cpt = 0;
    bool exit = true;
    void OnTriggerEnter(Collider collision)
    {
        // detect billet object collision
        if (collision.gameObject.tag == "a" && exit)
        {

            cpt++;
            print("cpt = " + cpt);
            exit = false;

        }
        
    }
    void OnTriggerExit(Collider collision)
    {
        print("exit");
        if (collision.gameObject.tag == "a")
        {

            exit = true;

        }
    }


}
