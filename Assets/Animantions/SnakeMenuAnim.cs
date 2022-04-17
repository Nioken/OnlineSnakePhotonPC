using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMenuAnim : MonoBehaviour
{
    public GameObject NextTarget;
    public GameObject TerrainPref;
    public GameObject Terrain;
    public GameObject[] target6;
    private void Start()
    {
        if(gameObject.name == "target 5")
        {
            target6 = GameObject.FindGameObjectsWithTag("target 6");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "SnakeHad")
        {
            if(gameObject.name == "target 5")
            {
                var tmp = Instantiate(TerrainPref, new Vector3(Terrain.transform.position.x, Terrain.transform.position.y, Terrain.transform.position.z + 180),Quaternion.identity).gameObject;
                target6[target6.Length-1].GetComponent<SnakeMenuAnim>().NextTarget = tmp.transform.GetChild(1).gameObject;
                Debug.Log(tmp.transform.GetChild(1).gameObject.name);
            }
            other.gameObject.GetComponent<SnakeControler>().CurrTarget = NextTarget;
        }
    }
}
