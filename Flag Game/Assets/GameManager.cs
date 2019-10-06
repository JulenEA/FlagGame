using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{

    public GameObject playerPrefab;

    public GameObject spawnPoints;

    private void Start()
    {

        if (PhotonNetwork.IsConnected)
        {
            if(playerPrefab == null || spawnPoints == null)
            {
                Debug.LogError("No se ha asignado la variable playerPrefab o spawnPoints");
                return;
            }
            else
            {
                string userId1 = PhotonNetwork.PlayerList[1].UserId;
                string userId0 = PhotonNetwork.PlayerList[0].UserId;
                string userIdLocal  = PhotonNetwork.LocalPlayer.UserId;

                Debug.Log("0: " + userId0 + ", 1: " + userId1 + " || Local: " + userIdLocal);

                Vector3 spawningPoint = GetSpawningPointForPlayer(PhotonNetwork.LocalPlayer.UserId);
                int playerTeam = GetPlayerTeam(PhotonNetwork.LocalPlayer.UserId);

                PhotonNetwork.Instantiate(playerPrefab.name, spawningPoint, Quaternion.identity, 0, new object[] { playerTeam });
            }
        }
    }

    private Vector3 GetSpawningPointForPlayer(string pUserId)
    {
        int playerNumber = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            if (p.UserId != null && pUserId.Equals(pUserId))
            {
                return spawnPoints.transform.GetChild(i).position;
            }
            
        }
        return new Vector3(0f,5f,0f);
    }

    private int GetPlayerTeam(string pUserId)
    {
        int playerNumber = PhotonNetwork.PlayerList.Length;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player p = PhotonNetwork.PlayerList[i];
            if (p.UserId != null && pUserId.Equals(pUserId))
            {
                return i % 2 != 0 ? 1 : 2;
            }

        }
        return 1;
    }

    public void OnExitButtonPressed()
    {
        if (!PhotonNetwork.IsConnected)
        {
            //Load MainMenu
            SceneManager.LoadScene(0);
        }
        else
        {
            PhotonNetwork.Disconnect();
        }
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected!");
        SceneManager.LoadScene(0);
    }
}
