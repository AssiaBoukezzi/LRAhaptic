using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionTwo : MonoBehaviour
{
    public static int cpt = 0;

    //bool pour savoir si ce point touche une cible (fichier.csv)
    public static int touch = 0;

    bool exit = true;
    void OnTriggerEnter(Collider collision)
    {
        // detect billet object collision
        if (collision.gameObject.tag == "a" && exit)
        {
            touch = 1;

            cpt++;
            print("cpt = " + cpt);
            exit = false;

        }
        
    }
    void OnTriggerExit(Collider collision)
    {
        touch = 0;
        print("exit");
        if (collision.gameObject.tag == "a")
        {

            exit = true;

        }
    }


}
