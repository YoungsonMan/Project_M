using Photon.Realtime;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using static Photon.Pun.UtilityScripts.PunTeams;


public class PlayerStatus : MonoBehaviourPun
{
    [SerializeField] public float speed;       // 플레이어 속도
    [SerializeField] public int power;         // 폭탄파워
    [SerializeField] public int bombCount;     // 폭탄수 

    public bool isBubble;                      // 물방울에 갇힌 상태

    // 플레이어 색상을 위한
    [SerializeField] public Color[] colors;
    // 팀넘버를 설정
    // -> 플레이어가 참조해서 캐릭터 색상을 설정?
    [SerializeField] public int teamNum;

    private void Awake( )
    {
        // 팀 설정에서 번호가 매겨지면 주석 풀어서 팀 설정
        // PhotonNetwork.LocalPlayer.GetTeam(out teamNum);
    }

    private void Update()
    {
        LimitStatus();
    }

    public void LimitStatus()
    {
        // 속도제한
        if (speed > 10)
        {
            speed = 10;
        }
        // 물풍선 파워 제한
        if (power > 8)
        {
            power = 8;
        }
        // 물풍선 개수 제한 
        if (bombCount > 8)
        {
            bombCount = 8;
        }
    }

}