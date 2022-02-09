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
    public static bool sleeping;

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

    public static Animator animator;

    #endregion
    
    #region Unity Callbacks

    void Start(){
        //Animator finden
        animator = GetComponent<Animator>();
    }

    void Update(){
        
        
        // Zustandswerte anpassen
        
        hungry += Time.deltaTime / 300;
        unsatisfied += Time.deltaTime / 300;
        tired += Time.deltaTime / 300;

        hungry = Mathf.Clamp(hungry, 0, 1);
        unsatisfied = Mathf.Clamp(unsatisfied, 0, 1);
        tired = Mathf.Clamp(tired, 0, 1);

        //Inaktive Zeit messen
        inactiveTime += Time.deltaTime;

        // Schlafen, wenn zu müde oder nichts passiert

        if(inactiveTime > sleepTime || tired > 1){
            sleeping = true;
        }

        if(sleeping){
            Sleep();
            wakeUpButton.SetActive(true);
        }else{
            wakeUpButton.SetActive(false);
        }

        //GUI anpasse
        SyncGUI();
    }
    #endregion

    #region Public Methods
    public void Sleep(){

        // Prüfen ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        // Wenn ja, Variable anpassen und Animation starten
        sleeping = true;
        tired -= Time.deltaTime;
        animator.SetTrigger("Sleep");
    }

    public static void WakeUp(){
        
        // Prüfen, ob Maskottchen gerade schläft
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Sleep"))
            return;
        
        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        sleeping = false;
        animator.SetTrigger("Idle");
    }

    public void Feed(){
        
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        hungry -= 0.3f;
        animator.SetTrigger("Pickup");
    }

    public void Pet(){
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        unsatisfied -= 0.3f;
        animator.SetTrigger("Laugh");
        myAudioSource.clip = lauthingSound;
        myAudioSource.Play();
    }

    void SyncGUI(){
        satImg.fillAmount = 1 - unsatisfied;
        tirImg.fillAmount = 1 - tired;
        hunImg.fillAmount = 1 - hungry;

    }
    #endregion


     #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(tired);
            stream.SendNext(hungry);
            stream.SendNext(unsatisfied);
            stream.SendNext(sleeping);
            stream.SendNext(inactiveTime);
        }
        else
        {
            // Network player, receive data
           tired = (float)stream.ReceiveNext();
           hungry = (float)stream.ReceiveNext();
           unsatisfied = (float)stream.ReceiveNext();
           sleeping = (bool)stream.ReceiveNext();
           inactiveTime = (float)stream.ReceiveNext();
        }
    }


    #endregion

}
}
