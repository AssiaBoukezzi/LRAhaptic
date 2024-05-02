using UnityEngine;

public class ScaleChange : MonoBehaviour
{

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
    bool max = false;

    void Update()
    {
        // Vérifie si la touche "Espace" est pressée.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(transform.localScale.y - 0.01 <= 0)
            {
                ChangeScaleZero();
            }
            else if (transform.localScale.y < 0.2016675f && max == false )
            {
                // Change l'échelle du GameObject pour le doubler en taille.
                ChangeScalePlus(new Vector3(0.005f, 0.005f, 0.0f));

            }
            else
            {
                max = true;
                ChangeScaleMoins(new Vector3(0.005f, 0.005f, 0.0f));
            }
        }
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Change l'échelle du GameObject pour le doubler en taille.
            ChangeScaleMoins(new Vector3(0.01f, 0.02f, 0.0f));
        }
    }
}