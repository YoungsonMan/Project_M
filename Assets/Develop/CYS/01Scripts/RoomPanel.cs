using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.GraphView;


public class RoomPanel : BaseUI
{
    [Header("방이름 인원수")]
    [SerializeField] TMP_Text _roomTitle;
    [SerializeField] TMP_Text _roomCapacity;
    // 방제목, 인원수 갱신을 위한 변수들
    private string _roomName, _roomMembers;


    [Header("한글폰트")]
    [SerializeField] TMP_FontAsset kFont;
    [Header("플레이어관련")]
    [SerializeField] PlayerEntry[] _playerEntries;
    [SerializeField] Button _startButton;

    // Map 관련
    [Header("맵관련")]
    // [SerializeField] List <string> mapList;
    private List<string> mapList = new List<string>();
    GameObject _mapImage;
    [SerializeField] Texture[] _mapTexture;
    [SerializeField] RawImage _mapRawImage;
    [SerializeField] TMP_Text _mapTitleText;    // 맵이름
    public int _miniMap = 0;                    // 방패널 맵 썸네일(레디버튼위) 0부터시작 _miniMap 0 == mapNumber 1
    GameObject _mapListPanel;
    GameObject _map01, _map02, _map03, _map04, _map05, _map06;
    Button _map01Button, _map02Button, _map03Button, _map04Button, _map05Button, _map06Button;
    public int mapNumber = 1;


    // 팀관련
    [Header("팀관련")]
    public int TeamNumber; // 여기서 값설정해서 플레이어한테
    [SerializeField] GameObject _teamChoicePanel;
    [SerializeField] GameObject[] _teamButtons;
    [SerializeField] GameObject[] _teamColor;
    public int teamColorNum;
    Image _teamImage;



    // 캐릭터관련
    [Header("캐릭터관련")]
    [SerializeField] GameObject _charChoicePanel;
    [SerializeField] GameObject[] _character;
    [SerializeField] Texture[] _charTexture;
    [SerializeField] RawImage _charRawImage;
    GameObject _playerImage;
    public int charNumber; // 우측상단 캐릭터 선택창 캐릭터번호


    [Header("레디버튼관련")]
    [SerializeField] Button _readyButton;
    [SerializeField] GameObject _readyTextBox;
    [SerializeField] TMP_Text _readyText;
    private readonly Color readyColor = new Color(1, 0.8f, 0, 1);
    private readonly Color notReadyColor = Color.white;
    private Player _player;
    PlayerEntry _playerEntry;
    [SerializeField] TMP_Text _readyPopText;

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
        _roomTitle = GetUI<TMP_Text>("RoomTitle");
        _roomTitle.font = kFont;
        _roomCapacity = GetUI<TMP_Text>("RoomCapacity");
        _roomCapacity.font = kFont;
        _roomName = _roomTitle.text;
        _roomMembers = _roomCapacity.text;



        GetUI<Button>("PreviousButton").onClick.AddListener(LeaveRoom);
        GetUI<TMP_Text>("PreviousButtonText").font = kFont;
        _startButton = GetUI<Button>("StartButton");
        GetUI<TMP_Text>("StartButtonText").font = kFont;
        GetUI<TMP_Text>("StartButtonText").fontSizeMin = 14;
        GetUI<TMP_Text>("StartButtonText").fontSize = 22;
        GetUI<TMP_Text>("StartButtonText").fontSizeMax = 58;
        _startButton.onClick.AddListener(StartGame);

        // 맵선택 관련
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
        // MapButtonInit(PhotonNetwork.LocalPlayer);
        _miniMap = 1;  // 시작맵 토마토설정
        mapNumber = 2; // 처음맵 세팅 위한 다시 변수값선언 (시작맵토마토...)
        _mapImage = GetUI("MapImage");
        _mapRawImage = (RawImage)_mapImage.GetComponent<RawImage>();
        _mapRawImage.texture = _mapTexture[_miniMap];
        _mapTitleText = GetUI<TMP_Text>("MapTitleText");
        //_mapTitleText.text = (mapList[mapNumber]);
        _mapTitleText.fontSizeMin = 14;
        _mapTitleText.fontSize = 22;
        _mapTitleText.fontSizeMax = 58;
        SetRoomInfo(PhotonNetwork.CurrentRoom);
        KoreanMap();
        Debug.Log($"들어가서 맵상태 로그, 버튼이닛후 맵 : {(mapList[mapNumber])}");

        GetUI<TMP_Text>("MapNameText01").text = "팜스트로이";
        GetUI<TMP_Text>("MapNameText02").text = "토마토스트로이";
        GetUI<TMP_Text>("MapNameText03").text = "아이스빌리지 10";
        GetUI<TMP_Text>("MapNameText04").text = "해적 14";
        GetUI<TMP_Text>("MapNameText05").text = "팩토리 07";
        GetUI<TMP_Text>("MapNameText06").text = "포레스트07";

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

        // 캐릭터관련
        _charChoicePanel = GetUI("CharChoicePanel");
        // charNumber = 0; //처음에 캐릭터 0 (보이)
        PhotonNetwork.LocalPlayer.SetCharacter(charNumber);
        GetUI<Button>("Character1").onClick.AddListener(SelectCharacter);
        GetUI<Button>("Character2").onClick.AddListener(SelectCharacter);
        GetUI<Button>("Character3").onClick.AddListener(SelectCharacter);

        _playerImage = GetUI("PlayerImage");
        _charRawImage = (RawImage)_playerImage.GetComponent<RawImage>();
        _charRawImage.texture = _charTexture[charNumber];
        // 위에 이렇게 이미지 받는걸 PlayerEntry에서도 하면 될듯?? (GetCharacter)써서?


        // 레디버튼 레디 (스타트버튼옆)
        _readyTextBox = GetUI("ReadyTextBox");
        _readyText = GetUI<TMP_Text>("ReadyText");
        _readyText.font = kFont;
        // 레디텍스트박스 밑에 레디버튼 (평상시 흰색글씨에 레디하면 노랑색되기위한구조)++처음에 만들고수정하다보니이렇게됨
        // 구조조정하려다가 망할뻔해서 일단 그냥 두기로함.
        _readyButton = GetUI<Button>("ReadyButton");
        _readyButton.onClick.AddListener(Ready);
        // 레디하면 플레이어 위에 나오는 READY텍스트
        _readyPopText = GetUI<TMP_Text>("ReadyPopText");
        _readyPopText.font = kFont;

    }
    private void Update()
    {
        // 방제가 바뀌거나 / 인원수가 바뀌면 업데이트 
        if (_roomName != PhotonNetwork.CurrentRoom.Name ||
            _roomMembers != $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}")
        {
            SetRoomInfo(PhotonNetwork.CurrentRoom);
        }

        // SelectMap();
        if (Input.GetKeyDown(KeyCode.P))
        {


        }
    }
    public void SetPlayer(Player player)
    {
        // KMS 플레이어 가져오기.
        _player = player;
        _readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        
        {
 
            UpdateReadyState();
        }

    }

    public void Ready()
    {
        // !레디 -> 레디 || 레디 -> !레디 
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        ready = !ready;

        PhotonNetwork.LocalPlayer.SetReady(ready);
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        // KMS 레디 부분갱신.
        {
            //UpdateReadyState();

            if (ready)
            {
                PhotonNetwork.LocalPlayer.SetReady(true);
                _readyText.text = "Ready";
                Debug.Log($"준비상태: {ready}");
            }
            else
            {
                PhotonNetwork.LocalPlayer.SetReady(false);
                _readyText.text = "";
            }
        }
    }
    private void UpdateReadyState()
    {
        if (_player.GetReady())
        {
            _readyText.text = "Ready";
            _readyText.color = readyColor;
            _readyPopText.text = "READY";
        }
        else
        {
            _readyText.text = "Ready";
            _readyText.color = notReadyColor;
            _readyPopText.text = "";
        }
    }
    // 뭔가 지금 딱히 쓸데가 없는거같음
    public GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] children = new GameObject[parent.transform.childCount];
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            children[i] = parent.transform.GetChild(i).gameObject;
        }
        return children;
    }
    public void SelectCharacter()
    {
        string SelectedChar = EventSystem.current.currentSelectedGameObject.name;
        switch (SelectedChar)
        {
            case "Character1":
                charNumber = 0;
                break;
            case "Character2":
                charNumber = 1;
                break;
            case "Character3":
                charNumber = 2;
                break;
            default:
                Debug.LogWarning("잘못된 캐릭터 선택!");
                return;
        }
        PhotonNetwork.LocalPlayer.SetCharacter(charNumber);
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        //_charRawImage.texture = _charTexture[charNumber];
        Debug.Log($"캐릭터번호: {charNumber}");

        // KMS 플레이어 정보 갱신
        UpdatePlayers();
    }
    public void SelectTeam()
    {
        string SelectedTeam = EventSystem.current.currentSelectedGameObject.name;
        // Debug.Log($"{SelectedTeam} is selected.");

        // SelectedTeam 이름이 누른 버튼과 동일하면 그버튼에 맞는 팀 넘버를 부여
        switch (SelectedTeam)
        {
            case "Team1":
                TeamNumber = 0;
                break;
            case "Team2":
                TeamNumber = 1;
                break;
            case "Team3":
                TeamNumber = 2;
                break;
            case "Team4":
                TeamNumber = 3;
                break;
            case "Team5":
                TeamNumber = 4;
                break;
            case "Team6":
                TeamNumber = 5;
                break;
            case "Team7":
                TeamNumber = 6;
                break;
            case "Team8":
                TeamNumber = 7;
                break;
        }
        PhotonNetwork.LocalPlayer.SetTeam(TeamNumber);
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam()}");
        // Debug.Log($"선택하신 팀번호: {PhotonNetwork.LocalPlayer.GetTeam(TeamNumber)}");

        // KMS 색상 또한 선택시 플레이어 갱신
        UpdatePlayers();
    }
    void OpenMapList()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _mapListPanel.SetActive(true);
    }
    void CloseMapList()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _mapListPanel.SetActive(false);
    }
    void GetMapList()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            mapList.Add(System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)));
        }
        
    }


    public void SelectMap()
    {

        string SelectedMap = EventSystem.current.currentSelectedGameObject.name;
        //Debug.Log($"{SelectedMap} is selected.");  // 클릭한Object 이름 따오기
        //if (SelectedMap == _map01Button.name)
        //    mapNumber = 1;

        // SelectedMap 이름이 눌른 버튼과 동일하면 그버튼에 맞는 맵 넘버를 부여
        switch (SelectedMap)
        {
            case "MapSelectButton01":
                mapNumber = 1;
                break;
            case "MapSelectButton02":
                mapNumber = 2;
                break;
            case "MapSelectButton03":
                mapNumber = 3;
                break;
            case "MapSelectButton04":
                mapNumber = 4;
                break;
            case "MapSelectButton05":
                mapNumber = 5;
                break;
            case "MapSelectButton06":
                mapNumber = 6;
                break;
        }
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _mapListPanel.SetActive(false);
        // 맵 전달
        // 방장일때만 가능하도록 | 방장아니면 그냥 창닫기
        // PhotonNetwork.CurrentRoom.SetMap(mapNumber);
        // 맵 선택하면 => Button눌리면
        // 그냥 눌리면 ___하는 함수 만들어서 거기서 정하게
        // 그 번호로 로드 씬
        if (PhotonNetwork.IsMasterClient)
        {
            _miniMap = mapNumber - 1;
            _mapRawImage.texture = _mapTexture[_miniMap];
            PhotonNetwork.CurrentRoom.SetMapNum(mapNumber);
            PhotonNetwork.CurrentRoom.SetMapName(mapList[mapNumber]);
            Debug.Log($"맵 번호 {mapNumber}가 설정되었습니다.");
            // KoreanMap();
            //_mapTitleText.text = (mapList[mapNumber]);
        }
    }
    private void KoreanMap()
    {
        if (mapNumber == 1)
            _mapTitleText.text = "팜스트로이";
        else if (mapNumber == 2)
            _mapTitleText.text = "토마토 스트로이";
        else if (mapNumber == 3)
            _mapTitleText.text = "아이스빌리지 10";
        else if (mapNumber == 4)
            _mapTitleText.text = "해적 14";
        else if (mapNumber == 5)
            _mapTitleText.text = "팩토리 07";
        else if (mapNumber == 6)
            _mapTitleText.text = "포레스트07";
    }

    /// <summary>
    /// 맵 선택 정보 업데이트
    /// </summary>
    /// <param name="mapNumber"></param>
    public void UpdateMapUI(int mapNumber)
    {
        mapNumber = PhotonNetwork.CurrentRoom.GetMapNum()-1;
        if (_mapRawImage == null || _mapTexture == null) return;

        if (mapNumber >= 0 && mapNumber < _mapTexture.Length)
        {
            _mapRawImage.texture = _mapTexture[mapNumber];
        }
    }
    public void UpdateMapName(string mapName)
    {
        mapName = PhotonNetwork.CurrentRoom.GetMapName();
        if (_mapTitleText.text != mapName)
        {
            _mapTitleText.text = mapName;
        }
        KoreanMap();
    }

    public void UpdatePlayers()
    {
        if (_playerEntries == null || _playerEntries.Length == 0)
        {
            Debug.LogWarning("PlayerEntries 배열이 초기화되지 않았습니다.");
            return;
        }

        foreach (PlayerEntry entry in _playerEntries)
        {
            if (entry != null)
            {
                entry.SetEmpty();
            }
            else
            {
                Debug.LogWarning("PlayerEntry가 null입니다.");
            }
        }
        // 현제 방에 있는 모든 플레이어 가져오기
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // Number 할당 전은 -1  이니까, 그 플레이어는 할당하지 않는다.
            if (player.GetPlayerNumber() == -1)
                continue;

            int number = player.GetPlayerNumber();
            _playerEntries[number].SetPlayer(player);

            // KMS 캐릭터 정보 업데이트
            {
                int characterId = player.GetCharacter();
                _playerEntries[number].UpdateCharacter(characterId);

                // KMS 팀 정보 업데이트
                int teamNumber = player.GetTeam();
                _playerEntries[number].UpdateTeam(teamNumber);
            }
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

    public void UpdateRoomProperty(Hashtable properties)
    {
        // 맵변경, num & UI
        if (properties.ContainsKey(CustomProperty.MAP))
        {
            UpdateMapUI(mapNumber);
        }
        // 맵이름UI 변경
        if (properties.ContainsKey(CustomProperty.MAPNAME))
        {
            UpdateMapName(mapList[mapNumber]);
        }


    }
    /// <summary>
    ///  플레이어 프로퍼티 업데이트
    ///  레디상황
    /// </summary>
    /// <param name="targetPlayer"></param>
    /// <param name="properties"></param>
    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties)
    {
        if (properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }

        // KMS CHARACTER 변경 처리
        if (properties.ContainsKey(CustomProperty.CHARACTER))
        {
            Debug.Log($"{targetPlayer.NickName}의 캐릭터가 변경됨: {targetPlayer.GetCharacter()}");

            // 변경된 캐릭터 UI 갱신
            UpdatePlayers();
        }
        if (properties.ContainsKey(CustomProperty.TEAM))
        {
            Debug.Log($"{targetPlayer.NickName}의 팀을 변경: {targetPlayer.GetTeam()}");

            // 변경된 캐릭터 UI 갱신
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
        Debug.Log($"{oldPlayer.NickName}님이 퇴장하였습니다.");
        UpdatePlayers();
    }
    private bool CheckAllReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 다 돌면서 한명이라도 false면 리턴
            if (player.GetReady() == false)
                return false;
        }   // 다 돌고 다 레디면
        return true;
    }

    public void SetRoomInfo(RoomInfo info)
    {
        _roomTitle.text = info.Name;
        //_roomCapacity.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        int currentMember = info.PlayerCount;
        _roomCapacity.text = $"{PhotonNetwork.PlayerList.Count()}/{info.MaxPlayers}";

    }
    public void StartGame()
    {
        Debug.Log(mapList[mapNumber]);
        PhotonNetwork.LoadLevel(mapList[mapNumber]); // 게임 연결하면서 이름따라서 변경
        PhotonNetwork.CurrentRoom.IsOpen = false;
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
    }

    public void LeaveRoom()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        PhotonNetwork.LeaveRoom();
    }


}
