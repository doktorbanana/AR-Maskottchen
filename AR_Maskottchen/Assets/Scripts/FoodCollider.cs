using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;

public class FoodCollider : MonoBehaviour
{
    public Maskottchen_Manager maskottchen_ManagerScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Maskottchen"))
        {
            Debug.Log("Collision!");
            Destroy(gameObject);
            maskottchen_ManagerScript.Feed();
        }
    }
}
