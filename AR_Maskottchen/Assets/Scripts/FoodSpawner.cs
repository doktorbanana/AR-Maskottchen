using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject food;

    public void spawnFood()
    {
        //Transform spawnPosition = new Transform(this.transform.position);
        //spawnPosition.position = spawnPosition.position + new Vector3(0f,0f,0.2f);
        //Vector3 spawnVector = this.transform.position + new Vector3(0f, -0.1f, 0.3f);
        //Instantiate(food, spawnVector, Quaternion.identity);

        var camera = Camera.main.transform;
        Vector3 pos = camera.position + new Vector3(0f,-0.2f,0f);
        Instantiate(food, pos + camera.forward * 0.2f, Quaternion.identity);
    }
}
