using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Local Player Status")]
    [SerializeField] PlayerStatus _localPlayerStatus;

    [Header("Team Management")]
    [Tooltip("Element represents the number of players belongs to each team")]
    [SerializeField] int[] _teammates;
    [Tooltip("Bit On/Off : Someone survived/Nobody survived")]
    [SerializeField] byte _teamFlag;

    [Header("Result UI")]
    [SerializeField] GameObject _resultUI;
    [SerializeField] TextMeshProUGUI _resultText;
    [SerializeField] PlayerInfo[] _playerInfos;

    private float _drawTolerance = 0.05f;
    private ValueTuple<float, int>[] _eliminatedTimes;

    public PlayerStatus LocalPlayerStatus { set { _localPlayerStatus = value; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Init;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Init;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;
            
        Debug.Log($"[GameManager]: Init");
        _teammates = new int[8];
        _teamFlag = 0;
        _eliminatedTimes = new ValueTuple<float, int>[8];
        _gameOverCoroutine = null;

        _resultUI.SetActive(false);
        foreach (PlayerInfo info in _playerInfos)
        {
            info.gameObject.SetActive(false);
        }
    }

    private void GameOver()
    {
        if (_gameOverCoroutine == null)
            StartCoroutine(GameOverRoutine());
    }

    private bool IsGameOver()
    {
        return _teamFlag == 0 || (_teamFlag & (_teamFlag - 1)) == 0;
    }

    private int GetWinner()
    {
        int winner = 0;
        while (_teamFlag > 1)
        {
            _teamFlag >>= 1;
            winner++;
        }

        return winner;
    }

    private List<int> GetTies()
    {
        _drawTolerance = 0.05f;
        List<int> ties = new();

        Array.Sort(_eliminatedTimes, (x, y) => y.Item1.CompareTo(x.Item1));
        for (int i = 1; i < _eliminatedTimes.Length; i++)
        {
            if (_eliminatedTimes[i - 1].Item1 - _eliminatedTimes[i].Item1 < _drawTolerance)
                ties.Add(_eliminatedTimes[i].Item2);
        }

        if (ties.Count > 0)
            ties.Add(_eliminatedTimes[0].Item2);

        return ties;
    }

    private void ShowResult()
    {
        List<int> ties = null;
        int winner;

        // Winner not exist (draw)
        if(_teamFlag == 0)
        {
            // Set proper main result text to each player
            ties = GetTies();

            if (ties.Contains(_localPlayerStatus.teamNum))
                _resultText.SetDraw();
            else
                _resultText.SetLose();

            // Set each players' result and nickname
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                int teamNum = PhotonNetwork.PlayerList[i].GetTeam();
                PlayerInfo info = _playerInfos[i];

                if (ties.Contains(teamNum))
                    info.resultText.SetDraw();
                else
                    info.resultText.SetLose();

                info.nicknameText.text = PhotonNetwork.PlayerList[i].NickName;

                info.gameObject.SetActive(true);
            }
        }
        // Winner exist
        else
        {
            // Set proper main result text to each player
            winner = GetWinner();

            if (_localPlayerStatus.teamNum == winner)
                _resultText.SetWin();
            else
                _resultText.SetLose();

            // Set each players' result and nickname
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                int teamNum = PhotonNetwork.PlayerList[i].GetTeam();
                PlayerInfo info = _playerInfos[i];

                if (teamNum == winner)
                    info.resultText.SetWin();
                else
                    info.resultText.SetLose();

                info.nicknameText.text = PhotonNetwork.PlayerList[i].NickName;

                info.gameObject.SetActive(true);
            }
        }

        // Show
        _resultUI.SetActive(true);
    }

    private Coroutine _gameOverCoroutine;
    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(_drawTolerance);

        ShowResult();

        yield return new WaitForSeconds(5f);

        _resultUI.SetActive(false);

        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    /// <summary>
    /// 팀원 수를 1만큼 증가시킵니다
    /// </summary>
    /// <param name="teamNum">해당 플레이어의 팀 번호</param>
    public void IncreaseTeammate(int teamNum)
    {
        _teammates[teamNum]++;
        _teamFlag |= (byte)(1 << teamNum);
    }

    /// <summary>
    /// 팀원 수를 1만큼 감소시킵니다
    /// </summary>
    /// <param name="teamNum">해당 플레이어의 팀 번호</param>
    public void DecreaseTeammate(int teamNum)
    {
        if (--_teammates[teamNum] == 0)
        {
            byte mask = (byte)~(1 << teamNum);
            _teamFlag &= mask;
            _eliminatedTimes[teamNum] = new ValueTuple<float, int>(Time.time, teamNum);

            if (IsGameOver())
                GameOver();
        }
    }
}
