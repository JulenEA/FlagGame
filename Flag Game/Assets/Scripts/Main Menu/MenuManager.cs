using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{

    public string appVersion = "1";

    public void OnOfflineButtonPressed()
    {
        SceneManager.LoadScene(1);
    }

    public void OnOnlineButtonPressed()
    {
        Connect();
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = appVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster reached");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Room failed");
        PhotonNetwork.CreateRoom(null, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Joined");
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel(2);
        }
    }



}
