using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;


public class RoomPanel : BaseUI
{
    [SerializeField] TMP_FontAsset kFont;
    [SerializeField] PlayerEntry[] _playerEntries;
    [SerializeField] Button _startButton;

   // [SerializeField] List <string> mapList;
    private List<string> mapList = new List<string>();


    // Map 관련
    // [SerializeField] Button _mapSelectButton;
    GameObject _mapListPanel;
    GameObject _map01;
    GameObject _map02;
    GameObject _map03;
    GameObject _map04;
    GameObject _map05;
    GameObject _map06;
    Button _map01Button;
    Button _map02Button;
    Button _map03Button;
    Button _map04Button;
    Button _map05Button;
    Button _map06Button;
    public int mapNumber = 1;


    private void OnEnable()
    {
        UpdatePlayers();

        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);

        // 레디상태 체크로그
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        Debug.Log($"레디상태: {ready}");

        Init();

        // TestLog();

    }
    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
        _startButton.onClick.RemoveListener(StartGame);
    }
    private void Init()
    {
        GetUI<Button>("PreviousButton").onClick.AddListener(LeaveRoom);
        GetUI<TMP_Text>("PreviousButtonText").font = kFont;
        _startButton = GetUI<Button>("StartButton");
        GetUI<TMP_Text>("StartButtonText").font = kFont;
        _startButton.onClick.AddListener(StartGame);

        GetMapList();
        // 맵선택 관련
        GetUI<Button>("MapSelectButton").onClick.AddListener(OpenMapList);
        _mapListPanel = GetUI("MapListPanel");
        GetUI<Button>("MapCancelButton").onClick.AddListener(CloseMapList);
        _map01Button = GetUI<Button>("MapSelectButton01");
        _map02Button = GetUI<Button>("MapSelectButton02");
        _map03Button = GetUI<Button>("MapSelectButton03");
        _map04Button = GetUI<Button>("MapSelectButton04");
        _map05Button = GetUI<Button>("MapSelectButton05");
        _map06Button = GetUI<Button>("MapSelectButton06");
        _map01Button.onClick.AddListener(Map1Selected);
        _map02Button.onClick.AddListener(Map2Selected);
        _map03Button.onClick.AddListener(Map3Selected);
        _map04Button.onClick.AddListener(Map4Selected);
        _map05Button.onClick.AddListener(Map5Selected);
        _map06Button.onClick.AddListener(Map6Selected);

        GetUI<TMP_Text>("MapNameText01").text = (mapList[1]);
        GetUI<TMP_Text>("MapNameText02").text = (mapList[2]);
        GetUI<TMP_Text>("MapNameText03").text = (mapList[3]);
        GetUI<TMP_Text>("MapNameText04").text = (mapList[4]);
        GetUI<TMP_Text>("MapNameText05").text = (mapList[5]);
        GetUI<TMP_Text>("MapNameText06").text = (mapList[6]);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SelectMap();
        }
    }
    void OpenMapList()
    {
        _mapListPanel.SetActive(true);
    }
    void CloseMapList()
    {
        _mapListPanel.SetActive(false);
    }
    void GetMapList()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            mapList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
    }
   void SelectMap()
   {

   }
    public void Map1Selected()
    {
        mapNumber = 1;
        _mapListPanel.SetActive(false);
    }
    public void Map2Selected()
    {
        mapNumber = 2;
        _mapListPanel.SetActive(false);
    }
    public void Map3Selected()
    {
        mapNumber = 3;
        _mapListPanel.SetActive(false);
    }
    public void Map4Selected()    
    {
        mapNumber = 4;
        _mapListPanel.SetActive(false);
    }
    public void Map5Selected()    
    {
        mapNumber = 5;
        _mapListPanel.SetActive(false);
    }
    public void Map6Selected()   
    {
        mapNumber = 6;
        _mapListPanel.SetActive(false);
    }
    public void UpdatePlayers()
    {
        foreach (PlayerEntry entry in _playerEntries)
        {
            entry.SetEmpty();
        }
        // 현제 방에 있는 모든 플레이어 가져오기
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Number 할당 전은 -1  이니까, 그 플레이어는 할당하지 않는다.
            if (player.GetPlayerNumber() == -1)
                continue;

            int number = player.GetPlayerNumber();
            _playerEntries[number].SetPlayer(player);
        }
        // 여기서 몇명이상은 안되게끔, 조건문을 걸면 몇며이상부터~ 하게 할 수 있다.
        
        // 본인이 방장일대만 누를 수 있게하기
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            _startButton.interactable = CheckAllReady();
        }
        else
        {
            _startButton.interactable = false;
        }

        // 레디버튼 본인일때만 본인거 누를수 있게하기


    }
    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties)
    {
        if(properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }
    }
    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName}님이 입장하였습니다.");
        UpdatePlayers();
    }
    public void ExitPlayer(Player oldPlayer)
    {
        Debug.Log($"{oldPlayer.NickName}님이 입장하였습니다.");
        UpdatePlayers();
    }
    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 다 돌면서 한명이라도 false면 리턴
            if(player.GetReady() == false)
                return false;
        }   // 다 돌고 다 레디면
        return true;
    }
    public void ChoseMap()
    {


        for (int i = 0; i<SceneManager.sceneCountInBuildSettings; i++)
        {
            mapList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
    }

    public void StartGame()
    {
        
      //  for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
      //  {
      //      mapList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
      //  }
        PhotonNetwork.LoadLevel(mapList[mapNumber]); // 게임 연결하면서 이름따라서 변경
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
