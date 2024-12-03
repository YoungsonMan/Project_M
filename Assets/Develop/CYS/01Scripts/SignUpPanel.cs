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
    [SerializeField] TMP_FontAsset kFont;

    [SerializeField] TMP_InputField _signUpIDInputField;    // 0
    [SerializeField] TMP_InputField _signUpPWInputField;    // 1
    [SerializeField] TMP_InputField _PWConfirmInputField;   // 2

    public int InputSelected;

    [SerializeField] GameObject _checkPopup;
    [SerializeField] TMP_Text _checkPopupMsg;
    
    AuthError error = AuthError.EmailAlreadyInUse;
    private void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        // ID & PW
        // TMP_Text
        GetUI<TMP_Text>("SignUpIDText").font = kFont;
        GetUI<TMP_Text>("SignUpIDText").text = "이메일";
        GetUI<TMP_Text>("SignUpIDText").fontSizeMin = 14;
        GetUI<TMP_Text>("SignUpIDText").fontSize = 36;
        GetUI<TMP_Text>("SignUpIDText").fontSizeMax = 72;

        GetUI<TMP_Text>("SignUpPWText").font = kFont;
        GetUI<TMP_Text>("SignUpPWText").text = "비밀번호";
        GetUI<TMP_Text>("SignUpPWText").fontSizeMin = 14;
        GetUI<TMP_Text>("SignUpPWText").fontSize = 36;
        GetUI<TMP_Text>("SignUpPWText").fontSizeMax = 72;

        GetUI<TMP_Text>("PWConfirmText").font = kFont;
        GetUI<TMP_Text>("PWConfirmText").text = "비밀번호 확인";
        GetUI<TMP_Text>("PWConfirmText").fontSizeMin = 14;
        GetUI<TMP_Text>("PWConfirmText").fontSize = 36;
        GetUI<TMP_Text>("PWConfirmText").fontSizeMax = 72;

        // TMP_InputField
        _signUpIDInputField = GetUI<TMP_InputField>("SignUpIDInputField");
        GetUI<TMP_Text>("SignUpIDPlaceholder").text = "example@gmail.com";
        GetUI<TMP_Text>("SignUpIDPlaceholder").font = kFont;
        GetUI<TMP_Text>("SignUpIDPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("SignUpIDPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("SignUpIDPlaceholder").fontSizeMax = 58;
        // 아이디 입력란 문자
        GetUI<TMP_Text>("SignUpIDInputText").font = kFont;
        GetUI<TMP_Text>("SignUpIDInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("SignUpIDInputText").fontSize = 22;
        GetUI<TMP_Text>("SignUpIDInputText").fontSizeMax = 58;

        // 비밀번호 입력
        _signUpPWInputField = GetUI<TMP_InputField>("SignUpPWInputField");
        GetUI<TMP_Text>("SUPWinputPlaceholder").text = "비밀번호를 입력하세요";
        GetUI<TMP_Text>("SUPWinputPlaceholder").font = kFont;
        GetUI<TMP_Text>("SUPWinputPlaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("SUPWinputPlaceholder").fontSize = 22;
        GetUI<TMP_Text>("SUPWinputPlaceholder").fontSizeMax = 58;
        // 비밀번호 입력란 문자
        GetUI<TMP_Text>("SUPWInputText").font = kFont;
        GetUI<TMP_Text>("SUPWInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("SUPWInputText").fontSize = 22;
        GetUI<TMP_Text>("SUPWInputText").fontSizeMax = 58;

        // 비밀번호 확인란
        _PWConfirmInputField = GetUI<TMP_InputField>("PWConfirmInputField");
        GetUI<TMP_Text>("PWConfirmplaceholder").text = "위와 동일해야 합니다.";
        GetUI<TMP_Text>("PWConfirmplaceholder").font = kFont;
        GetUI<TMP_Text>("PWConfirmplaceholder").fontSizeMin = 14;
        GetUI<TMP_Text>("PWConfirmplaceholder").fontSize = 22;
        GetUI<TMP_Text>("PWConfirmplaceholder").fontSizeMax = 58;
        // 비밀번호 확인란 입력란 문자
        GetUI<TMP_Text>("PWConfirmInputText").font = kFont;
        GetUI<TMP_Text>("PWConfirmInputText").fontSizeMin = 14;
        GetUI<TMP_Text>("PWConfirmInputText").fontSize = 22;
        GetUI<TMP_Text>("PWConfirmInputText").fontSizeMax = 58;





        // Button
        GetUI<Button>("SignUpConfirmButton").onClick.AddListener(SignUp);  // Init 바꿔야함
        GetUI<Button>("SignUpCancelButton").onClick.AddListener(Close);  // Init 바꿔야함
        GetUI<TMP_Text>("SignUpConfirmText").font = kFont;
        GetUI<TMP_Text>("SignUpCancelText").font = kFont;

        // CheckPopup
        _checkPopup = GetUI("CheckPopup");
        _checkPopupMsg = GetUI<TMP_Text>("CheckPopupMsg");
        _checkPopupMsg.font = kFont;
        _checkPopupMsg.fontSizeMin = 14;
        _checkPopupMsg.fontSize = 22;
        _checkPopupMsg.fontSizeMax = 58;
        GetUI<TMP_Text>("CheckPopupButtonText").font = kFont;
        GetUI<TMP_Text>("CheckPopupButtonText").fontSizeMin = 14;
        GetUI<TMP_Text>("CheckPopupButtonText").fontSize = 22;
        GetUI<TMP_Text>("CheckPopupButtonText").fontSizeMax = 58;
        GetUI<Button>("CheckPopupButton").onClick.AddListener(ClosePopup);
    }

    void Update()
    {
        TabInputField();
        // 업데이트에서 돌리고 있는거 말고 다른방법있으면 변경
        // 일단 가입되고 비활성화되면 쓸일 없으니까 가입당시에만 돌아가는거정도는...
        // CheckAvailability();
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

            _checkPopup.SetActive(true);
            _checkPopupMsg.text = "입력하지 않은 곳이 있습니다. \n다시한번 확인해주세요";
            return;
        }
        if (_password != _confirmPW)
        {
            Debug.LogWarning("패스워드가 일치하지 않습니다.");
            _checkPopup.SetActive(true);
            _checkPopupMsg.text = "패스워드가 일치하지 않습니다.";
            return;
        }
        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(_email, _password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                _checkPopup.SetActive(true);
                _checkPopupMsg.text = "회원가입이 취소되었습니다.";
                return;
            }
            if (task.IsFaulted)
            {
                // 일단 인증에러 팝업창나게하는거 중지
                Firebase.FirebaseException exception = task.Exception.InnerException as Firebase.FirebaseException;
                switch ((AuthError)exception.ErrorCode)
                {

                    case AuthError.EmailAlreadyInUse:
                        Debug.LogWarning($"이메일이 이미 사용중입니다.");
                        _checkPopup.SetActive(true);
                        _checkPopupMsg.text = "이메일이 이미 사용중입니다.";
                        break;
                    case AuthError.WeakPassword:
                        Debug.LogWarning($"비밀번호가 너무 쉽습니다.");
                        _checkPopup.SetActive(true);
                        _checkPopupMsg.text = "비밀번호가 너무 쉽습니다. \n다른비밀번호를 사용해주세요.";
                        break;
                    case AuthError.WrongPassword:
                        Debug.LogWarning($"잘못된 비밀번호입니다.: {exception.ErrorCode}");
                        _checkPopupMsg.text = "비밀번호가 틀립니다.. \n바르게 입력하세요.";
                        break;

                }
                

                Debug.LogWarning("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                _checkPopup.SetActive(true);
                _checkPopupMsg.text = $"{task.Exception} 에러가 일어났습니다.";
                return;
            }
            // 체크팝업 _checkPopup.SetActive(true);

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");
            _checkPopupMsg.text = $"회원가입을 축하드립니다! \n{result.User.UserId}로 인증메일을 확인해주세요.";
            gameObject.SetActive(false);
        });
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);

    }
    public void ClosePopup()
    {
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        _checkPopup.SetActive(false);
    }

    public void Close()
    {
        Debug.Log("닫기버튼");
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.CLICK);
        gameObject.SetActive(false);
    }
}
