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
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using static Photon.Pun.UtilityScripts.PunTeams;


public class RoomPanel : BaseUI
{
    [SerializeField] TMP_FontAsset kFont;
    [SerializeField] PlayerEntry[] _playerEntries;
    [SerializeField] Button _startButton;

    // Map 관련
   // [SerializeField] List <string> mapList;
    private List<string> mapList = new List<string>();
    GameObject _mapImage;
    [SerializeField] Texture[] _mapTexture;
    [SerializeField] RawImage _mapRawImage;
    public int _miniMap = 0; // 방패널 맵 썸네일(레디버튼위) 0부터시작 _miniMap 0 == mapNumber 1

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



    // 팀관련
    public int TeamNumber; // 여기서 값설정해서 플레이어한테
    [SerializeField] GameObject _teamChoicePanel;
    // [SerializeField] GameObject[] _teamButtons;

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
        Debug.Log($"들어가서 맵상태 로그, 맵 : {(mapList[mapNumber])}");
    }
    private void OnDisable()
    {
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
        _startButton.onClick.RemoveListener(StartGame);
        // 방들어가면서 StartGameAddListner가 됐는데 그상태로 방장되서 시작되서 안되는것
        // 방지위해서 방패널 비활성화시 구독취소를 했어야 했음.
    }
    private void Init()
    {
        GetUI<Button>("PreviousButton").onClick.AddListener(LeaveRoom);
        GetUI<TMP_Text>("PreviousButtonText").font = kFont;
        _startButton = GetUI<Button>("StartButton");
        GetUI<TMP_Text>("StartButtonText").font = kFont;
        _startButton.onClick.AddListener(StartGame);

        // 맵선택 관련
        _mapImage = GetUI("MapImage");
        _mapRawImage = (RawImage)_mapImage.GetComponent<RawImage>();
        _mapRawImage.texture = _mapTexture[_miniMap];
        GetMapList();



        GetUI<Button>("MapSelectButton").onClick.AddListener(OpenMapList);
        _mapListPanel = GetUI("MapListPanel");
        GetUI<Button>("MapCancelButton").onClick.AddListener(CloseMapList);
        _map01Button = GetUI<Button>("MapSelectButton01");
        _map02Button = GetUI<Button>("MapSelectButton02");
        _map03Button = GetUI<Button>("MapSelectButton03");
        _map04Button = GetUI<Button>("MapSelectButton04");
        _map05Button = GetUI<Button>("MapSelectButton05");
        _map06Button = GetUI<Button>("MapSelectButton06");
        _map01Button.onClick.AddListener(SelectMap);
        _map02Button.onClick.AddListener(SelectMap);
        _map03Button.onClick.AddListener(SelectMap);
        _map04Button.onClick.AddListener(SelectMap);
        _map05Button.onClick.AddListener(SelectMap);
        _map06Button.onClick.AddListener(SelectMap);
        mapNumber = 2; // 처음맵 세팅 위한 다시 변수값선언
        // 의심되는 곳에서 디버그 찍고했는데 init() OnEnable에서는 안멈춤 .뭔가
        Debug.Log($"들어가서 맵상태 로그, 버튼이닛후 맵 : {(mapList[mapNumber])}");

        GetUI<TMP_Text>("MapNameText01").text = (mapList[1]);
        GetUI<TMP_Text>("MapNameText02").text = (mapList[2]);
        GetUI<TMP_Text>("MapNameText03").text = (mapList[3]);
        GetUI<TMP_Text>("MapNameText04").text = (mapList[4]);
        GetUI<TMP_Text>("MapNameText05").text = (mapList[5]);
        GetUI<TMP_Text>("MapNameText06").text = (mapList[6]);


        // 팀관련
        _teamChoicePanel = GetUI("TeamChoicePanel");
        GetUI<Button>("Team1").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team2").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team3").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team4").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team5").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team6").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team7").onClick.AddListener(SelectTeam);
        GetUI<Button>("Team8").onClick.AddListener(SelectTeam);

    }
    private void Update()
    {
        // SelectMap();
        if (Input.GetKeyDown(KeyCode.P))
        {
            
        }
    }

    public void SelectTeam()
    {
        string SelectedTeam = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log($"{SelectedTeam} is selected.");

        // SelectedTeam 이름이 누른 버튼과 동일하면 그버튼에 맞는 팀 넘버를 부여
        switch (SelectedTeam)
        {
            case "Team1": TeamNumber = 0;
                break;
            case "Team2": TeamNumber = 1;
                break;
            case "Team3": TeamNumber = 2;
                break;
            case "Team4": TeamNumber = 3;
                break;
            case "Team5": TeamNumber = 4;
                break;
            case "Team6": TeamNumber = 5;
                break;
            case "Team7": TeamNumber = 6;
                break;
            case "Team8": TeamNumber = 7;
                break;
        }
        PhotonNetwork.LocalPlayer.SetTeam(TeamNumber);
        Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam(TeamNumber)}");
       // Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam(TeamNumber)}");
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
    /// <summary>
    ///  EventSystem.current.currentSelectedGameObject.name로
    ///  맵이름을 비교해서 같은맵 눌린걸로 맵Int 변경
    /// </summary>
    public void SelectMap()
    {
        string SelectedMap = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log($"{SelectedMap} is selected.");
        //if (SelectedMap == _map01Button.name)
        //    mapNumber = 1;

        // SelectedMap 이름이 눌른 버튼과 동일하면 그버튼에 맞는 맵 넘버를 부여
        switch (SelectedMap)
        {
            case "MapSelectButton01": mapNumber = 1; 
                break;
            case "MapSelectButton02": mapNumber = 2;
                break;
            case "MapSelectButton03": mapNumber = 3;
                break;
            case "MapSelectButton04": mapNumber = 4;
                break;
            case "MapSelectButton05": mapNumber = 5;
                break;
            case "MapSelectButton06": mapNumber = 6;
                break;
        }
        _miniMap = mapNumber - 1;
        _mapRawImage.texture = _mapTexture[_miniMap];
        _mapListPanel.SetActive(false);
        // 맵 선택하면 => Button눌리면
        // 그냥 눌리면 ___하는 함수 만들어서 거기서 정하게
        // 그 번호로 로드 씬
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
        // ㄴ PlayerEntry에서 구현


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


    public void StartGame()
    {
        Debug.Log(mapList[mapNumber]);
        PhotonNetwork.LoadLevel(mapList[mapNumber]); // 게임 연결하면서 이름따라서 변경
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
