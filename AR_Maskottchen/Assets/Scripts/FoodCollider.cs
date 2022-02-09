using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodCollider : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Leos_Mascot")
        {
            Debug.Log("Collision!");
        }
    }
}
