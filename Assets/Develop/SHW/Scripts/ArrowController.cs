using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviourPun
{
    private void Awake()
    {
        if(photonView.IsMine == false)
        {
            enabled = false;
        }
    }
}
