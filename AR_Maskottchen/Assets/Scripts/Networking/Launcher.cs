using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Maskottchen.Database;


namespace Maskottchen.Networking{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Private Variables
        private string gameVersion = "1";
        [Tooltip("Maximale Anzahl der User in einem Raum.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [Tooltip("Ladebalken fürs Network Connecting & Play Button")]
        [SerializeField]
        private GameObject connectionProgress, playButton;

        [Tooltip("Ladebalken Location & Location Failed UI")]
        [SerializeField]
        private GameObject locationProgress, locationFailed, gpsDeactived;

        [Tooltip("GPS Position bei der das Game startet in Längen- und Breitengraden")]
        [SerializeField]
        private Vector2 startPos;

        [Tooltip("Maximal zulässige Distanz, bei der das Game noch startet in Längen- und Breitengraden")]
        [SerializeField]
        private Vector2 maxDist;

        [Tooltip("GPS Abfrage für Debugging ausschalten")]
        [SerializeField]
        private bool gpsFake;

        bool isConnecting;

        [SerializeField]
        FirebaseDBManager firebaseManager;

        #endregion

        #region Monobehaviour Callbacks

        void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;

            //Wenn noch ein Maskottchen sichtbar sein sollte, wird es zerstört
            if(GameObject.FindGameObjectWithTag("Maskottchen")){
                Destroy(GameObject.FindGameObjectWithTag("Maskottchen"));
            }

            if(GameObject.FindGameObjectWithTag("Essen")){
                GameObject[] objs = GameObject.FindGameObjectsWithTag("Essen");
            }

        }

        void Start()
        {
            if (!gpsFake)
            {
                StartCoroutine(GetLocationData());
            }
            else
            {
                Debug.Log("GPS Fake für Debuggin ist aktiviert. Für Build deaktivieren.");
                StartNetworkMenu();
            }

        }

        #endregion

        #region Public Methods
        public void Connect()
        {

            //UI anpassen
            connectionProgress.SetActive(true);
            playButton.SetActive(false);

            if (PhotonNetwork.IsConnected)
            {

                // Versuchen einem bestehendem Raum beizureten
                PhotonNetwork.JoinRandomRoom();
                Debug.Log("Networking versuche Random Room beizutreten");

            }
            else
            {

                // Mit Server verbinden
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;

            }
        }

        public void LocateDevice()
        {
            StartCoroutine(GetLocationData());
        }

        #endregion

        #region Private Methods
        IEnumerator GetLocationData()
        {
            //Debug.Log("Versuche Location zu ermitteln...");


            // Check if the user has location service enabled.
            if (!Input.location.isEnabledByUser)
            {
               Debug.Log("GPS deaktiviert. Debugging-Modus kann im Script aktiviert werden (GPS-Fake).");

                // UI anpassen
                gpsDeactived.SetActive(true);
                playButton.SetActive(false);
                connectionProgress.SetActive(false);
                locationProgress.SetActive(false);
                locationFailed.SetActive(false);

                yield break;

            }

            // UI anpassen
            playButton.SetActive(false);
            connectionProgress.SetActive(false);
            locationProgress.SetActive(true);
            locationFailed.SetActive(false);
            gpsDeactived.SetActive(false);

            // Starts the location service.
            Input.location.Start();

            // Waits until the location service initializes
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // If the service didn't initialize in 20 seconds this cancels location service use.
            if (maxWait < 1)
            {
                print("Location Service Timed out");
                yield break;
            }

            // If the connection failed this cancels location service use.
            if (Input.location.status == LocationServiceStatus.Failed)
            {
                //Debug.Log("Unable to determine device location");
                yield break;
            }
            else
            {
                // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
                print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

                if (Input.location.lastData.latitude - startPos.x < maxDist.x && Input.location.lastData.longitude - startPos.y < maxDist.y)
                {
                    StartNetworkMenu();
                }
                else
                {

                    playButton.SetActive(false);
                    connectionProgress.SetActive(false);
                    locationProgress.SetActive(false);
                    locationFailed.SetActive(true);
                    gpsDeactived.SetActive(false);
                }
            }

            // Stops the location service if there is no need to query location updates continuously.
            Input.location.Stop();
        }

        void StartNetworkMenu()
        {
            // Das Networking Menu aktivieren
            playButton.SetActive(true);
            connectionProgress.SetActive(false);
            locationProgress.SetActive(false);
            locationFailed.SetActive(false);
            gpsDeactived.SetActive(false);
        }

        void LoadSettings(){
            
            // Letzten Gamestate laden
           firebaseManager.GetGameState();
        }

        #endregion

        #region MonoBehaviourPunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {


            Debug.Log("Networking: Mit PUN-Netzwerk verbunden.");

            if (isConnecting)
            {
                // Raum erstellen oder Beitreten, wenn verbunden
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {

            //UI anpassen
            playButton.SetActive(true);
            connectionProgress.SetActive(false);

            isConnecting = false;

            //Error für nicht verbunden
            //Debug.LogError("Networking: Nicht mehr mit dem Server verbunden! Grund: " + cause);

        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Networking: Client ist einem Room beigetreten.");

            // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {

                Debug.Log("Networking: Als erster Spieler diesen Raum betreten.");

                //Szene Laden
                PhotonNetwork.LoadLevel("Main");

                // Werte des Maskottchens (Hunger, Zufriedenheit, Müdigkeit) laden

                LoadSettings();

            }
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Networking: Beitreten zu einem Random Room nicht möglich. " + message + ". " + returnCode + "\n Neuer Random Room wird erstellt.");

            // Neuen Raum erstellen, falls noch keiner exisitiert
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

        }

        #endregion
    }
}