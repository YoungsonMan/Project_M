using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;

public class LoginPanel : BaseUI
{
    [SerializeField] TMP_InputField _emailInputField;
    [SerializeField] TMP_InputField _pwInputField;

    // [SerializeField] Image _nicknamePanel;
    // [SerializeField] Image _verificationPanel;

    // 밑에 따로 스크립트 빼서 관리하기???
    [SerializeField] GameObject _signUpPanel;
    [SerializeField] GameObject _verificationPanel;
    [SerializeField] GameObject _nicknamePanel;

    private void OnEnable()
    {
        Init();
        TestLogin();
    }
  //  private void Start()
  //  {
  //      Init();
  //  }
    private void Init()
    {
        // TMP_Text
        GetUI<TMP_Text>("IDText");
        GetUI<TMP_Text>("PWText");
        
        // TMP_InputField
        _emailInputField = GetUI<TMP_InputField>("IDInputField");
        GetUI<TMP_Text>("IDInputPlaceholder").text = "example@gmail.com";
        _pwInputField = GetUI<TMP_InputField>("PWInputField");
        GetUI<TMP_Text>("PWInputPlaceholder").text = "Type your password in";

        // Button
        GetUI<Button>("LoginButton").onClick.AddListener(Login);
        GetUI<Button>("SignUpButton").onClick.AddListener(GoToSignUp);
        GetUI<Button>("ResetPWButton");

        // VerificationPanel
        _verificationPanel = GetUI("VerificationPanel");
        GetUI<TMP_Text>("WaitingText");
        GetUI<Button>("VerificationCancelButton");

        // NicknamePanel
        _nicknamePanel = GetUI("NicknamePanel");
        GetUI<TMP_Text>("NicknameText");
        GetUI<TMP_InputField>("NicknameInputField");
        GetUI<Button>("ConfirmButton");

        _signUpPanel = GetUI("SignUpPanel");
        
    }

    /// <summary>
    /// 테스트 편하게하려는 로그인 코드
    /// </summary>
    public void TestLogin()
    {
        _emailInputField.text = "ysc1350@gmail.com";
        _pwInputField.text = "q1w2e3r4";
    }


    public void SubscirbesEvents()
    {
       // GetUI<Button>("LoginButton").onClick.AddListener(Login);
    }
    public void GoToSignUp()
    {
        Debug.Log("회원가입 창 열기");
        _signUpPanel.SetActive(true);
    }
    public void Login()
    {
        Debug.Log("LoginButton 테스트 로그");
        
        string email = _emailInputField.text;
        if (email == "")
        {
            Debug.Log("Email is empty, please put your eamil");
        }
        string password = _pwInputField.text;
        if (password == "")
        {
            Debug.Log("Password is empty, please put your eamil");
        }

        
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
           .ContinueWithOnMainThread(task =>
           {
               if (task.IsCanceled)
               {
                   Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                   return;
               }
               if (task.IsFaulted)
               {
                   Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                   return;
               }

               AuthResult result = task.Result;
               Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
               CheckUserInfo();
           });

        
    }
    public void CheckUserInfo()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;         // 혹시나 로그인안되서 유저가 없으면 return

        Debug.Log($"Display Name: \t {user.DisplayName}");
        Debug.Log($"Email Address: \t {user.Email}");
        Debug.Log($"Email Verification: \t {user.IsEmailVerified}");
        Debug.Log($"User ID: \t\t {user.UserId}");

        if (user.IsEmailVerified == false)
        {
            // 이메일인증 안됐으면 인증창 활성화
            _verificationPanel.gameObject.SetActive(true);
        }
        else if (user.DisplayName == "")
        {
            // 닉네임 없으면 닉네임 만드는창 활성화
            _nicknamePanel.gameObject.SetActive(true);
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }


}
