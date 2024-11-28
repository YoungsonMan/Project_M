using Photon.Realtime;
using System.Diagnostics;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public static class CustomProperty
{
    public const string READY = "Ready";
    public const string LOAD = "load";

    // (구현중) 팀 커스텀 프로퍼티
    public const string TEAM = "Team";

    public static void SetReady(this Player player, bool ready)
    {
        PhotonHashtable customProperty = new PhotonHashtable();
        customProperty[READY] = ready;
        player.SetCustomProperties(customProperty);
    }
    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
        }
        else
        {
            return false;
        }
    }

    public static void SetLoad(this Player player, bool load)
    {
        PhotonHashtable customProperty = new PhotonHashtable();
        customProperty[LOAD] = load;
        player.SetCustomProperties(customProperty);
    }
    public static bool GetLoad(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        if (customProperty.ContainsKey(LOAD))
        {
            return (bool)customProperty[LOAD];
        }
        else
        {
            return false;
        }
    }

    // 팀 설정 커스컴 프로퍼티
    /// <summary>
    /// 팀을 설정하는 함수 입니다.
    /// num에 팀의 번호를 넣어주세요(0~7번)
    /// PlayerEntry의 SetPlayer에서 사용했을 때, 잘 동작됨을 확인했습니다.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="num"></param>
    public static void SetTeam(this Player player, int num)
    {
        PhotonHashtable customProperty = new PhotonHashtable();
        customProperty[TEAM] = num;
        player.SetCustomProperties(customProperty);
    }
    public static void GetTeam(this Player player, out int num)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        // 키값이 있는 경우
        if (customProperty.ContainsKey(TEAM))
        {
            // 입력한 변수에 키값을 할당?
            num = (int)customProperty[TEAM];
            return;
        }
        else
        {
            // 키값이 없을 경우 임의로 0번 팀으로 강제배치?
            num = 0;
            return;
        }
    }
}
