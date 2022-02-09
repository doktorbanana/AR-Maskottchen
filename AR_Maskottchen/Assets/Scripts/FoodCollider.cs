using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;

public class FoodCollider : MonoBehaviour
{
    public Maskottchen_Manager maskottchen_ManagerScript;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Maskottchen"))
        {
            Debug.Log("Collision!");
            Destroy(gameObject);
            maskottchen_ManagerScript.Feed();
        }
    }
}
