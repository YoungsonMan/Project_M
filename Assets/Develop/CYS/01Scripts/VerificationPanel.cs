using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VerificationPanel : BaseUI
{
    [SerializeField] GameObject _verificationPanel;
    [SerializeField] GameObject _nicknamePanel;


    // Awake() 에서 돌렸었음
    //_nicknamePanel = GameObject.Find("NicknamePanel");
    // 시작하자마자 찾고 나서 비활성 하면 되지않을까
    // OnEnable에서 이메일 보내기가있는데 이메일에 보낼곳 참조가 안되서 안된다..
    // 그냥 일단 인스펙터에서 연결해서 쓰는쪽으로..
    // 그냥 닉네임패널을 인증패널 자녀로 두면 찾아서 쓸수있긴하고 인증되면 닉네임 지으니까
    // 상관은 없을거 같은데 만약에 인증하다가 인증만 하고 닉네임을 안짓고 나갔다가 
    // 다시 시작할때 닉네임 생성창이 안나올까해서 아니 근데 그러면 로그인 패널에서 bool타입으로 닉이 있나없나
    // 확인하고 없으면 닉네임 패널 열리게 하면 되려나??

    // 인증패널 밑에 두니까 인증패널 꺼져있으면 안켜짐
    // 일단 그냥 인스펙터에서 조정

    private void OnEnable()
    {
        _verificationPanel = GetUI("VerificationPanel");
        GetUI<TMP_Text>("WaitingText");
        GetUI<TMP_Text>("WaitingText").text = "본인인증을 기다리는 중입니다.\r\n이메일을 확인해주세요";
        GetUI<TMP_Text>("WaitingText").fontSizeMin = 14;
        GetUI<TMP_Text>("WaitingText").fontSize = 22;
        GetUI<TMP_Text>("WaitingText").fontSizeMin = 58;

        GetUI<Button>("VerificationCancelButton").onClick.AddListener(GoBack);

        // _nicknamePanel = GameObject.Find("NicknamePanel"); // 이게 비활성화되있으면 안되네
        // 아 일단 모르겠다 그냥 일단 지금은 Inspector에서 연결해서 쓰기
        SendVerifyEmail();
    }

    private void OnDisable()
    {
        if (_verificationRoutine != null)
        {
            StopCoroutine(_verificationRoutine);
        }
    }
    private void GoBack()
    {
        _verificationPanel.SetActive(false);
    }
    private void SendVerifyEmail()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                gameObject.SetActive(false); // 실패하면 인증기다리는창 비활성화
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Email sent successfully.");
            _verificationRoutine = StartCoroutine(VerificationRoutine());
        });
    }

    Coroutine _verificationRoutine;

    IEnumerator VerificationRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(3f);
        while (true)
        {
            // 인증 확인
            BackendManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("인증호가인");
                    gameObject.SetActive(false);
                    _nicknamePanel.gameObject.SetActive(true);
                }

                Debug.Log("User profile updated successfully.");
            });
            yield return delay; ;
        }
    }
}
