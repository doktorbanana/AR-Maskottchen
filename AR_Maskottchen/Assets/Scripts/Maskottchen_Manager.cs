using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace Maskottchen.Manager{
public class Maskottchen_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    #region Variables
    public static float hungry, unsatisfied, tired;

    public static bool sleeping = false, feeding = false, petting = false, wakingUp = false;

    bool coroutineRunning = false;

    [SerializeField]
    private AudioClip lauthingSound;
    public AudioSource myAudioSource;
    
    [SerializeField]
    [Tooltip("Wie viele Sekunden muss der Spieler inaktiv sein, damit das Maskottchen einschläft?")]        float sleepTime = 20;
    private static float inactiveTime = 0;

    [SerializeField]
    GameObject wakeUpButton;

    [SerializeField]
    Image satImg, hunImg, tirImg;

    [SerializeField]
    GameObject buttons, zustände, startanleitung;

    public static PhotonView phoView;

    GameObject maskottchen;

    public static Animator animator;

    #endregion
    
    #region Unity Callbacks

    void Start(){
        phoView = GetComponent<PhotonView>();
    }
    void Update(){
        
        // Masskottchen in der Szene finden
        if(!maskottchen){
            FindMaskottchen();
            buttons.SetActive(false);
            zustände.SetActive(false);
            startanleitung.SetActive(true);
            return;
        }

                    
        // GUI anpassen
        buttons.SetActive(true);
        zustände.SetActive(true);
        startanleitung.SetActive(false);
        
        // Zustandswerte anpassen
        hungry += Time.deltaTime / 60;
        unsatisfied += Time.deltaTime / 60;

        hungry = Mathf.Clamp(hungry, 0, 1);
        unsatisfied = Mathf.Clamp(unsatisfied, 0, 1);
        tired = Mathf.Clamp(tired, 0, 1);

        //Inaktive Zeit messen
        inactiveTime += Time.deltaTime;

        // Schlafen, wenn zu müde oder nichts passiert
        if(inactiveTime > sleepTime || tired >= 1){
            sleeping = true;
        }

        if(sleeping){
            Sleep();
            wakeUpButton.SetActive(true);
        }else{
            wakeUpButton.SetActive(false);
            tired += Time.deltaTime / 60;
        }

        // Aktionen ausführen, wenn sich die entsprechenden bools ändern
        if(feeding && !coroutineRunning) StartCoroutine(Feed());
        if(petting && !coroutineRunning) StartCoroutine(Pet());
        if(wakingUp && !coroutineRunning) WakeUp();

        //GUI anpasse
        SyncGUI();
    }
    #endregion

    #region Public Methods
    public void Sleep(){

        // Prüfen ob Maskottchen gerade Idle ist
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Catch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Laugh"))
            return;

        sleeping = true;

        // Ownership über dieses Objekt übernehmen, damit Daten geschrieben werden können
        photonView.RequestOwnership();

        // Wenn ja, Animation starten
        tired -= Time.deltaTime / 20;
        animator.SetTrigger("Sleep");
    }

    public static void WakeUp(){
        
        // Prüfen, ob Maskottchen gerade schläft
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Sleep"))
            return;
        
        wakingUp = true;

        // Ownership über dieses Objekt übernehmen, damit Daten geschrieben werden können
        phoView.RequestOwnership();

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        sleeping = false;
        animator.SetTrigger("Idle");

        wakingUp = false;
    }

    public IEnumerator Feed(){

        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            yield return null;
                
        feeding = true;
        coroutineRunning = true;


       // Ownership über dieses Objekt übernehmen, damit Daten geschrieben werden können
        phoView.RequestOwnership();

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        hungry -= 0.3f;

        animator.SetTrigger("Catch");
        
        // Auf Ende der Animation warten
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        // Essen wieder freigeben
        feeding = false;
        coroutineRunning = false;
    }

    public IEnumerator Pet(){
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            yield return null;
        
        petting = true;
        
        coroutineRunning = true;

        // Ownership über dieses Objekt übernehmen, damit Daten geschrieben werden können
        phoView.RequestOwnership();

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        unsatisfied -= 0.3f;
        animator.SetTrigger("Laugh");

        myAudioSource.clip = lauthingSound;
        myAudioSource.Play();


        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        //Streicheln wieder freigeben
        petting = false;  

        coroutineRunning = false;
    }

    void SyncGUI(){
        satImg.fillAmount = 1 - unsatisfied;
        tirImg.fillAmount = 1 - tired;
        hunImg.fillAmount = 1 - hungry;

    }

    void FindMaskottchen(){

        //Maskottchen finden
        maskottchen = GameObject.FindGameObjectWithTag("Maskottchen");
        
        if(maskottchen){
            Debug.Log("Maskottchen gefunden: " + maskottchen.name);

            //Animator finden
            animator = maskottchen.GetComponent<Animator>();
        }   
    }

    #endregion


     #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(feeding);
            stream.SendNext(petting);
            stream.SendNext(wakingUp);
            stream.SendNext(sleeping);
            stream.SendNext(hungry);
            stream.SendNext(unsatisfied);
            stream.SendNext(tired);
        }
        else
        {
            // Network player, receive data
           feeding = (bool)stream.ReceiveNext();
           petting = (bool)stream.ReceiveNext();
           wakingUp = (bool)stream.ReceiveNext();
           sleeping = (bool)stream.ReceiveNext();
           hungry = (float)stream.ReceiveNext();
           unsatisfied = (float)stream.ReceiveNext();
           tired = (float)stream.ReceiveNext();
        }
    }


    #endregion

}
}
