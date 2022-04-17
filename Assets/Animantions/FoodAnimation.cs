using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodAnimation : MonoBehaviour
{
    public float RotationSpeed;
    public float MoveSpeed;
    private Vector3 MaxY;
    private Vector3 MinY;
    private float maxY;
    private float minY;
    private bool IsUp = false;
    // Start is called before the first frame update
    void Start()
    {
        MaxY = transform.position + new Vector3(0,0.1f,0);
        MinY = transform.position - new Vector3(0, 0.1f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y + 90f,transform.rotation.eulerAngles.z)) , RotationSpeed * Time.deltaTime);
        if (!IsUp)
        {
            //transform.position = Vector3.Slerp(transform.position, MaxY, MoveSpeed * Time.deltaTime);
            transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);
            if(transform.position.y >= MaxY.y - 0.01f)
            {
                IsUp = true;
            }
        }
        else
        {
            //transform.position = Vector3.Slerp(transform.position, MinY, MoveSpeed * Time.deltaTime);
            transform.Translate(Vector3.down * MoveSpeed * Time.deltaTime);
            if (transform.position.y <= MinY.y + 0.01f)
            {
                IsUp = false;
            }
        }
    }
}
