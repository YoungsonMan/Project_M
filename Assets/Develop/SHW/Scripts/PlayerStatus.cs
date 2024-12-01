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
    [Header("MaxStatus")]
    [SerializeField] public float maxSpeed;       // 플레이어 속도
    [SerializeField] public int maxPower;         // 폭탄파워
    [SerializeField] public int maxBombCount;     // 폭탄수 

    public bool isBubble;                      // 물방울에 갇힌 상태

    // 플레이어 색상을 위한
    [SerializeField] public Color[] colors;
    // 색상 변경용 
    [SerializeField] public Color color;
    [SerializeField] Renderer bodyRenderer;

    // 팀넘버를 설정
    // -> 플레이어가 참조해서 캐릭터 색상을 설정?
    [SerializeField] public int teamNum;

    [SerializeField] GameObject canvas;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            // Set team number as the selected team number from custom properties
            photonView.RPC(nameof(SetTeamNum), RpcTarget.AllViaServer, PhotonNetwork.LocalPlayer.GetTeam());


            canvas.SetActive(true);
        }
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            GameManager.Instance.LocalPlayerStatus = this;

            // Set color as team color
            photonView.RPC(nameof(SetColor), RpcTarget.AllViaServer);
        }
    }

    private void Update()
    {
        LimitStatus();
    }

    [PunRPC]
    private void SetTeamNum(int n) => teamNum = n;

    [PunRPC]
    public void SetColor()
    {
        // Change color as team color
        color = colors[teamNum];
        for (int i = 0; i < bodyRenderer.materials.Length; i++)
        {
            bodyRenderer.materials[i].color = color;
        }

        // Notify to GameManager
        GameManager.Instance.IncreaseTeammate(teamNum);
    }


    public void LimitStatus()
    {
        // 속도제한
        if (speed > maxSpeed)
        {
            speed = maxSpeed;
        }
        // 물풍선 파워 제한
        if (power > maxPower)
        {
            power = maxPower;
        }
        // 물풍선 개수 제한 
        if (bombCount > maxBombCount)
        {
            bombCount = maxBombCount;
        }
    }

}