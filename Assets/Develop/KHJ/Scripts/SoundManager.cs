using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    // BGM types
    public enum E_BGM
    {
        BGM_TITLE,

        BGM_FACTORY,
        BGM_FARM,
        BGM_FOREST,
        BGM_ICEVILLAGE,
        BGM_PIRATE,
        BGM_TOMATO,
    }

    // SFX typs
    public enum E_SFX
    {
        SFX_BUTTON,

        SFX_EXPLOSION,

        SFX_WIN,
        SFX_LOSE,
        SFX_DRAW,
    }

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] _bgms;
    [SerializeField] AudioClip[] _sfxs;

    [Header("Audio Source")]
    [SerializeField] AudioSource _audioBgm;
    [SerializeField] AudioSource _audioSfx;

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

    /// <summary>
    /// 원하는 BGM을 재생합니다.
    /// </summary>
    /// <param name="bgmIdx">재생을 원하는 BGM</param>
    public void PlayBGM(E_BGM bgmIdx)
    {
        _audioBgm.clip = _bgms[(int)bgmIdx];
        _audioBgm.Play();
    }

    /// <summary>
    /// 재생 중인 BGM을 정지합니다.
    /// </summary>
    public void StopBGM()
    {
        _audioBgm.Stop();
    }

    /// <summary>
    /// 원하는 SFX를 한 번 재생합니다.
    /// </summary>
    /// <param name="sfxIdx">재생을 원하는 SFX</param>
    public void PlaySFX(E_SFX sfxIdx)
    {
        _audioSfx.PlayOneShot(_sfxs[(int)sfxIdx]);
    }
}