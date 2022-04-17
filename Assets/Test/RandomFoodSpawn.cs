using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RandomFoodSpawn : MonoBehaviour
{
    public GameObject[] FoodsObjects;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCorutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnCorutine()
    {
        while (true)
        {
            Vector3 SpawnPos = new Vector3(Random.Range(170, 15), 0.8f, Random.Range(10, 170));
            if (!Physics.CheckSphere(SpawnPos, 0.5f)){
                PhotonNetwork.Instantiate(FoodsObjects[Random.Range(0, FoodsObjects.Length)].name, SpawnPos, Quaternion.identity);
            }
            else
            {
                continue;
            }
            yield return new WaitForSeconds(10);
        }
    }
}
