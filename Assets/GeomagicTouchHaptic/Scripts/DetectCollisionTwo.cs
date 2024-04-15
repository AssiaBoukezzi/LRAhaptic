using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionTwo : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        // detect billet object collision
        if (collision.gameObject.tag == "BILLET")
        {

            //Vector3 collisionDirection = (transform.position - collision.contacts[0].point).normalized;
            Vector3 collisionDirection = new Vector3(0, 1, 0).normalized;

            // send constant force
            //TwoHapticsCoulombForceAttraction.instance.setForce(collisionDirection * 1.5f);


        }
        /*else if(collision.gameObject.tag == "CIBLE")
        {
            TwoHapticsCoulombForceAttraction.instance.state = 1;
        }
        else if (collision.gameObject.tag == "TRAJ")
        {
            TwoHapticsCoulombForceAttraction.instance.state = 0;
        }*/
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "BILLET")
        {
            // reset force to zero when collision exit
            //TwoHapticsCoulombForceAttraction.instance.setForce(Vector3.zero);
        }
        if (collision.gameObject.tag == "CIBLE")
        {
            // reset force to zero when collision exit
            //TwoHapticsCoulombForceAttraction.instance.state = 0;
        }
    }


}
