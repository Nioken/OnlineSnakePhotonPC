using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Chat;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private List<GameObject> MenuGUI = new List<GameObject>();
    [SerializeField]
    private TMP_InputField NickField;
    [SerializeField]
    private TMP_Text MaxPlayersText;
    [SerializeField]
    private Slider MaxPlayersSlider;
    [SerializeField]
    private TMP_InputField RoomNameField;
    [SerializeField]
    private GameObject RoomCreationGUI;
    [SerializeField]
    private GameObject RoomListGUI;
    [SerializeField]
    private List<RoomInfo> RoomList = new List<RoomInfo>();
    [SerializeField]
    private GameObject RoomInfoPref;
    [SerializeField]
    private GameObject ScrollContent;
    [SerializeField]
    private GameObject PlayersReadyGUI;
    [SerializeField]
    private GameObject PlayerReadyPref;
    [SerializeField]
    private float InfosOffset = 0;
    [SerializeField]
    private List<GameObject> RoomInfosPrefabs = new List<GameObject>();
    [SerializeField]
    private List<GameObject> PlayerReadyList = new List<GameObject>();
    [SerializeField]
    private GameObject PlayersScrollContent;
    [SerializeField]
    private int PlayersInRoom;
    [SerializeField]
    private int ReadyPlayers;
    [SerializeField]
    private GameObject CounterText;
    [SerializeField]
    private Vector2 sizeDelta;
    [SerializeField]
    private bool IsButtonHiden = false;


    public void SetNickName()
    {
        PhotonNetwork.NickName = NickField.text;
        Debug.Log(NickField.text);
    }
    public void GUICreateRoom()
    {
        for(int i = 0; i < MenuGUI.Count; i++)
        {
            MenuGUI[i].SetActive(false);
        }
        RoomCreationGUI.SetActive(true);
    }
    public void HideCreationGUI()
    {
        RoomCreationGUI.SetActive(false);
    }
    public void MainGUI()
    {
        RoomCreationGUI.SetActive(false);
        for (int i = 0; i < MenuGUI.Count; i++)
        {
            MenuGUI[i].SetActive(true);
        }
    }
    public void SetMaxPlayers()
    {
        MaxPlayersText.text = "Max Players: " + MaxPlayersSlider.value.ToString();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomList = roomList;
        Debug.Log("RoomListUpdated");
    }
    public void ShowRoomList()
    {
        ScrollContent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        InfosOffset = 230;
        for (int i = 0; i < MenuGUI.Count; i++)
        {
            MenuGUI[i].SetActive(false);
        }
        RoomListGUI.SetActive(true);

        for(int i = 0; i < RoomList.Count; i++)
        {
            GameObject RoomInfoObject = Instantiate(RoomInfoPref, ScrollContent.transform);
            RoomInfosPrefabs.Add(RoomInfoObject);
            RoomInfoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(RoomInfoObject.GetComponent<RectTransform>().anchoredPosition.x, RoomInfoObject.GetComponent<RectTransform>().anchoredPosition.y - InfosOffset);
            ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(ScrollContent.GetComponent<RectTransform>().sizeDelta.x, ScrollContent.GetComponent<RectTransform>().sizeDelta.y + InfosOffset);
            InfosOffset += 220;
            RoomInfoObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = RoomList[i].Name;
            RoomInfoObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = "Players: " + RoomList[i].PlayerCount + "/" + RoomList[i].MaxPlayers;
            string roomName = RoomList[i].Name;
            Debug.ClearDeveloperConsole();
            Debug.Log(roomName);
            RoomInfoObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(()=>JoinRoom(roomName));
            Debug.Log(RoomInfoObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.ToString());

        }
    }
    public void HideRoomList()
    {
        for (int i = 0; i < MenuGUI.Count; i++)
        {
            MenuGUI[i].SetActive(true);
        }
        for(int i = 0; i < RoomInfosPrefabs.Count; i++)
        {
            Destroy(RoomInfosPrefabs[i]);
        }
        RoomListGUI.SetActive(false);
    }
    public void ShowRoomPlayers()
    {
        PlayersReadyGUI.SetActive(true);
    }
    public void HideRoomPlayers()
    {
        PlayersReadyGUI.SetActive(false);
    }
    
    void Start()
    {
        sizeDelta = ScrollContent.GetComponent<RectTransform>().sizeDelta;
        PhotonNetwork.NickName = "Player" + Random.Range(1, 10);
        Debug.Log(PhotonNetwork.NickName);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log(PhotonNetwork.CurrentLobby.Name);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinLobby();
        Debug.Log(PhotonNetwork.CloudRegion);
    }

    public void createRoom()
    {
        PhotonNetwork.CreateRoom(RoomNameField.text, new Photon.Realtime.RoomOptions { MaxPlayers = (byte)MaxPlayersSlider.value},TypedLobby.Default);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom(string Name) {
        PhotonNetwork.JoinRoom(Name);
    }


    public override void OnJoinedRoom()
    {
        //SceneManager.LoadScene(1);
        Debug.Log(PhotonNetwork.NickName + " joined room");
        HideRoomList();
        for (int i = 0; i < MenuGUI.Count; i++)
        {
            MenuGUI[i].SetActive(false);
        }
        ShowRoomPlayers();
        PlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        HideCreationGUI();
        SpawnReadyPrefs();

    }
    public void SpawnReadyPrefs()
    {
        int count = 1;
        for (int i = 0; i < PlayerReadyList.Count; i++)
        {
            Destroy(PlayerReadyList[i]);
        }
        PlayerReadyList.Clear();
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            var tmp = Instantiate(PlayerReadyPref, PlayersScrollContent.transform);
            tmp.transform.GetChild(0).GetComponent<TMP_Text>().text = player.Value.NickName;
            if (count > 1)
            {
                tmp.GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp.GetComponent<RectTransform>().anchoredPosition.x, tmp.GetComponent<RectTransform>().anchoredPosition.y - 80 * count);
            }
            PlayerReadyList.Add(tmp);
            count++;
        }
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        MainGUI();
        HideRoomList();
        HideRoomPlayers();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    public void SetReady(string Nickname)
    {
        Debug.Log(Nickname + " ready");
        for(int i = 0; i < PlayerReadyList.Count; i++)
        {
            if(PlayerReadyList[i].transform.GetChild(0).GetComponent<TMP_Text>().text == Nickname)
            {
                if (PlayerReadyList[i].transform.GetChild(1).GetComponent<Image>().enabled == false)
                {
                    PlayerReadyList[i].transform.GetChild(1).GetComponent<Image>().enabled = true;
                    ReadyPlayers++;
                }
            }
        }
    }

    public void CallReadyRPC()
    {
        photonView.RPC("SetReady", RpcTarget.All, PhotonNetwork.NickName);
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        if(PlayersInRoom != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            PlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
            SpawnReadyPrefs();
        }
        if(ReadyPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            if (!IsButtonHiden)
            {
                GameObject.Find("ReadyBtn").SetActive(false);
                GameObject.Find("LeaveBtn").SetActive(false);
                IsButtonHiden = true;
            }
            StartCoroutine(StartCountCorutine());
        }
        
    }
    public IEnumerator StartCountCorutine()
    {
        CounterText.SetActive(true);
        int Iterations = 3;
        while (true)
        {
            CounterText.GetComponent<TMP_Text>().text = "Game will start in " + Iterations + " Seconds...";
            Iterations--;
            if(Iterations == 0)
            {
                SceneManager.LoadScene(1);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
