using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] public int point_ToWin = 1;
    public int score_Player0 = 0;
    public int score_Player1 = 0;
    [SerializeField] public TextMeshProUGUI text_Score_Player0 = null;
    [SerializeField] public TextMeshProUGUI text_Score_Player1 = null;
    [SerializeField] public bool inGameScene = false;


    private RoomDoorWay roomDoorWay = null;
    [SerializeField] public Player[] players = null;

    public string playerName;
    private void Start()
    {
        roomDoorWay = RoomDoorWay.instance;
        roomDoorWay.ConnectToMasterServer();
        players = roomDoorWay.GetPlayers();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        if (SceneManager.GetActiveScene().name == "FirstScene_Oohira") StartCoroutine(InitScene_Game());
        if (SceneManager.GetActiveScene().name == "Menu") StartCoroutine(InitScene_Menu());
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " をロードしました");
        if (scene.name == "FirstScene_Oohira") StartCoroutine(InitScene_Game());
        if (scene.name == "Menu") StartCoroutine(InitScene_Menu());
    }
    void OnSceneUnloaded(Scene scene)
    {
        Debug.Log(scene.name + " をアンロードしました");
        if (scene.name == "FirstScene_Oohira") StartCoroutine(ExitScene_Game());
    }

    public GameObject WhiteKing;
    public GameObject BlackKing;
    public GameObject Image;
    public bool whiteturn = true;
    private IEnumerator InitScene_Game()
    {
        Debug.Log("InitScene_Game");
        roomDoorWay.Join();
        yield return new WaitUntil(() => RoomDoorWay.instance.Ready());
        inGameScene = true;
        text_Score_Player0 = GameObject.Find("Score_Player0").GetComponent<TextMeshProUGUI>();
        text_Score_Player1 = GameObject.Find("Score_Player1").GetComponent<TextMeshProUGUI>();
        text_Score_Player0.text = (0 + " / " + point_ToWin);
        text_Score_Player1.text = (0 + " / " + point_ToWin);
    }
    private IEnumerator ExitScene_Game()
    {
        yield return new WaitForSeconds(0.1f);
        roomDoorWay.Leave();
    }

    private IEnumerator InitScene_Menu()
    {
        yield return new WaitForSeconds(0.1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A");
            RoomDoorWay.instance.Join();
        }

        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch)) UnityEditor.EditorApplication.isPaused = true;
        Update_GameScene();
    }

    public void Goal(Owners player)
    {
        if (player == Owners.player0)
        {
            score_Player0++;
            text_Score_Player0.text = (score_Player0 + " / " + point_ToWin);
        }
        else
        {
            score_Player1++;
            text_Score_Player1.text = (score_Player1 + " / " + point_ToWin);
        }
    }


    private void Update_GameScene()
    {
        if (!inGameScene) return;
        if (score_Player0 >= point_ToWin || score_Player1 >= point_ToWin)
            Load_Menu();
    }


    public void Load_Game()
    {
        StartCoroutine(Load_Game_Co(3f));
    }
    private IEnumerator Load_Game_Co(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("FirstScene_Oohira");
    }

    public void Load_Menu()
    {
        StartCoroutine(Load_Menu_Co(3f));
    }
    private IEnumerator Load_Menu_Co(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("StartScene");
    }
}
