using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Maskottchen.Networking{
public class NetworkManager : MonoBehaviourPunCallbacks
{

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


    #endregion

}

}
