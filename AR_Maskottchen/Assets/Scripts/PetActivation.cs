using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maskottchen.Manager;
using Photon.Pun;
public class PetActivation : MonoBehaviour
{
    public Maskottchen_Manager maskottchen_ManagerScript;

    public AudioClip giggle1, giggle2, giggle3, giggle4;
    public AudioSource myAudioSource;
    public Collider coll;
    public Camera cam;
    public GameObject maskottchenmanager;

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
        maskottchenmanager = GameObject.FindWithTag("MaskottchenManager");
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
 

    void Update()
    {

        TapPetTouch();
        //TapPetMouse();

        //wenn oft genug getapt wurde, wird Pet() Animation getriggert
        /*if (tapCounter == triggerCount)
        {
            maskottchenmanager.GetComponent<PhotonView>().RPC("Pet", Photon.Pun.RpcTarget.All);
            Debug.Log("Pet Trigger");
            tapCounter = 1;
        }*/
    }


    void TapPetTouch()
    {
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            return;
        }
        
        //nur wenn Maskottchen wach ist
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            Ray ray = cam.ScreenPointToRay(touch.position);
            RaycastHit hit;

            //checkt, ob Maskottchen getroffen wurde
            if(coll.Raycast(ray, out hit, 100.0f))
            { 
                
                //prüfen, ob letzter Tap zu lange her ist
                //tapCounter startet mit 1, weil Wert erst beim 2. klicken ansteigt (tapCounter++)
                if(touch.phase == TouchPhase.Began/* && Time.time < lastTapTime + maxTapTimeInterval*/)
                {
                    //tapCounter++;
                    maskottchenmanager.GetComponent<PhotonView>().RPC("Pet", Photon.Pun.RpcTarget.All);
                }
                /*else{
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
                Debug.Log(tapCounter);*/
            }
        }
    }

    
    void TapPetMouse()
    {
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
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

