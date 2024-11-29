using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;

public class MainPanel : BaseUI
{
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
    [SerializeField] TMP_Dropdown _userInfoDropdown;
    // TODO : 유저 데이터베이스 구성으로 따라 뭘 보여줄지 의논해보고 구성하기.

    [SerializeField] GameObject _lobbyPanel;

    [SerializeField] Button _logOutButton;
    [SerializeField] TMP_Text _logOutText;
    private void OnEnable()
    {
        Init();
        _lobbyPanel.SetActive(true); 

    }

    private void Init()
    {
        _lobbyPanel = GetUI("LobbyPanel");
        _logOutButton = GetUI<Button>("LogOutButton");
        _logOutButton = GetUI<Button>("LogOutButton");
        _logOutButton.onClick.AddListener(LogOut);

        // TestLog();

    }

    public void JoinLobby()
    {
        // 시작하면서 그냥 로비창 열리게
        Debug.Log("로비입장 테스트 로그");
        _lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
    }
    public void LogOut()
    {
        Debug.Log("로그아웃 테스트 로그");
        PhotonNetwork.Disconnect();
    }

   // private void TestLog()
   // {
   //     FirebaseUser user = BackendManager.Auth.CurrentUser;
   //     if (user == null)
   //     {
   //         Debug.Log("플레이어가 로그인이 올바르지않습니다.");
   //         return;
   //     }
   //     Debug.Log("룸패널 테스트로그");
   //     Debug.Log($"Display Name: \t {user.DisplayName}");
   //     Debug.Log($"Email Address: \t {user.Email}");
   //     Debug.Log($"Email Verification: \t {user.IsEmailVerified}");
   //     Debug.Log($"User ID: \t\t {user.UserId}");
   //     Debug.Log("");
   // }

}
