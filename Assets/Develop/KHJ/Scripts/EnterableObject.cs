using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterableObject : MonoBehaviourPun
{
    [Header("Shrink rate to hide")]
    [SerializeField] private float shrinkRate = 0.1f;

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.transform.localScale *= shrinkRate;
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.transform.localScale /= shrinkRate;
    }
}
