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
    private string letzteAktion = "Idle00,00", eigeneLetzeAktion = "Idle00,00";

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


    GameObject maskottchen;

    static Animator animator;

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

        // Die letze Aktion wird über Photon zwischen den verschiedenen Spielern gesynct. 
        // Prüfen, ob sich die letzte Aktion von der vorletzten Aktion unterscheidet. Wenn ja, dann die letzte Aktion durchführen. 
        Debug.Log("Letzte Aktion: " + letzteAktion);
        Debug.Log("Eigene Letze: " + eigeneLetzeAktion);
        Debug.Log("Inactive Time: " + (inactiveTime>0));

        if(!(letzteAktion == eigeneLetzeAktion)){
            switch(letzteAktion.Remove(letzteAktion.Length - 5)){
                case "Sleep":
                    Sleep();
                    eigeneLetzeAktion = letzteAktion;
                    break;
                case "WakeUp":
                    WakeUp();
                    eigeneLetzeAktion = letzteAktion;
                    break;
                case "Feed":
                    Feed();
                    eigeneLetzeAktion = letzteAktion;
                    break;
                case "Pet":
                    Pet();
                    eigeneLetzeAktion = letzteAktion;
                    break;
            }
        }
    }
    #endregion

    #region Aktionen: Schlafen, Aufwachen, Füttern,  Kitzeln
    public void Sleep(){

        // Prüfen ob Maskottchen gerade Idle ist
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Catch") || animator.GetCurrentAnimatorStateInfo(0).IsName("Laugh"))
            return;
        
        photonView.RequestOwnership();

        // Wenn ja, Animation starten
        tired -= Time.deltaTime / 20;
        animator.SetTrigger("Sleep");
        
        letzteAktion = "Sleep00,00";

    }

    public void WakeUp(){
        
        // Prüfen, ob Maskottchen gerade schläft
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Sleep"))
            return;

        photonView.RequestOwnership();


        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;

        animator.SetTrigger("Idle");  
        letzteAktion = "WakeUp" + Time.time.ToString("00.00");
    }

    public void Feed(){

        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        photonView.RequestOwnership();


        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        hungry -= 0.3f;
        animator.SetTrigger("Catch");
        
        letzteAktion = "Feed" + Time.time.ToString("00.00");

    }

    public void Pet(){
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        photonView.RequestOwnership();


        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        unsatisfied -= 0.3f;
        animator.SetTrigger("Laugh");

        // Audio
        myAudioSource.clip = lauthingSound;
        myAudioSource.Play();

        letzteAktion = "Pet" + Time.time.ToString("00.00");
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
            // We own this player: send the others our data
            stream.SendNext(letzteAktion);
            stream.SendNext(hungry);
            stream.SendNext(unsatisfied);
            stream.SendNext(tired);
        }
        else if(PhotonNetwork.InRoom)
        {
            // Network player, receive data
            
            this.letzteAktion = (string)stream.ReceiveNext();
            hungry = (float)stream.ReceiveNext();
            unsatisfied = (float)stream.ReceiveNext();
            tired = (float)stream.ReceiveNext();
        }
    }


    #endregion

}
}
