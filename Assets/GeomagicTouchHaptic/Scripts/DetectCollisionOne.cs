using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionOne : MonoBehaviour
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
            print(2);
            Vector3 collisionDirection = (transform.position - collision.contacts[0].point).normalized;

            // send constant force
            OneHapticMovementOnly.instance.setForce(collisionDirection * 2.0f);

        }

    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "BILLET")
        {
            // reset force to zero when collision exit
            OneHapticMovementOnly.instance.setForce(Vector3.zero);
        }
    }


}
