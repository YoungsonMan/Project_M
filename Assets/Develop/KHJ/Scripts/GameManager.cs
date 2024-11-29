using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] int[] _teammates;
    [SerializeField] byte _teamFlag;

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
    }

    private void GameOver()
    {
        int winner = GetWinner();
        Debug.Log($"[GameManager]: Winner: Team {winner}");
        PhotonNetwork.LoadLevel(0);
        PhotonNetwork.CurrentRoom.IsOpen = true;
    }

    private bool IsGameOver()
    {
        return _teamFlag != 0 && (_teamFlag & (_teamFlag - 1)) == 0;
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

            if (IsGameOver())
                GameOver();
        }
    }
}
