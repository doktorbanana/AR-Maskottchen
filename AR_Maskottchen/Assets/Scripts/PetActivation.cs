using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;

public class PetActivation : MonoBehaviour
{
    public Maskottchen_Manager maskottchen_ManagerScript;

    public AudioClip tapSound;
    public AudioSource myAudioSource;
    Camera cam;

    [SerializeField]
    private float maxTapTimeInterval = 1.0f;
    [SerializeField]
    private int triggerCount = 5;
    private float lastTapTime = 0.0f;
    private int tapCounter = 1;


    void Start()
    {
        maskottchen_ManagerScript = GetComponent<Maskottchen_Manager>();
    }
 

    void Update()
    {
 
        //nur wenn Maskottchen wach ist
        if (Input.touchCount > 0 && Maskottchen_Manager.sleeping == false)
        {
            TapPet();
        }

        //wenn oft genug getapt wurde, wird Pet() Animation getriggert
        if (tapCounter == triggerCount)
        {
            maskottchen_ManagerScript.Pet();
            Debug.Log("Pet Trigger");
            tapCounter = 1;
        }
    }


    void TapPet()
    {
        Touch touch = Input.GetTouch(0);

        myAudioSource.clip = tapSound;
        myAudioSource.Play();

        //prüfen, ob letzter Tap zu lange her ist
        //tapCounter startet mit 1, weil Wert erst beim 2. klicken ansteigt (tapCounter++)
        if (touch.phase == TouchPhase.Began && Time.time < lastTapTime + maxTapTimeInterval)
        {
            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            //checkt, ob Maskottchen getroffen wurde
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.rigidbody != null)
                {
                    tapCounter++;
                }
            }
        }
        else{
            tapCounter = 1;
        }

        lastTapTime = Time.time;
        Debug.Log(tapCounter + " " + lastTapTime);
    }
}

