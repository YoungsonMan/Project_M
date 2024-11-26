using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Firebase.Auth;


public class RoomPanel : BaseUI
{
    [SerializeField] PlayerEntry[] _playerEntries;
    [SerializeField] Button _startButton;

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
    }
    private void Init()
    {
        GetUI<Button>("PreviousButton").onClick.AddListener(LeaveRoom);
        _startButton = GetUI<Button>("StartButton");
        _startButton.onClick.AddListener(StartGame);
    }
    private void TestLog()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;
        Debug.Log("룸패널 테스트로그");
        Debug.Log($"Display Name: \t {user.DisplayName}");
        Debug.Log($"Email Address: \t {user.Email}");
        Debug.Log($"Email Verification: \t {user.IsEmailVerified}");
        Debug.Log($"User ID: \t\t {user.UserId}");
        Debug.Log("");
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
        PhotonNetwork.LoadLevel("KMS_ICE_Scene"); // 게임 연결하면서 이름따라서 변경
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


}
