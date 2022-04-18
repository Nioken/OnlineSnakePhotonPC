using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject StartPlayerPrefab;
    public List<GameObject> StatPrefs = new List<GameObject>();
    [SerializeField]
    private List<Vector3> StartPosotions = new List<Vector3>();
    [SerializeField]
    private TMP_Text GameTimer;
    //private int min = 5;
    //private int sec = 0;
    private DateTime time;
    [SerializeField]
    private TMP_Text WinnerText;
    
    void Start()
    {
        
        StartCoroutine(GameTimeCorutine());
        StartPosotions.Add(new Vector3(91.0309219f, 3.63000011f, 135.665436f));
        StartPosotions.Add(new Vector3(117.380096f, 3.63000011f, 18.4112129f));
        StartPosotions.Add(new Vector3(74.0999985f, 3.63000011f, 18.4112129f));
        StartPosotions.Add(new Vector3(24.2000008f, 3.63000011f, 71.8000031f));
        GameObject tmp = PhotonNetwork.Instantiate(StartPlayerPrefab.name, StartPosotions[PhotonNetwork.LocalPlayer.ActorNumber], Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(newPlayer.NickName + " joined this room");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.NickName + "Leave the room");
    }

    IEnumerator GameTimeCorutine()
    {
        time = new DateTime(2000, 12, 12, 0, 5, 0);
        Debug.Log(time);
        Debug.Log(time.Minute > 0 || time.Second > 0);
        while (time.Minute > 0 || time.Second > 0)
        {
            yield return new WaitForSeconds(1);
            time = time.AddSeconds(-1);
            if (time.Second >= 10)
            {
                GameTimer.text = "0" + time.Minute.ToString() + ":" + time.Second.ToString();
            }
            else
            {
                GameTimer.text = "0" + time.Minute.ToString() + ":" + "0" + time.Second.ToString();
            }
        }
        var Stats = GameObject.FindGameObjectsWithTag("StatPrefTag");
        int MaxScore = Convert.ToInt32(Stats[0].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
        string WinnerNick = Stats[0].transform.GetChild(0).GetComponent<TMP_Text>().text;
        for(int i = 0; i < Stats.Length-1; i++)
        {
            if(Convert.ToInt32(Stats[i+1].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text) >
                Convert.ToInt32(Stats[i].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text))
            {
                MaxScore = Convert.ToInt32(Stats[i + 1].transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text);
                WinnerNick = Stats[i + 1].transform.GetChild(0).GetComponent<TMP_Text>().text;
            }
        }
        WinnerText.gameObject.SetActive(true);
        WinnerText.text = WinnerNick + " is winner!";
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene(0);
    }


}
