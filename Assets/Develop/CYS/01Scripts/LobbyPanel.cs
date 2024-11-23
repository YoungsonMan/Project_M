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

    private Dictionary<string, RoomEntry> roomDictionary = new Dictionary<string, RoomEntry>();

    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        _lobbyPanel = GetUI("LobbyPanel");
        _createRoomButton = GetUI<Button>("CreateRoomButton");
        _createRoomButton.onClick.AddListener(CreateRoomMenu);
        _createRoomText = GetUI<TMP_Text>("CreateRoomText");

        _quickStartButton = GetUI<Button>("QuickStartButton");
        // _quickStartButton.onClick.AddListener(); 랜덤조인 활성화해야됨
        _quickStartText = GetUI<TMP_Text>("QuickStartText");

        _roomListPanel = GetUI("RoomListPanel");
        _roomListContent = GetUI<RectTransform>("RoomListContent");

        // TODO: 밑에 기재된것들 만들어야됨 
        // 방만들기패널
        _createRoomPanel = GetUI("CreateRoomPanel");
        _roomNameInputField = GetUI<TMP_InputField>("RoomNameInputField");
        _maxPlayerInputField = GetUI<TMP_InputField>("MaxPlayerInputField");
        GetUI<Button>("CreateRoomtButton").onClick.AddListener(CreateRoomConfirm);
        GetUI<Button>("CreateRoomCancelButton").onClick.AddListener(CreateRoomCancel);


        TestLog();
    }
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

        _roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        _maxPlayerInputField.text = "8";
    }
    public void CreateRoomCancel()
    {
        _createRoomPanel.SetActive(false);
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
        // 방 패널 만들어야됨
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

}
