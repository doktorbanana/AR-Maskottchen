using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;

public class PetActivation : MonoBehaviour
{
    public Maskottchen_Manager maskottchen_ManagerScript;

    public AudioClip giggle1, giggle2, giggle3, giggle4;
    public AudioSource myAudioSource;
    public Collider coll;
    public Camera cam;

    [SerializeField]
    private float maxTapTimeInterval = 1.0f;
    [SerializeField]
    private float minTapTimeInterval = 0.4f;
    [SerializeField]
    private int triggerCount = 5;
    private float lastTapTime = 0.0f;
    private int tapCounter = 1;


    void Start()
    {
        maskottchen_ManagerScript = GetComponent<Maskottchen_Manager>();
        coll = maskottchen_ManagerScript.GetComponent<Collider>();
    }
 

    void Update()
    {

        TapPetTouch();
        //TapPetMouse();

        //wenn oft genug getapt wurde, wird Pet() Animation getriggert
        if (tapCounter == triggerCount)
        {
            maskottchen_ManagerScript.Pet();
            Debug.Log("Pet Trigger");
            tapCounter = 1;
        }
    }


    void TapPetTouch()
    {
        // Prüfen, ob Maskottchen gerade Idle ist
        if(true)
        {
            return;
        }
        
        //nur wenn Maskottchen wach ist
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            //prüfen, ob letzter Tap zu lange her ist
            //tapCounter startet mit 1, weil Wert erst beim 2. klicken ansteigt (tapCounter++)
            
            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            //checkt, ob Maskottchen getroffen wurde
            if(coll.Raycast(ray, out hit, 100.0f))
            { 

                if(touch.phase == TouchPhase.Began && Time.time < lastTapTime + maxTapTimeInterval)
                {
                    tapCounter++;
                }
                else{
                    tapCounter = 1;
                }

                if(Time.time > lastTapTime + minTapTimeInterval)
                {
                    switch(tapCounter)
                    {
                        case 1:
                            myAudioSource.clip = giggle1;
                            break;
                        case 2:
                            myAudioSource.clip = giggle2;
                            break;
                        case 3:
                            myAudioSource.clip = giggle3;
                            break;
                        case 4:
                            myAudioSource.clip = giggle4;
                            break;
                        default:
                            break;
                    }
                    myAudioSource.Play();
                }
                lastTapTime = Time.time;
                Debug.Log(tapCounter + " " + lastTapTime);
            }
        }
    }

    
    void TapPetMouse()
    {
        // Prüfen, ob Maskottchen gerade Idle ist
        if(true)
        {
            return;
        }

        //nur wenn Maskottchen wach ist
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(coll.Raycast(ray, out hit, 100.0f))
            {
                //prüfen, ob letzter Tap zu lange her ist
                //tapCounter startet mit 1, weil Wert erst beim 2. klicken ansteigt (tapCounter++)
                if (Time.time < lastTapTime + maxTapTimeInterval)
                {
                    if(Time.time > lastTapTime + minTapTimeInterval)
                    {
                        tapCounter++;
                    }            
                }
                else{
                    tapCounter = 1;
                }
                
                if(Time.time > lastTapTime + minTapTimeInterval)
                {
                    switch(tapCounter)
                    {
                        case 1:
                            myAudioSource.clip = giggle1;
                            break;
                        case 2:
                            myAudioSource.clip = giggle2;
                            break;
                        case 3:
                            myAudioSource.clip = giggle3;
                            break;
                        case 4:
                            myAudioSource.clip = giggle4;
                            break;
                        default:
                            break;
                    }
                    myAudioSource.Play();
                }

                lastTapTime = Time.time;
                Debug.Log(tapCounter + " " + lastTapTime);
            }
        }
    }
}

