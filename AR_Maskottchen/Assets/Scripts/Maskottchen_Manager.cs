using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Maskottchen.Manager{
public class Maskottchen_Manager : MonoBehaviourPunCallbacks
{
    #region Variables
    public static float hungry, unsatisfied;
    public static bool sleeping;
    
    [SerializeField]
    [Tooltip("Wie viele Sekunden muss der Spieler inaktiv sein, damit das Maskottchen einschläft?")]        float sleepTime = 20;
    private static float inactiveTime = 0;

    [SerializeField]
    GameObject wakeUpButton;

    private static Animator animator;

    
    private FirebaseDBManager firebaseDBManager;

    #endregion
    
    #region Unity Callbacks

    void Start(){
        //Animator finden
        animator = GetComponent<Animator>();
        firebaseDBManager = GameObject.FindGameObjectWithTag("FirebaseDBManager").GetComponent<FirebaseDBManager>();
    }

    void Update(){
        
        //Inaktive Zeit messen
        inactiveTime += Time.deltaTime;

        if(inactiveTime > sleepTime){
            sleeping = true;
        }

        if(sleeping){
            Sleep();
            wakeUpButton.SetActive(true);
        }else{
            wakeUpButton.SetActive(false);
        }
        hungry += Time.deltaTime;
        unsatisfied += Time.deltaTime;

    }
    #endregion

    #region Public Methods
    public void Sleep(){

        // Prüfen ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        // Wenn ja, Variable anpassen und Animation starten
        sleeping = true;
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
        hungry = 0;
        animator.SetTrigger("Catch");

        firebaseDBManager.UpdateGameState(new GameState(103));
    }

    public void Pet(){
        // Prüfen, ob Maskottchen gerade Idle ist
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            return;
        
        // Wenn ja, Variablen anpassen und Animation starten
        inactiveTime = 0;
        unsatisfied = 0;
        animator.SetTrigger("Laugh");
    }
    #endregion

}
}
