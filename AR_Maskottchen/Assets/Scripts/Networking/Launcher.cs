using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Maskottchen.Networking{
public class Launcher : MonoBehaviourPunCallbacks
{

    #region Private Variables
    private string gameVersion = "1";
    [Tooltip("Maximale Anzahl der User in einem Raum.")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [Tooltip("Ladebalken UI Panel")]
    [SerializeField]
    private GameObject progressLabel, playButton;
    
    bool isConnecting;

    #endregion

    #region Monobehaviour Callbacks
    
    void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

    void Start(){

        playButton.SetActive(true);
        progressLabel.SetActive(false);
    
    }
    #endregion
    
    #region Public Methods
    public void Connect(){
        
        //UI anpassen
        progressLabel.SetActive(true);
        playButton.SetActive(false);

        if(PhotonNetwork.IsConnected){

            // Versuchen einem bestehendem Raum beizureten
            PhotonNetwork.JoinRandomRoom();

        }else{

            // Mit Server verbinden
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        
        }
    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster(){

        
        Debug.Log("Networking: Mit PUN-Netzwerk verbunden.");

        if(isConnecting)
        {
            // Raum erstellen oder Beitreten, wenn verbunden
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause){

        //UI anpassen
        playButton.SetActive(true);
        progressLabel.SetActive(false);
        
        isConnecting = false;

        //Error für nicht verbunden
        Debug.LogError("Networking: Nicht mehr mit dem Server verbunden! Grund: " + cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Networking: Client ist einem Room beigetreten.");

        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1){
            
            Debug.Log("Networking: Als erster Spieler diesen Raum betreten.");

            //Szene Laden
            PhotonNetwork.LoadLevel("Main");
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Networking: Beitreten zu einem Random Room nicht möglich. " + message + ". " + returnCode + "\n Neuer Random Room wird erstellt.");

        // Neuen Raum erstellen, falls noch keiner exisitiert
        PhotonNetwork.CreateRoom(null, new RoomOptions{ MaxPlayers = maxPlayersPerRoom});

    }

    #endregion
}
}