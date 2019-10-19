using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalaMovement : MonoBehaviourPunCallbacks
{

    public float velocity = 30f;
    Rigidbody bulletRigidBody;
    Vector3 TargetPosition;

    BalaSettings balaSettings;



    void Awake()
    {
        balaSettings = GetComponent<BalaSettings>();

        /* Hay que comentar esto si la bala no se instancia con PhotonNetwork, y sí con RPC
         * 
        int balaTeam = (int) photonView.InstantiationData[0];
        balaSettings.SetBalaTeam(balaTeam);
        */
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag.Equals("Wall"))
        {
            Debug.Log("Destroying bullet");
            Destroy(gameObject);
            
        }
        else if (other.gameObject.tag.Equals("Player"))
        {
            Debug.Log(balaSettings == null ? "null" : "Not null" + " , " + balaSettings.GetBalaTeam());
            Debug.Log(other.gameObject.GetComponent<PlayerSettings>() == null ? "null" : "Not null" + " , " + other.gameObject.GetComponent<PlayerSettings>().GetPlayerTeam());
            Debug.Log(other.name);

            int iHitPlayerTeam = other.gameObject.GetComponent<PlayerSettings>().GetPlayerTeam();
            if (iHitPlayerTeam != balaSettings.GetBalaTeam())
            {
                if (!PhotonNetwork.IsConnected)
                {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                }
                else
                {
                    Destroy(gameObject);
                    PhotonNetwork.Destroy(other.gameObject);
                }

            }
        }
    }



    public void SetBulletTargetPositon(Vector3 pTargetPosition)
    {
        TargetPosition = pTargetPosition;
    }

    [PunRPC]
    public void DestroyBulletRpc(GameObject pBullet)
    {
        Destroy(pBullet);
    }
}
