using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class CameraController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float camSens = 0.25f;
    private Vector3 lastMouse = new Vector3(255, 255, 255);
    private bool IsSpectate;
    void Start()
    {
        if(SceneManager.GetActiveScene().name != "Menu")
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }
        if (!IsSpectate)
        {
            if (!gameObject.GetComponent<PhotonView>().IsMine)
            {
                gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(SetNicknamesTargetCorutine());
            }
        }
    }

    void Update()
    {

        CameraControl();
    }

    public void CameraControl()
    {
        if (!IsSpectate)
        {
            if (!gameObject.GetComponent<PhotonView>().IsMine) return;


            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
        }
        else
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;

            if (Input.GetKey(KeyCode.W))
            {
                if (!Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, 1.2f))
                    gameObject.transform.Translate(Vector3.forward * 10 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (!Physics.Raycast(gameObject.transform.position, gameObject.transform.forward * -1, 1.2f))
                    gameObject.transform.Translate(Vector3.back * 10 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (!Physics.Raycast(gameObject.transform.position, gameObject.transform.right * -1, 1.2f))
                    gameObject.transform.Translate(Vector3.left * 10 * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!Physics.Raycast(gameObject.transform.position, gameObject.transform.right, 1.2f))
                    gameObject.transform.Translate(Vector3.right * 10 * Time.deltaTime);
            }

        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)    {
        StartCoroutine(WaitCorutine());
    }
    IEnumerator WaitCorutine()
    {
        yield return new WaitForSeconds(3);
        if (gameObject.GetComponent<PhotonView>().IsMine)
        {
            GameObject[] NickNames = GameObject.FindGameObjectsWithTag("Nickname");
            for (int i = 0; i < NickNames.Length; i++)
            {
                NickNames[i].GetComponent<NicknameLookAt>().Target = gameObject;
            }
        }
    }
    IEnumerator SetNicknamesTargetCorutine()
    {
        yield return new WaitForSeconds(3);
            if (gameObject.GetComponent<PhotonView>().IsMine)
            {
                GameObject[] NickNames = GameObject.FindGameObjectsWithTag("Nickname");
                for (int i = 0; i < NickNames.Length; i++)
                {
                    NickNames[i].GetComponent<NicknameLookAt>().Target = gameObject;
                }
            }
        
    }
}
