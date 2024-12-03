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

    [SerializeField] GameObject _notificationPanel;
    // string _notification;

    private void Start()
    {
        Init();
        // TestLogin();
        TestLogin();
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGM(SoundManager.E_BGM.LOGIN);
    }

    private void OnEnable()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM(SoundManager.E_BGM.LOGIN);
        }
    }

    private void OnDisable()
    {
        
    }
    private void Update()
    {
        TabInputField();
        // 엔터키에서 로그인 버튼 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
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
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    // 직접 클릭하면 바뀌는데 InputSelected는 안바뀌니까
    // 각자 매서드로 InputSelected바뀌게설정
    public void EmailSelected()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        InputSelected = 0;
    } 

    public void PwSelected()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        InputSelected = 1; 
    }



    //  private void Start()
    //  {
    //      Init();
    //  }
    private void Init()
    {
        // TMP_Text
        // ID_Text
        GetUI<TMP_Text>("IDText").font = kFont;
        GetUI<TMP_Text>("IDText").fontSizeMin = 14;
        GetUI<TMP_Text>("IDText").fontSize = 36;
        GetUI<TMP_Text>("IDText").fontSizeMax = 72;
        // PW_Text
        GetUI<TMP_Text>("PWText").font = kFont;
        GetUI<TMP_Text>("PWText").fontSizeMin = 14;
        GetUI<TMP_Text>("PWText").fontSize = 36;
        GetUI<TMP_Text>("PWText").fontSizeMax = 72;
        
        GetUI<TMP_Text>("IDText").text = "이메일";
        GetUI<TMP_Text>("PWText").text = "비밀번호";

        // TMP_InputField
        // ID
        _emailInputField = GetUI<TMP_InputField>("IDInputField");
        GetUI<TMP_Text>("IDInputPlaceholder").text = "example@gmail.com";
        GetUI<TMP_Text>("IDInputPlaceholder").font = kFont;
        GetUI<TMP_Text>("IDInputPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("IDInputPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("IDInputPlaceholder").fontSizeMax = 58;
        GetUI<TMP_Text>("IDInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("IDInputText").fontSize = 22;
        GetUI<TMP_Text>("IDInputText").fontSizeMax = 58;

        // PW
        _pwInputField = GetUI<TMP_InputField>("PWInputField");
        GetUI<TMP_Text>("PWInputPlaceholder").text = "비밀번호를 입력하세요";
        GetUI<TMP_Text>("PWInputPlaceholder").font = kFont;
        GetUI<TMP_Text>("PWInputPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("PWInputPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("PWInputPlaceholder").fontSizeMax = 58;
        GetUI<TMP_Text>("PWInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("PWInputText").fontSize = 22;
        GetUI<TMP_Text>("PWInputText").fontSizeMax = 58;

        // Button
        // LoginButton
        GetUI<Button>("LoginButton").onClick.AddListener(Login);
        GetUI<TMP_Text>("LoginButtonText").fontSizeMin = 14;
        GetUI<TMP_Text>("LoginButtonText").fontSize = 36;
        GetUI<TMP_Text>("LoginButtonText").fontSizeMax = 72;
        GetUI<TMP_Text>("LoginButtonText").text = "로그인";
        // SignUpButton
        GetUI<Button>("SignUpButton").onClick.AddListener(GoToSignUp);
        GetUI<TMP_Text>("SignUpText").fontSizeMin = 14;
        GetUI<TMP_Text>("SignUpText").fontSize = 36;
        GetUI<TMP_Text>("SignUpText").fontSizeMax = 72;
        GetUI<TMP_Text>("SignUpText").text = "회원가입";
        // ResetPWButton
        GetUI<Button>("ResetPWButton").onClick.AddListener(ResetPW);
        GetUI<TMP_Text>("ResetPWText").fontSizeMin = 14;
        GetUI<TMP_Text>("ResetPWText").fontSize = 36;
        GetUI<TMP_Text>("ResetPWText").fontSizeMax = 72;
        GetUI<TMP_Text>("ResetPWText").text = "비밀번호 찾기";

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
        GetUI<TMP_Text>("RestPwIDText").fontSizeMin = 14;
        GetUI<TMP_Text>("RestPwIDText").fontSize = 36;
        GetUI<TMP_Text>("RestPwIDText").fontSizeMax = 72;
        GetUI<TMP_Text>("RestPwIDText").text = "이메일";
        // placeInput
        GetUI<TMP_Text>("RestPwIDInputTextPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("RestPwIDInputTextPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("RestPwIDInputTextPlaceholder").fontSizeMax = 58;
        GetUI<TMP_Text>("RestPwIDInputTextPlaceholder").text = "이메일을 입력하세요";
        // RestPwIDInputText
        GetUI<TMP_Text>("RestPwIDInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("RestPwIDInputText").fontSize = 22;
        GetUI<TMP_Text>("RestPwIDInputText").fontSizeMax = 58;

        _restPwIDInputField = GetUI<TMP_InputField>("RestPwIDInputField");
        GetUI<Button>("RestPwConfirmButton").onClick.AddListener(SendResetPwEmail);
        GetUI<TMP_Text>("RestPwConfirmText").text = "초기화 메일발송";
        GetUI<TMP_Text>("RestPwConfirmText").fontSizeMin = 14;
        GetUI<TMP_Text>("RestPwConfirmText").fontSize = 22;
        GetUI<TMP_Text>("RestPwConfirmText").fontSizeMax = 58;

        GetUI<Button>("RestPwCancelButton").onClick.AddListener(CancelFindingPW);
        GetUI<TMP_Text>("RestPwCancelText").text = "취소";
        GetUI<TMP_Text>("RestPwCancelText").fontSizeMin = 14;
        GetUI<TMP_Text>("RestPwCancelText").fontSize = 36;
        GetUI<TMP_Text>("RestPwCancelText").fontSizeMax = 72;

        // 알림창
        _notificationPanel = GetUI("NotificationPanel");
        // 알림 메세지
        GetUI<TMP_Text>("NotificationText").font = kFont;
        GetUI<TMP_Text>("NotificationText").fontSizeMin = 14;
        GetUI<TMP_Text>("NotificationText").fontSize = 22;
        GetUI<TMP_Text>("NotificationText").fontSizeMax = 40;
      //  GetUI<TMP_Text>("NotificationText").text = _notification;
        // 확인/닫기버튼
        GetUI<Button>("NotificationButton").onClick.AddListener(CloseNotification);





        // 종료버튼
        GetUI<Button>("ExitButton").onClick.AddListener(QuitGame);
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
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    public void Login()
    {
        Debug.Log("LoginButton 테스트 로그");
        
        string email = _emailInputField.text;
        if (email == "")
        {
            Debug.Log("이메일을 입력 해 주세요.");
            GetUI<TMP_Text>("NotificationText").text = "이메일을 입력 해 주세요.";
            OpenNotifiaction();
        }
        string password = _pwInputField.text;
        if (password == "")
        {
            Debug.Log("비밀번호를 입력 하세요.");
            GetUI<TMP_Text>("NotificationText").text = "비밀번호를 입력 하세요.";
            OpenNotifiaction();
        }

        
        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
           .ContinueWithOnMainThread(task =>
           {
               if (task.IsCanceled)
               {
                   Debug.LogWarning("SignInWithEmailAndPasswordAsync was canceled.");
                   GetUI<TMP_Text>("NotificationText").text = "로그인 인증이 취소되었습니다. ";
                   OpenNotifiaction();
                   return;
               }
               if (task.IsFaulted)
               {
                   Debug.LogWarning("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                   GetUI<TMP_Text>("NotificationText").text = $"올바른 아이디/비밀번호를\n 입력해주세요.";
                   OpenNotifiaction();
                   return;
               }

               AuthResult result = task.Result;
               Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
               CheckUserInfo();
           });

        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
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
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
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
                Debug.LogWarning("SendPasswordResetEmailAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogWarning("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("Password reset email sent successfully.");
            _resetPwPanel.SetActive(false);
        });
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
    }
    public void CancelFindingPW()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _resetPwPanel.SetActive(false);
    }
    public void OpenNotifiaction()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _notificationPanel.SetActive(true);
    }
    public void CloseNotification()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _notificationPanel.SetActive(false);
    }

}
