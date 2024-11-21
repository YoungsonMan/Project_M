using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

public class VerificationPanel : BaseUI
{
    [SerializeField] GameObject _verificationPanel;
    [SerializeField] GameObject _nicknamePanel;


    // Awake() ���� ���Ⱦ���
    //_nicknamePanel = GameObject.Find("NicknamePanel");
    // �������ڸ��� ã�� ���� ��Ȱ�� �ϸ� ����������
    // OnEnable���� �̸��� �����Ⱑ�ִµ� �̸��Ͽ� ������ ������ �ȵǼ� �ȵȴ�..
    // �׳� �ϴ� �ν����Ϳ��� �����ؼ� ����������..
    // �׳� �г����г��� �����г� �ڳ�� �θ� ã�Ƽ� �����ֱ��ϰ� �����Ǹ� �г��� �����ϱ�
    // ����� ������ ������ ���࿡ �����ϴٰ� ������ �ϰ� �г����� ������ �����ٰ� 
    // �ٽ� �����Ҷ� �г��� ����â�� �ȳ��ñ��ؼ� �ƴ� �ٵ� �׷��� �α��� �гο��� boolŸ������ ���� �ֳ�����
    // Ȯ���ϰ� ������ �г��� �г� ������ �ϸ� �Ƿ���??

    // �����г� �ؿ� �δϱ� �����г� ���������� ������
    // �ϴ� �׳� �ν����Ϳ��� ����

    private void OnEnable()
    {
        _verificationPanel = GetUI("VerificationPanel");
        // _nicknamePanel = GameObject.Find("NicknamePanel"); // �̰� ��Ȱ��ȭ�������� �ȵǳ�
        // �� �ϴ� �𸣰ڴ� �׳� �ϴ� ������ Inspector���� �����ؼ� ����
        SendVerifyEmail();
    }

    private void OnDisable()
    {
        if (_verificationRoutine != null)
        {
            StopCoroutine(_verificationRoutine);
        }
    }
    private void SendVerifyEmail()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                gameObject.SetActive(false); // �����ϸ� ������ٸ���â ��Ȱ��ȭ
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
            // ���� Ȯ��
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
                    Debug.Log("����ȣ����");
                    gameObject.SetActive(false);
                    _nicknamePanel.gameObject.SetActive(true);
                }

                Debug.Log("User profile updated successfully.");
            });
            yield return delay; ;
        }
    }
}