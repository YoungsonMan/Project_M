using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WaterBomb : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private WaitForSeconds _delay;
    private IObjectPool<WaterBomb> _objectPool;

    public IObjectPool<WaterBomb> ObjectPool { set { _objectPool = value; } }

    private void OnEnable()
    {
        Deactivate();
    }

    private void Start()
    {
        _delay = new WaitForSeconds(_lifeTime);
    }

    private void Deactivate() => StartCoroutine(DeactivateRoutine());

    IEnumerator DeactivateRoutine()
    {
        yield return _delay;

        _objectPool.Release(this);
    }


    /// <summary>
    /// 물폭탄이 설치될 위치를 지정합니다.
    /// </summary>
    /// <param name="placerPosition">폭탄을 설치하는 오브젝트의 position</param>
    public void SetLocation(Vector3 placerPosition)
    {
        transform.position = placerPosition;
    }
}
