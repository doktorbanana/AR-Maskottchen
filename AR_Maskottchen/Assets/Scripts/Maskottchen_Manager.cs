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

    bool sleeping;
    
    [SerializeField]
    private AudioClip lauthingSound, spawnSound;
    
    [SerializeField]
    [Tooltip("Wie viele Sekunden muss der Spieler inaktiv sein, damit das Maskottchen einschläft?")]       
    float sleepTime = 20;
    private float inactiveTime = 0;

    [SerializeField]
    GameObject wakeUpButton;

    [SerializeField]
    Image satImg, hunImg, tirImg;

    [SerializeField]
    GameObject buttons, zustände, startanleitung;


    GameObject maskottchen;

    Animator animator;

    #endregion
    
    #region Unity Callbacks

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

        //Alle Zustände auf einen Bereich zwischen 0 und 1 beschränken
        hungry = Mathf.Clamp(hungry, 0, 1);
        unsatisfied = Mathf.Clamp(unsatisfied, 0, 1);
        tired = Mathf.Clamp(tired, 0, 1);
        
        //Inaktive Zeit messen
        inactiveTime += Time.deltaTime;       

        //Zustandicons anpassen
        SyncGUI();
    }
    #endregion

    #region Aktionen: Schlafen, Aufwachen, Füttern,  Kitzeln

    [PunRPC]
    public void Sleep(){

        // Prüfen ob Maskottchen gerade Idle ist
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Catch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Laugh"))
            return;
        sleeping = true;

        // Wenn ja, Animation starten
        tired -= Time.deltaTime / 20;
        animator.SetTrigger("Sleep");
        
    }

    [PunRPC] 
    public void WakeUp(){

        // Prüfen, ob Maskottchen gerade schläft
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Sleep"))
            return;
        
        sleeping = false;

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;

        animator.SetTrigger("Idle");  
    }

    [PunRPC]
    public void Feed(){

        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        hungry -= 0.2f;
        animator.SetTrigger("Catch");

        //Audio
        maskottchen.GetComponent<AudioSource>().clip = spawnSound;
        maskottchen.GetComponent<AudioSource>().Play();
        
    }

    [PunRPC]
    public void Pet(){
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        

        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        unsatisfied -= 0.3f;
        animator.SetTrigger("Laugh");

        // Audio
        maskottchen.GetComponent<AudioSource>().clip = lauthingSound;
        maskottchen.GetComponent<AudioSource>().Play();

    }

    #endregion

    #region Weitere Methoden
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

        if (stream.IsWriting && PhotonNetwork.InRoom)
        {
            // Wenn der Spieler der MasterClient ist, dann werden die Zustände an die anderen Spieler gesendet
            stream.SendNext(hungry);
            stream.SendNext(unsatisfied);
            stream.SendNext(tired);
            stream.SendNext(sleeping);
        }
        else if(PhotonNetwork.InRoom)
        {
            // Wenn der Spieler ein Netzwerk-Spieler ist, werden die Daten empfangen

            hungry = (float)stream.ReceiveNext();
            unsatisfied = (float)stream.ReceiveNext();
            tired = (float)stream.ReceiveNext();
            sleeping = (bool)stream.ReceiveNext();
        }
    }


    #endregion

}
}
