using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviourPunCallbacks
{

    //TESTING
    public bool mobileControl = true;


    public float dashMinimumLength = 0.1f;
    public ButtonPressed buttonPressed;
    public float dashLength = 6f;

    int floorMask;

    private NavMeshAgent navMeshAgent;

    private bool dashed = false;
    private Vector2? touchStartPosition;


    //Disparos
    public GameObject balaGO;
    public GameObject cañonGO;
    private float coolDownAccumulated;

    


    //Settings del jugador
    PlayerSettings playerSettings;

    Rigidbody playerRigidBody;

    void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        playerRigidBody = GetComponent<Rigidbody>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerSettings = GetComponent<PlayerSettings>();
        if (PhotonNetwork.IsConnected)
        {
            InitializePlayerSettings();
        }
    }

    private void InitializePlayerSettings()
    {
        playerSettings.playerTeam = (int)photonView.InstantiationData[0];
    }

    private void Start()
    {
        //Esto es para que se pueda disparar desde el principio de la partida
        coolDownAccumulated = playerSettings.GetCoolDownTime();
    }

    void Update()
    {
        if (mobileControl)
        {
            MobileUpdate();
        }
        else
        {
            DesktopUpdate();
        }
       
    }

    private void DesktopUpdate()
    {

        if(PhotonNetwork.IsConnected && !photonView.IsMine)
        {
            return;
        }

        //CoolDown management
        if (coolDownAccumulated < playerSettings.GetCoolDownTime())
        {
            coolDownAccumulated += Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {   
            Vector3 raycastHit = getRayCastPoint(Input.mousePosition);
            if (raycastHit != Vector3.zero)
            {
                navMeshAgent.destination = raycastHit;
                navMeshAgent.isStopped = false;
            }

            dashed = false;
            touchStartPosition = null;
 
        }
        else if (Input.GetMouseButtonDown(1))
        {  
            Debug.Log(coolDownAccumulated + " / " + playerSettings.GetCoolDownTime());
            if (coolDownAccumulated >= playerSettings.GetCoolDownTime())
            {

                navMeshAgent.destination = transform.position;
                navMeshAgent.isStopped = true;
                Quaternion newRotation = RotatePlayerForShoting(Input.mousePosition);
                transform.rotation = newRotation;

                GameObject bala = null;
                if (!PhotonNetwork.IsConnected)
                {
                    bala = Instantiate(balaGO, cañonGO.transform.position, newRotation) as GameObject;
                    bala.GetComponent<BalaMovement>().SetBulletTargetPositon(new Vector3(0, 0, 0));
                    bala.transform.Rotate(new Vector3(90f, 0f, 0f));
                    bala.GetComponent<Rigidbody>().AddForce(transform.forward * playerSettings.GetBulletVelocity());
                    bala.GetComponent<BalaSettings>().SetBalaTeam(playerSettings.GetPlayerTeam());
                }
                else
                {
                    //Hay que pasarle parámetros a la bala de networking
                    //[0] -> BalaTeam
                    //bala = PhotonNetwork.Instantiate(balaGO.name, cañonGO.transform.position, newRotation, 0, new object[] { playerSettings.GetPlayerTeam() }) as GameObject;

                    //Vamos a intentar Instanciar con RPC
                    Debug.Log("Antes del disparo");
                    photonView.RPC("InstantiateBullet", RpcTarget.All, new object[] { newRotation , getRayCastPoint(Input.mousePosition) });

                }

                //Esto se hace para los dos porque el transform de la bala ya se está observando.
                
                //Destroy(bala, playerSettings.GetBulletDestroyingTime());

                coolDownAccumulated = 0f;
            } 
        }
    }

    [PunRPC]
    public void InstantiateBullet(Quaternion pNewRotation, Vector3 pTargetPosition)
    {


        Debug.Log("Disparo");
        GameObject bala = Instantiate(balaGO, cañonGO.transform.position, pNewRotation) as GameObject;
        bala.transform.Rotate(new Vector3(90f, 0f, 0f));

        //Este funciona bien, pero hay que tener en cuenta que es 3D y por lo tanto va hacia abajo. Habría que controlar eso si se usa este método.
        //bala.GetComponent<Rigidbody>().AddForce((pTargetPosition - bala.transform.position).normalized * playerSettings.GetBulletVelocity());
        bala.GetComponent<Rigidbody>().AddForce(bala.transform.up * playerSettings.GetBulletVelocity());
        bala.GetComponent<BalaSettings>().SetBalaTeam(playerSettings.GetPlayerTeam());

    }

    private void MobileUpdate()
    {

        //CoolDown management
        if (coolDownAccumulated < playerSettings.GetCoolDownTime())
        {
            coolDownAccumulated += Time.deltaTime;
        }

        if (Input.touchCount == 1)
        {
            int fingerID = Input.GetTouch(0).fingerId;
            if (!buttonPressed.IsButtonPressed() && Input.GetTouch(0).phase == TouchPhase.Ended && !IsPointerOverUIObject())
            {
                Vector3 raycastHit = getRayCastPoint(Input.GetTouch(0).position);
                if (raycastHit != Vector3.zero)
                {
                    if (dashed && touchStartPosition != null && IsDashLongEnough(Input.GetTouch(0).position))
                    {
                        Debug.Log("DASH");
                        navMeshAgent.destination = Flash(raycastHit);
                        return;
                    }
                    else
                    {
                        navMeshAgent.destination = raycastHit;
                        navMeshAgent.isStopped = false;
                    }
                }

                dashed = false;
                touchStartPosition = null;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Debug.Log("MOVED");
                dashed = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchStartPosition = Input.GetTouch(0).position;
            }
            else
            {
                //Debug.Log("NO HACER NADA");
            }
        }
        else if (Input.touchCount == 2)
        {

            //Debug.Log("DOS TOQUES");
            if (buttonPressed.IsButtonPressed())
            {
                Debug.Log(coolDownAccumulated + " / " + playerSettings.GetCoolDownTime());
                if (coolDownAccumulated >= playerSettings.GetCoolDownTime())
                {

                    navMeshAgent.destination = transform.position;
                    navMeshAgent.isStopped = true;
                    Quaternion newRotation = RotatePlayerForShoting(Input.touches[1].position);
                    transform.rotation = newRotation;

                    GameObject bala = Instantiate(balaGO, cañonGO.transform.position, newRotation) as GameObject;
                    bala.GetComponent<BalaMovement>().SetBulletTargetPositon(new Vector3(0, 0, 0));
                    bala.transform.Rotate(new Vector3(90f, 0f, 0f));
                    bala.GetComponent<Rigidbody>().AddForce(transform.forward * playerSettings.GetBulletVelocity());
                    bala.GetComponent<BalaSettings>().SetBalaTeam(playerSettings.GetPlayerTeam());

                    Destroy(bala, playerSettings.GetBulletDestroyingTime());

                    coolDownAccumulated = 0f;
                }
            }
        }
    }

    private Quaternion RotatePlayerForShoting(Vector2 pSecondFingerPositon)
    {
        Vector3 rotationPosition = getRayCastPoint(pSecondFingerPositon);

        Vector3 playerToMouse = rotationPosition - transform.position;
        playerToMouse.y = 0f;
        Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
        playerRigidBody.MoveRotation(newRotation);

        return newRotation;
        
    }

    

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public bool IsDashLongEnough(Vector3 finishPoint)
    {
        return Math.Abs((finishPoint - touchStartPosition).Value.magnitude) >= Math.Abs(dashMinimumLength);
    }
    //Hay que cotrolar mejor la distancia. A veces recorre mucho y otras poco. Debería ser siempre igual.
    public Vector3 Flash(Vector3 endPoint)
    {
        Vector3 flashDirection = (endPoint - getRayCastPoint(touchStartPosition.Value)).normalized;
        float step = dashLength * Time.deltaTime;
        Vector3 targetPosition = transform.position + flashDirection * step;
        targetPosition.y = transform.position.y;
        //Debug.Log(flashDirection.x + ", " + flashDirection.y + ", " + flashDirection.z);
        transform.position = targetPosition;//Vector3.MoveTowards(transform.position, targetPosition, step);
        return targetPosition;

    }

    public Vector3 getRayCastPoint(Vector2 touchedPointOnScreen)
    {
        Ray rayCam = Camera.main.ScreenPointToRay(touchedPointOnScreen);
        RaycastHit raycastHit;
        if (Physics.Raycast(rayCam, out raycastHit, 100f, floorMask))
        {
            return raycastHit.point;
        }
        return Vector3.zero;
    }
}
