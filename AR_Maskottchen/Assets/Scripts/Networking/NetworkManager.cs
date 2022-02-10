using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Maskottchen.Manager;
using Maskottchen.Database;

namespace Maskottchen.Networking{
public class NetworkManager : MonoBehaviourPunCallbacks
{

    #region Public Variables
    public GameObject playerPrefab;

    #endregion

    #region Private Variables

    [SerializeField]
    private FirebaseDBManager firebaseDBManager;

    #endregion

    #region Unity Callbacks

    void Start()
    {
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(Random.Range(-1,1),0,-5), Quaternion.identity, 0);
    }

    void OnApplicationQuit()
    {
        // Zustand des Maskottchens  speichern
        SaveGame();
    }

    void OnApplicationFocus(bool focusState){
        if(!focusState) SaveGame();

    }

    void OnApplicationPause(bool pauseState){
        if(pauseState) SaveGame();
    }

    #endregion

    #region PhotonCallbacks
     
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player other)
    {

        //Level Laden
        if (PhotonNetwork.IsMasterClient)
        {
            Load_Main_Scene();
        }
    }


public override void OnPlayerLeftRoom(Player other)
{

    if (PhotonNetwork.IsMasterClient)
    {
        Load_Main_Scene();
    }
    
}


    
    #endregion

    #region Public Methods


    public void LeaveRoom()
    {   
        //Zustände des Maskottchens speichern
        SaveGame();
        
        //Raum verlassen
        PhotonNetwork.LeaveRoom();
    }


    #endregion

    #region Private Methods

    void Load_Main_Scene()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Kann Szene nicht laden, da nicht der Master Client");
        }
        Debug.LogFormat("PhotonNetwork : Szene wird geladen.");
        PhotonNetwork.LoadLevel("Main");
    }

    void SaveGame(){
        // Speichern der Zustände in Firebase
        Debug.Log("Versuche zu speichern...");       
        firebaseDBManager.UpdateGameState(Maskottchen_Manager.hungry, Maskottchen_Manager.unsatisfied, Maskottchen_Manager.tired);
    }


    #endregion

}

}
