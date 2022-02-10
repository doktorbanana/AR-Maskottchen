using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FoodController : MonoBehaviour
{
    public GameObject maskottchenmanager;
    public GameObject maskottchen;

    [SerializeField]
    private AudioClip eatingSound;

    private void Start()
    {
        maskottchenmanager = GameObject.FindWithTag("MaskottchenManager");
        maskottchen = GameObject.FindWithTag("Maskottchen");
    }


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

            //Animation
            maskottchenmanager.GetComponent<PhotonView>().RPC("Feed", Photon.Pun.RpcTarget.All);

            //Audio essen
            maskottchen.GetComponent<AudioSource>().clip = eatingSound;
            maskottchen.GetComponent<AudioSource>().Play();
        }
    }
}
