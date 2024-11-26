using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class SignUpPanel : BaseUI
{
    [SerializeField] TMP_InputField _signUpIDInputField;    // 0
    [SerializeField] TMP_InputField _signUpPWInputField;    // 1
    [SerializeField] TMP_InputField _PWConfirmInputField;   // 2

    public int InputSelected;

    [SerializeField] GameObject _checkPopup;
    [SerializeField] GameObject _alreadyExistMsg;
    [SerializeField] GameObject _availableAddressMsg;
    
    AuthError error = AuthError.EmailAlreadyInUse;
    private void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        // ID & PW
        // TMP_Text
        GetUI<TMP_Text>("SignUpIDText");
        GetUI<TMP_Text>("SignUpPWText");
        GetUI<TMP_Text>("PWConfirmText");

        // TMP_InputField
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");
        GetUI<TMP_Text>("SignUpIDPlaceholder").text = "example@gmail.com";
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");
        GetUI<TMP_Text>("SUPWinputPlaceholder").text = "Cannot be too simple like\n qwer1234 & longer than 6 chars";
        _PWConfirmInputField = GetUI<TMP_InputField>("PWConfirmInputField");
        GetUI<TMP_Text>("PWConfirmplaceholder").text = "Must to match with the password.";

        // Button
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);  // Init 바꿔야함
        GetUI<Button>("SignUpCancelButton").onClick.AddListener(Close);  // Init 바꿔야함

        // CheckPopup
        _checkPopup = GetUI("CheckPopup");
        _alreadyExistMsg = GetUI("AlreadyExistMsg");
        _availableAddressMsg = GetUI("AvailableAddressMsg");
        GetUI<Button>("CheckPopupButton").onClick.AddListener(Close);
    }

    void Update()
    {
        TabInputField();
        // 업데이트에서 돌리고 있는거 말고 다른방법있으면 변경
        // 일단 가입되고 비활성화되면 쓸일 없으니까 가입당시에만 돌아가는거정도는...
        CheckAvailability();
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
            if (InputSelected > 2)
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
                _signUpIDInputField.Select();
                break;
            case 1:
                _signUpPWInputField.Select();
                break;
             case 2:
                _PWConfirmInputField.Select();
                break;

        }
    }
    // 직접 클릭하면 바뀌는데 InputSelected는 안바뀌니까
    // 각자 매서드로 InputSelected바뀌게설정
    public void EmailSelected() => InputSelected = 0;
    public void PwSelected() => InputSelected = 1;
    public void ConfirmationSelected() => InputSelected = 2;


    public void SignUp()
    {
        Debug.Log("SignUpButton 테스트 로그 : 회원가입버튼 눌림");

        string _email = _signUpIDInputField.text;
        string _password = _signUpPWInputField.text;
        string _confirmPW = _PWConfirmInputField.text;

        if(_email.IsNullOrEmpty() || _password.IsNullOrEmpty() || _confirmPW.IsNullOrEmpty())
        {
            // TODO : 시간이된다면 팝업창으로 띄우기..?
            Debug.LogWarning("입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요");
            return;
        }
        if (_password != _confirmPW)
        {
            Debug.LogWarning("패스워드가 일치하지 않습니다.");
        }
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                /* 일단 인증에러 팝업창나게하는거 중지
                Firebase.FirebaseException exception = task.Exception.InnerException as Firebase.FirebaseException;
                switch ((AuthError)exception.ErrorCode)
                {

                    case AuthError.EmailAlreadyInUse:
                        Debug.LogWarning($"이메일이 이미 사용중입니다.");
                        _checkPopup.SetActive(true);
                        break;
                }
                */

                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            // 체크팝업 _checkPopup.SetActive(true);

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
            gameObject.SetActive(false);
        });

    }

    /// <summary>
    /// 사용가능한 이메일인지 체크 :
    /// 가능/불가능에 따라 다른 메시지 출력
    /// </summary>
    public void CheckAvailability()
    {
        if (error == AuthError.EmailAlreadyInUse)
        {
            _alreadyExistMsg.SetActive(true);
            _availableAddressMsg.SetActive(false);
        }
        else
        {
            _availableAddressMsg.SetActive(true);
            _alreadyExistMsg.SetActive(false);
        }
    }

    public void Close()
    {
        Debug.Log("닫기버튼");
        gameObject.SetActive(false);
    }
}
