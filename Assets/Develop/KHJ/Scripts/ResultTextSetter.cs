using TMPro;
using UnityEngine;

public static class ResultTextSetter
{
    public static void SetWin(this TextMeshProUGUI tmp)
    {
        tmp.text = "Win!!";
        tmp.color = Color.cyan;
    }

    public static void SetDraw(this TextMeshProUGUI tmp)
    {
        tmp.text = "Draw!!";
        tmp.color = Color.green;
    }

    public static void SetLose(this TextMeshProUGUI tmp)
    {
        tmp.text = "Lose..";
        tmp.color = Color.gray;
    }
}
