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
    [SerializeField] TMP_InputField _signUpIDInputField;
    [SerializeField] TMP_InputField _signUpPWInputField;
    [SerializeField] TMP_InputField _PWConfirmInputField;


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
        GetUI<TMP_Text>("SUPWinputPlaceholder").text = "Cannot be too simple like\n qwer1234";
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
        // 업데이트에서 돌리고 있는거 말고 다른방법있으면 변경
        // 일단 가입되고 비활성화되면 쓸일 없으니까 가입당시에만 돌아가는거정도는...
        CheckAvailability();
    }


    public void SignUp()
    {
        Debug.Log("SignUpButton 테스트 로그 : 회원가입!");

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
                Firebase.FirebaseException exception = task.Exception.InnerException as Firebase.FirebaseException;
                switch ((AuthError)exception.ErrorCode)
                {

                    case AuthError.EmailAlreadyInUse:
                        Debug.LogWarning($"이메일이 이미 사용중입니다.");
                        _checkPopup.SetActive(true);
                        break;
                }

                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            _checkPopup.SetActive(true);
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
