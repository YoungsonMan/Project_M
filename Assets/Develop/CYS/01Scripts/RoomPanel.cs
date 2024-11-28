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
    public int chosenMap;

    

   // enum MapButtons { _map01Button, _map02Button, _map03Button, _map04Button, _map05Button, _map06Button }
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

        // 맵선택 관련
        GetMapList();
        _mapImage = GetUI("MapImage");
        _mapRawImage = (RawImage)_mapImage.GetComponent<RawImage>();
        _mapRawImage.texture = _mapTexture[_miniMap];



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

        GetUI<TMP_Text>("MapNameText01").text = (mapList[1]);
        GetUI<TMP_Text>("MapNameText02").text = (mapList[2]);
        GetUI<TMP_Text>("MapNameText03").text = (mapList[3]);
        GetUI<TMP_Text>("MapNameText04").text = (mapList[4]);
        GetUI<TMP_Text>("MapNameText05").text = (mapList[5]);
        GetUI<TMP_Text>("MapNameText06").text = (mapList[6]);
    }
    private void Update()
    {
        // SelectMap();
        if (Input.GetKeyDown(KeyCode.P))
        {
            
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
            case "MapSelectButton01":
                mapNumber = 0;
                break;
            case "MapSelectButton02":
                mapNumber = 1;
                break;
            case "MapSelectButton03":
                mapNumber = 2;
                break;
            case "MapSelectButton04":
                mapNumber = 3;
                break;
            case "MapSelectButton05":
                mapNumber = 4;
                break;
            case "MapSelectButton06":
                mapNumber = 5;
                break;

        }
        // 맵 선택하면 => Button눌리면
        // 그냥 눌리면 ___하는 함수 만들어서 거기서 정하게
        // 그 번호로
        // if (SelectedMap == _map01Button.name)
        // {
        //     Debug.Log("맵눌리는거테스트\n 1번눌렸습니다.");
        //     SelectedMap = "";
        // }


        //  for (int i = 0; i < mapList.Count; i++)
        //  {
        //      if ( == _map01Button)
        //      mapNumber = i;
        //      switch (mapNumber)
        //      {
        //          case 1: mapNumber = 0;
        //              break;
        //          case 2: mapNumber = 1; 
        //              break;
        //          case 3: mapNumber = 2;
        //              break;
        //          case 4: mapNumber = 3;
        //              break;
        //          case 5: mapNumber = 4;
        //              break;
        //          case 6: mapNumber = 5;
        //              break;
        //
        //      }
        //  }
    }
    private void ChoseMap()
    { 
    }
    public void Map1Selected()
    {
        mapNumber = 1;
        _miniMap = mapNumber-1;
        _mapListPanel.SetActive(false);
        _mapRawImage.texture = _mapTexture[_miniMap];
    }
    public void Map2Selected()
    {
        mapNumber = 2;
        _miniMap = mapNumber - 1;
        _mapListPanel.SetActive(false);
        _mapRawImage.texture = _mapTexture[_miniMap];
    }
    public void Map3Selected()
    {
        mapNumber = 3;
        _miniMap = mapNumber - 1;
        _mapListPanel.SetActive(false);
        _mapRawImage.texture = _mapTexture[_miniMap];
    }
    public void Map4Selected()    
    {
        mapNumber = 4;
        _miniMap = mapNumber-1;
        _mapListPanel.SetActive(false);
        _mapRawImage.texture = _mapTexture[_miniMap];
    }
    public void Map5Selected()    
    {
        mapNumber = 5;
        _miniMap = mapNumber-1;
        _mapListPanel.SetActive(false);
    }
    public void Map6Selected()   
    {
        mapNumber = 6;
        _miniMap = mapNumber - 1;
        _mapListPanel.SetActive(false);
        _mapRawImage.texture = _mapTexture[_miniMap];
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
