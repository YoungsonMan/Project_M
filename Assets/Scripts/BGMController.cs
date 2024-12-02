using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 적절한 BGM 재생
        switch (scene.name)
        {
            case "FarmStroy":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.FARM);
                break;

            case "ICE_Villege10":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.ICE_VILLAGE);
                break;

            case "Pirate14":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.PIRATE);
                break;

            case "Forest07":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.FOREST);
                break;

            case "Factory07":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.FACTORY);
                break;

            case "TomatoStroy":
                SoundManager.Instance.PlayBGM(SoundManager.E_BGM.TOMATO);
                break;

            default:
                Debug.LogWarning($"BGM 설정이 없는 씬: {scene.name}");
                SoundManager.Instance.StopBGM();
                break;
        }
    }
}
