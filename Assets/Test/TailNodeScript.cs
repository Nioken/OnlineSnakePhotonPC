using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TailNodeScript : MonoBehaviour
{
    [SerializeField]
    private SnakeControler _snakeControler;
    public GameObject TargetObject;
    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (!gameObject.GetPhotonView().IsMine) return;
            _snakeControler = GameObject.FindGameObjectWithTag("SnakeHad").GetComponent<SnakeControler>();
            if (TargetObject == null)
            {
                TargetObject = _snakeControler.SnakeTail[_snakeControler.SnakeTail.Count - 2].gameObject;
            }
        }
    }

    void Update()
    {
        MoveTailNode();
    }

    public void MoveTailNode()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            gameObject.transform.LookAt(TargetObject.transform, Vector3.up);
            gameObject.transform.position = Vector3.Lerp(transform.position, TargetObject.transform.position, _snakeControler.MoveSpeed * Time.deltaTime);
        }
        else
        {
            if (!gameObject.GetPhotonView().IsMine) return;
            gameObject.transform.LookAt(TargetObject.transform, Vector3.up);
            gameObject.transform.position = Vector3.Lerp(transform.position, TargetObject.transform.position, _snakeControler.MoveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (SceneManager.GetActiveScene().name == "Menu") return;
        if(!gameObject.GetPhotonView().IsMine) return;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
