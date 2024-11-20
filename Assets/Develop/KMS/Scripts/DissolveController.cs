using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveController : MonoBehaviour
{
    private Material originalMaterial;      // 원본 메터리얼
    private Material dissolveMaterial;      // Dissolve 효과가 적용될 메터리얼
    public Shader dissolveShader;           // Dissolve 효과를 위한 셰이더
    public Texture noiseTexture;            // Dissolve 효과에 사용될 노이즈 텍스처
    public float dissolveSpeed = 0.5f;      // 오브젝트가 소멸되는 속도 조절 변수
    public float delayTime = 1.5f;          // 딜레이 타임 지난 후 소멸 시작

    private void Start()
    {
        // 원본 메터리얼 저장
        originalMaterial = GetComponent<Renderer>().material;

        // Dissolve 메터리얼 생성
        dissolveMaterial = new Material(dissolveShader);

        // 원본 메터리얼의 텍스처와 색상 복사
        dissolveMaterial.mainTexture = originalMaterial.mainTexture;
        dissolveMaterial.SetColor("_Color", originalMaterial.color);
        dissolveMaterial.SetTexture("_NoiseTex", noiseTexture);

        // 소멸시작
        StartDestruction();
    }

    /// <summary>
    /// 소멸시작시 Dissolve Shader가 든 메터리얼 장착.
    /// </summary>
    public void StartDestruction()
    {
        GetComponent<Renderer>().material = dissolveMaterial;
        StartCoroutine(DissolveEffect());
    }

    /// <summary>
    /// 점진적으로 DissolveAmount 값을 증가시켜 오브젝트를 점차적으로 소멸시키는 효과를 구현
    /// </summary>
    private IEnumerator DissolveEffect()
    {
        yield return new WaitForSeconds(delayTime);

        float dissolveAmount = 0;

        // 점진적 소멸.
        while (dissolveAmount < 0.3f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;

        }

        // 소멸 완료 후 오브젝트 제거.
        Destroy(gameObject);
    }
}
