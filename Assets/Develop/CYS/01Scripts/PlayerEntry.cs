using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
//using System.Drawing;
using TMPro;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : BaseUI
{
    // [SerializeField] GameObject _playerImage; ("PlayerImage") 나중에 
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _readyText;
    [SerializeField] Button _readyButton;

    private void OnEnable()
    {
        Init();        
    }
    private void Init()
    {
      //  _nameText = GetUI<TMP_Text>("PlayerNameText");
      //  _readyText = GetUI<TMP_Text>("ReadyText");
      //  _readyButton = GetUI<Button>("ReadyButton");

    }
    public void SetPlayer(Player player)
    {
        if(player.IsMasterClient)
        {
            _nameText.text = $"MASTER\n{player.NickName}";
            // 일단 "MASTER" 글씨, 추후 이미지라던가 의논후 변경
        }
        else
        {
            _nameText.text = player.NickName;
        }
        _readyButton.gameObject.SetActive(true);
        _readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        // 플레이어가 본인이지 확인 -> 레디버튼 player =isLocal 도 가능

        if (player.GetReady())
        {
            _readyText.text = "Ready";
            // _readyButton.transition.;
        }
        else
        {
            _readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        _readyText.text = "";
        _nameText.text = "None";
        _readyButton.gameObject.SetActive(false);
    }

    public void Ready()
    {
        // !레디 -> 레디 || 레디 -> !레디 
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        ready = !ready;

        PhotonNetwork.LocalPlayer.SetReady(ready);
        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
            _readyText.text = "Ready";
            Debug.Log($"준비상태: {ready}");
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
            _readyText.text = "";
        }
    }

}
