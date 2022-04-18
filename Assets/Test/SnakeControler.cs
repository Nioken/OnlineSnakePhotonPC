using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class SnakeControler : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject SnakeHead;
    public GameObject Camera;
    [SerializeField]
    private GameObject TailPrefab;
    public float MoveSpeed;
    public float RotateSpeed;
    public List<GameObject> SnakeTail = new List<GameObject>();
    public new PhotonView photonView;
    public GameObject Nickname;
    public GameObject CurrTarget;
    [SerializeField]
    private GameObject SpectatorCamPref;
    public int Score;
    [SerializeField]
    private GameObject StatPref;
    private GameObject StatTransform;
    [SerializeField]
    private RoomManager roomManager;
    private int PlayerInRoom;




    void SpawnOtherStats()
    {
        if (photonView.IsMine)
        {
            int count = 1;
            roomManager = GameObject.Find("MultiPlayerManager").GetComponent<RoomManager>();
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                if (player.Value.NickName != photonView.Owner.NickName)
                {
                    var tmp = Instantiate(StatPref, StatTransform.transform);
                    tmp.GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp.GetComponent<RectTransform>().anchoredPosition.x, tmp.GetComponent<RectTransform>().anchoredPosition.y - 100 * count);
                    roomManager.StatPrefs.Add(tmp);
                    tmp.transform.GetChild(0).GetComponent<TMP_Text>().text = player.Value.NickName;
                    StatPref.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Score.ToString();
                    count++;
                }
            }
        }
        photonView.RPC("LogScores", RpcTarget.All, Score, PhotonNetwork.NickName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SpawnOtherStats();
    }
    public override void OnEnable()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            roomManager = GameObject.Find("MultiPlayerManager").GetComponent<RoomManager>();
            StatTransform = GameObject.Find("StatsParent");
        }
    }
        
    
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Nickname = transform.GetChild(2).gameObject;
            Nickname.GetComponent<TMP_Text>().text = PhotonNetwork.NickName;
            Destroy(Nickname);
        }
        else
        {
            photonView = gameObject.GetComponent<PhotonView>();
            Nickname = transform.GetChild(2).gameObject;
            Nickname.GetComponent<TMP_Text>().text = photonView.Owner.NickName;
            if (Nickname.GetPhotonView().IsMine)
            {
                Destroy(Nickname);
            }
            if (!photonView.IsMine) return;
            PlayerInRoom = PhotonNetwork.CurrentRoom.Players.Count;
            StatTransform = GameObject.Find("StatsParent");
            StatPref = Instantiate(StatPref, StatTransform.transform);
            roomManager = GameObject.Find("MultiPlayerManager").GetComponent<RoomManager>();
            roomManager.StatPrefs.Add(StatPref);
            StatPref.transform.parent = StatTransform.transform;
            StatPref.transform.GetChild(0).GetComponent<TMP_Text>().text = PhotonNetwork.NickName;
            StatPref.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = Score.ToString();
            SpawnOtherStats();
        }
    }

    [PunRPC]
    public void LogScores(int _Score, string NickName)
    {
        var tmp = GameObject.FindGameObjectsWithTag("StatPrefTag");
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].transform.GetChild(0).GetComponent<TMP_Text>().text.Contains(NickName))
            {
                tmp[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = _Score.ToString();
            }
        }
    }

    private void OnCollisionEnter(Collision obj)
    {
        if(obj.gameObject.tag == "Eat")
        {
            if (photonView.Owner.IsMasterClient)
            {
                PhotonNetwork.Destroy(obj.gameObject);
            }
            else
            {
                Destroy(obj.gameObject);
            }
            if (!photonView.IsMine) return;
            if(SnakeTail.Count <= 0)
            {
                GameObject tmp;
                SnakeTail.Add(tmp = PhotonNetwork.Instantiate(TailPrefab.name, new Vector3(SnakeHead.transform.position.x, SnakeHead.transform.position.y, SnakeHead.transform.position.z - 0.4f), Quaternion.identity));
                Score++;
            }
            if(SnakeTail.Count > 0)
            {
                GameObject tmp;
                GameObject LastTailObject = SnakeTail[SnakeTail.Count-1];
                SnakeTail.Add(tmp = PhotonNetwork.Instantiate(TailPrefab.name, new Vector3(LastTailObject.transform.position.x, LastTailObject.transform.position.y, LastTailObject.transform.position.z - 0.42f), Quaternion.identity));
                Score++;
            }
            photonView.RPC("LogScores", RpcTarget.All, Score, photonView.Owner.NickName);
        }
        if(obj.gameObject.tag == "Enviroment")
        {
            if (!photonView.IsMine) return;
            for (int i = 2; i < SnakeTail.Count; i++)
            {
                PhotonNetwork.Destroy(SnakeTail[i]);
            }
            PhotonNetwork.Destroy(gameObject.transform.parent.gameObject);
            Instantiate(SpectatorCamPref, gameObject.transform.position, Quaternion.identity);
            photonView.RPC("LogScores", RpcTarget.AllBuffered, Score, PhotonNetwork.NickName);
        }
    }
    void Update()
    {
        SnakeControl();
    }

    public void SnakeControl()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            var targetRotation = Quaternion.LookRotation(CurrTarget.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1 * Time.deltaTime);
            transform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime);

            return;
        }
        if (!photonView.IsMine) return;

        if (PlayerInRoom < PhotonNetwork.CurrentRoom.Players.Count)
        {
            PlayerInRoom = PhotonNetwork.CurrentRoom.Players.Count;
            SpawnOtherStats();
        }

        transform.Translate(Vector3.right * MoveSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up * RotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.down * RotateSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MoveSpeed *= 3;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed /= 3;
        }
    }
}
