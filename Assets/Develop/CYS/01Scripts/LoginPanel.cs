using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System;

public class LoginPanel : BaseUI
{
    [SerializeField] TMP_FontAsset kFont;


    [SerializeField] TMP_InputField _emailInputField;   // 0
    [SerializeField] TMP_InputField _pwInputField;      // 1

    public int InputSelected;

    // [SerializeField] Image _nicknamePanel;
    // [SerializeField] Image _verificationPanel;

    // 밑에 따로 스크립트 빼서 관리하기???
    [SerializeField] GameObject _signUpPanel;
    [SerializeField] GameObject _verificationPanel;
    [SerializeField] GameObject _nicknamePanel;

    [SerializeField] GameObject _resetPwPanel;
    TMP_InputField _restPwIDInputField;

    SoundManager soundManager = SoundManager.Instance;

    private void OnEnable()
    {
        Init();
        TestLogin();
    }
    private void OnDisable()
    {
        soundManager.StopBGM();
    }
    private void Update()
    {
        TabInputField();
        // 엔터키에서 로그인 버튼 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            soundManager.PlaySFX(SoundManager.E_SFX.BOMB_EXPLOSION);
            Login();
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
            case 0: _emailInputField.Select();
                break;
            case 1: _pwInputField.Select();
                break;
        }
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    // 직접 클릭하면 바뀌는데 InputSelected는 안바뀌니까
    // 각자 매서드로 InputSelected바뀌게설정
    public void EmailSelected()
    {
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
        InputSelected = 0;
    } 

    public void PwSelected()
    {
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
        InputSelected = 1; 
    }



    //  private void Start()
    //  {
    //      Init();
    //  }
    private void Init()
    {
        soundManager.StopBGM();
        soundManager.PlayBGM(SoundManager.E_BGM.LOGIN);


        // TMP_Text
        GetUI<TMP_Text>("IDText").font = kFont;
        GetUI<TMP_Text>("PWText").font = kFont;
        
        // TMP_InputField
        _emailInputField = GetUI<TMP_InputField>("IDInputField");
        GetUI<TMP_Text>("IDInputPlaceholder").text = "example@gmail.com";
        GetUI<TMP_Text>("IDInputPlaceholder").font = kFont;
        _pwInputField = GetUI<TMP_InputField>("PWInputField");
        GetUI<TMP_Text>("PWInputPlaceholder").text = "Type your password in";
        GetUI<TMP_Text>("PWInputPlaceholder").font = kFont;

        // Button
        GetUI<Button>("LoginButton").onClick.AddListener(Login);
        GetUI<Button>("SignUpButton").onClick.AddListener(GoToSignUp);
        GetUI<Button>("ResetPWButton").onClick.AddListener(ResetPW);

        // VerificationPanel
        _verificationPanel = GetUI("VerificationPanel");
        GetUI<TMP_Text>("WaitingText").font = kFont;
        GetUI<Button>("VerificationCancelButton");

        // NicknamePanel
        _nicknamePanel = GetUI("NicknamePanel");
        GetUI<TMP_Text>("NicknameText").font = kFont;
        GetUI<TMP_InputField>("NicknameInputField");
        GetUI<Button>("ConfirmButton");

        _signUpPanel = GetUI("SignUpPanel");


        // ResetPasswordPanel
        _resetPwPanel = GetUI("ResetPwPanel");
        _restPwIDInputField = GetUI<TMP_InputField>("RestPwIDInputField");
        GetUI<Button>("RestPwConfirmButton").onClick.AddListener(SendResetPwEmail);
        GetUI<Button>("RestPwCancelButton").onClick.AddListener(CancelFindingPW);
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
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
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

        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
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

    private void ResetPW()
    {
        _resetPwPanel.SetActive(true);
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    /// <summary>
    /// 비밀번호 재설정
    /// InputField에 적힌 주소로 비밀번호 재설정 이메일 보내기
    /// </summary>
    public void SendResetPwEmail()
    {
        string email = _restPwIDInputField.text;
        BackendManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
            _resetPwPanel.SetActive(false);
        });
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    public void CancelFindingPW()
    {
        soundManager.PlaySFX(SoundManager.E_SFX.CLICK);
        _resetPwPanel.SetActive(false);
    }


}
