using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;        // 인증관련
using Firebase.Database;    // 데이터베이스
using Firebase.Extensions;

public class BackendManager : MonoBehaviour
{
    public static BackendManager Instance { get; private set; }

    private FirebaseApp _app;
    public static FirebaseApp App { get { return Instance._app; } }
    
    private FirebaseAuth _auth;
    public static FirebaseAuth Auth { get {return Instance._auth; } }

    private FirebaseDatabase _database;
    public static FirebaseDatabase Database { get { return Instance._database; } }

    private void Awake()
    {
        SetSingleton();
    }
    void Start()
    {
        CheckDependency();
    }


    private void SetSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 호환성 체크 & 수정
    /// 비동기식, Async (Task, MultiThread방식으로 체크)
    /// 비동기식 작업이 끝났을때(로그인됐을때) 이것 하겠다.
    /// CheckAndFixDependenciesAsync() 하고나면 ContinueWithOnMainThread(task =>...
    /// </summary>
    private void CheckDependency()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if(task.Result == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;
                _auth = FirebaseAuth.DefaultInstance;
                _database = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase 사용준비완료. ");

            }
            else
            {
                Debug.LogError ($"Cannot resolve all Firebase dependencies: {task.Result}");
                _app = null;
                _auth = null;
                _database = null;
            }
        });
    }
}
