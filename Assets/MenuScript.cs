using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject SnakePref;
    public Vector3 StartPos;
    // Start is called before the first frame update
    void Start()
    {
        SnakePref = GameObject.Find("SnakeHead3");
        StartPos = SnakePref.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log(other.name);
            SnakePref.transform.position = StartPos;
            SnakePref.transform.GetChild(0).transform.position = StartPos;
            SnakePref.transform.GetChild(1).transform.position = StartPos - Vector3.back;
            SnakePref.transform.GetChild(2).transform.position = StartPos - Vector3.back * 2;
            Debug.Log("11111");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
