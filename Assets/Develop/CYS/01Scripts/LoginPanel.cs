using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;

public class LoginPanel : BaseUI
{
    [SerializeField] TMP_InputField _emailInputField;
    [SerializeField] TMP_InputField _pwInputField;

    [SerializeField] Image _nicknamePanel;
    [SerializeField] Image _verificationPanel;


    private void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        // TMP_Text
        GetUI<TMP_Text>("IDText");
        GetUI<TMP_Text>("PWText");
        
        // TMP_InputField
        _emailInputField = GetUI<TMP_InputField>("IDInputField");
        _pwInputField = GetUI<TMP_InputField>("PWInputField");
        
        // Button
        GetUI<Button>("LoginButton");
        GetUI<Button>("SignUpButton");
        GetUI<Button>("ResetPWButton");

        // VerificationPanel
        _verificationPanel = GetUI<Image>("VerifcationPanel");
        GetUI<TMP_Text>("WaitingText");
        GetUI<Button>("VerificationCancelButton");

        // NicknamePanel
        _nicknamePanel = GetUI<Image>("NicknamePanel");
        GetUI<TMP_Text>("NicknameText");
        GetUI<TMP_InputField>("NicknameInputField");
        GetUI<Button>("ConfirmButton");

    }

    public void Login()
    {
        string email = _emailInputField.text;
        string password = _pwInputField.text;

        
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
            // TODO: 이메일 인증창(Panel) 생성후 창 활성화
            // verificationPanel....
        }
        else if (user.DisplayName == "")
        {
            // TODO: 닉네임 없으면 닉네임 만드는창 활성화
            // nicknamePanel...
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

}
