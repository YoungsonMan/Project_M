using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;

public class LobbyPanel : BaseUI
{
    [SerializeField] TMP_FontAsset kFont;

    /*  LobbyPanel Objects List:
     *  + LobbyPanel (GameObject)
     *      - CreateRoomButton <Button>
     *          - CreateRoomText <TMP_Text>
     *      - QuickStartButton <Button>
     *          - QuickStartText <TMP_Text>
     *      - RoomListPanel    <Button>
     *          - RoomListViewport
     *              - RoomListContent
     *          - RoomListScrollbar
     *              - RLSBSlidingArea
     *                  - RLSBSAHandle
     *      
     */
    /* Objects in UI
     * + UserInfoDropdown (캐릭터정보창 / dropdownButton) 상시오픈 버튼누르면 올라가기
     *  - UserInfoLabel
     *  - UserInfoArrow (downArrow Image)
     *  - UserInfoTemplate
     *      - UserInfoViewport
     *          - UserInfoContent
     *              - UserInfoList
     *                  - UserInfoListBackground   // 얘들은 없앨수도??
     *                  - UserInfoListCheckmark
     *                  - UserInfoListLabel
     *
     * + LobbyPanel // 얘는 따로 스크립트 또 붙여서 운영
     * 
     * + LogOutButton
     *  - LogOutText
     * 
     */
    // [SerializeField] TMP_Dropdown _userInfoDropdown;
    // TODO : 유저 데이터베이스 구성으로 따라 뭘 보여줄지 의논해보고 구성하기.

    // mainPanel

    // [SerializeField] GameObject _lobbyPanel;
    [SerializeField] GameObject _mainPanel;  //뒤에 그냥 배경벽지
    [SerializeField] Button _logOutButton;
    [SerializeField] TMP_Text _logOutText;
    // -----------------------------------------------



    [SerializeField] GameObject _lobbyPanel;
    [SerializeField] Button _createRoomButton;
    [SerializeField] TMP_Text _createRoomText;

    [SerializeField] Button _quickStartButton;
    [SerializeField] TMP_Text _quickStartText;

    [SerializeField] GameObject _roomListPanel;
    [SerializeField] RectTransform _roomListContent;
    [SerializeField] RoomEntry _roomEntryPrefab;

    [SerializeField] GameObject _createRoomPanel;
    [SerializeField] TMP_InputField _roomNameInputField;
    [SerializeField] TMP_InputField _maxPlayerInputField;

    public int InputSelected;

    private bool _isMakingRoom = false;

    private Dictionary<string, RoomEntry> roomDictionary = new Dictionary<string, RoomEntry>();

    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        // from mainPanel

        _mainPanel = GetUI("MainPanel");

        // LogOutButton
        _logOutButton = GetUI<Button>("LogOutButton");
        _logOutButton.onClick.AddListener(LogOut);
        _logOutText = GetUI<TMP_Text>("LogOutText");
        _logOutText.font = kFont;
        _logOutText.fontSizeMin = 14;
        _logOutText.fontSize = 36;
        _logOutText.fontSizeMax = 72;


        _lobbyPanel = GetUI("LobbyPanel");
        // 방만들기 버튼
        _createRoomButton = GetUI<Button>("CreateRoomButton");
        _createRoomButton.onClick.AddListener(CreateRoomMenu);
        _createRoomText = GetUI<TMP_Text>("CreateRoomText");
        _createRoomText.fontSizeMin = 14;
        _createRoomText.fontSize = 36;
        _createRoomText.fontSizeMax = 72;
        _createRoomText.text = "방만들기";
        _createRoomText.font = kFont;

        // 빠른시작버튼
        _quickStartButton = GetUI<Button>("QuickStartButton");
        _quickStartButton.onClick.AddListener(RandomMatching); 
        _quickStartText = GetUI<TMP_Text>("QuickStartText");
        _quickStartText.fontSizeMin = 14;
        _quickStartText.fontSize = 36;
        _quickStartText.fontSizeMax = 72;
        _quickStartText.text = "빠른시작";
        _quickStartText.font = kFont;

        _roomListPanel = GetUI("RoomListPanel");
        _roomListContent = GetUI<RectTransform>("RoomListContent");

        // 방만들기패널
        _createRoomPanel = GetUI("CreateRoomPanel");
        // 방이름_Text
        GetUI<TMP_Text>("RoomName");
        GetUI<TMP_Text>("RoomName").fontSizeMin = 14;
        GetUI<TMP_Text>("RoomName").fontSize = 36;
        GetUI<TMP_Text>("RoomName").fontSizeMax = 72;
        GetUI<TMP_Text>("RoomName").text = "방이름";
        // 최대인원_Text
        GetUI<TMP_Text>("CapText");
        GetUI<TMP_Text>("CapText").fontSizeMin = 14;
        GetUI<TMP_Text>("CapText").fontSize = 36;
        GetUI<TMP_Text>("CapText").fontSizeMax = 72;
        GetUI<TMP_Text>("CapText").text = "최대인원";
        // 방이름기재_IputField
        _roomNameInputField = GetUI<TMP_InputField>("RoomNameInputField");
        GetUI<TMP_Text>("RoomNamePlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("RoomNamePlaceholder").fontSize = 22;
        GetUI<TMP_Text>("RoomNamePlaceholder").fontSizeMax = 58;
        GetUI<TMP_Text>("RoomNamePlaceholder").text = "방이름을 적어주세요";
        // 방이름기재_IputFieldText
        GetUI<TMP_Text>("RoomNameText").fontSizeMin = 14;
        GetUI<TMP_Text>("RoomNameText").fontSize = 22;
        GetUI<TMP_Text>("RoomNameText").fontSizeMax = 58;
        // 최대인원기재_IputField
        _maxPlayerInputField = GetUI<TMP_InputField>("MaxPlayerInputField");
        GetUI<TMP_Text>("MaxPlayerPlaceholder");
        GetUI<TMP_Text>("MaxPlayerPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("MaxPlayerPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("MaxPlayerPlaceholder").fontSizeMax = 58;
        GetUI<TMP_Text>("MaxPlayerPlaceholder").text = "2~8";
        // 최대인원기재_IputFieldText
        GetUI<TMP_Text>("MaxPlayerText").fontSizeMin = 14;
        GetUI<TMP_Text>("MaxPlayerText").fontSize = 22;
        GetUI<TMP_Text>("MaxPlayerText").fontSizeMax = 58;




        GetUI<Button>("CreateRoomtButton").onClick.AddListener(CreateRoomConfirm);
        GetUI<Button>("CreateRoomCancelButton").onClick.AddListener(CreateRoomCancel);


        GetUI<Button>("ExitButton").onClick.AddListener(QuitGame);
        // UpdateRoomList();
        // TestLog();
    }
    private void Update()
    {
        if (_isMakingRoom)
        {
            TabInputField();
        }
    }
    /// <summary>
    /// TabInputField
    /// Int 변수로 InputField 하나씩지정해서 탭키 누르면 ++ 되고
    /// 최대 수치를 넘어가면 처음으로 돌아가도록
    /// </summary>
    public void TabInputField()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            InputSelected++;
            if (InputSelected > 1)
                InputSelected = 0;
            SelectInputField();
        }
    }
    /// <summary>
    /// 마우스로 클릭해서 하면 정해주는
    /// </summary>
    public void SelectInputField()
    {
        switch (InputSelected)
        {
            case 0:
                _roomNameInputField.Select();
                break;
            case 1:
                _maxPlayerInputField.Select();
                break;

        }
    }
    public void RoomNameSelected() => InputSelected = 0;
    public void MaxPlayerSelected() => InputSelected = 1;


    private void TestLog()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("플레이어가 로그인이 올바르지않습니다.");
            return;
        }
        Debug.Log("Lobby Panel 테스트로그");
        Debug.Log($"Display Name: \t {user.DisplayName}");
        Debug.Log($"Email Address: \t {user.Email}");
        Debug.Log($"Email Verification: \t {user.IsEmailVerified}");
        Debug.Log($"User ID: \t\t {user.UserId}");
        Debug.Log("");
    }
    public void CreateRoomMenu()
    {
        _createRoomPanel.SetActive(true);
        _isMakingRoom = true;
        _roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        _maxPlayerInputField.text = "8";
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    public void CreateRoomCancel()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _createRoomPanel.SetActive(false);
        _isMakingRoom = false;
    }
    public void CreateRoomConfirm()
    {
        string roomName = _roomNameInputField.text;
        if (roomName == "")
        {
            // "방이름"은 무조건 있어야 만들 수 있음.
            Debug.LogWarning("방 이름을 지정해야 방을 생성할 수 있습니다.");
            return;
        }

        int maxPlayer = int.Parse(_maxPlayerInputField.text);
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 8);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, options);
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        // 방 패널 만들어야됨
    }
    public void RandomMatching()
    {
        Debug.Log("랜덤 매칭 요청");

        // 비어 있는 방이 없으면 들어가지 않는 방식
        // PhotonNetwork.JoinRandomRoom();
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        // 비어 있는 방이 없으면 새로 방을 만들어서 들어가는 방식
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 8 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // 방이 사라진 경우 + 방이 비공개인 경우 + 입장이 불가능한 방인 경우
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                // 예외 상황 : 로비 들어가자마자 사라지는 방인 경우
                if (roomDictionary.ContainsKey(info.Name) == false)
                    continue;

                Destroy(roomDictionary[info.Name].gameObject);
                roomDictionary.Remove(info.Name);
            }

            // 새로운 방이 생성된 경우
            else if (roomDictionary.ContainsKey(info.Name) == false)
            {
                RoomEntry roomEntry = Instantiate(_roomEntryPrefab, _roomListContent);
                roomDictionary.Add(info.Name, roomEntry);
                roomEntry.SetRoomInfo(info);
            }

            // 방의 정보가 변경된 경우
            else if (roomDictionary.ContainsKey((string)info.Name) == true)
            {
                RoomEntry roomEntry = roomDictionary[info.Name];
                roomEntry.SetRoomInfo(info);
            }
        }
    }
    public void ClearRoomEntries()
    {
        foreach (string name in roomDictionary.Keys)
        {
            Destroy(roomDictionary[name].gameObject);
        }
        roomDictionary.Clear();
    }

    // From mainPanel
    public void LogOut()
    {
        Debug.Log("로그아웃 테스트 로그");
        PhotonNetwork.Disconnect();
    }


    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif

    }
}
