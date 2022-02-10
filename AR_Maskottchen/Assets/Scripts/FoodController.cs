using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodController : MonoBehaviour
{

    private void Update()
    {
        var camera = Camera.main.transform;
        Vector3 pos = camera.position + new Vector3(0f, -0.1f, 0f) + camera.forward * 0.2f;
        this.transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Maskottchen"))
        {
            gameObject.SetActive(false);
        }
    }
}
