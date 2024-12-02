using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WaterBomb : MonoBehaviour, IExplosionInteractable
{
    private enum E_DirectionType
    {
        Up, Down, Right, Left, SIZE
    }
    [Header("Waterbomb")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float _lifeTime;
    [SerializeField] private int _range = 1;
    [SerializeField] private LayerMask _waterBombLayerMask;

    [Header("Explosion Effect")]
    [SerializeField] private ExplosionEffect[] _effects;

    [Header("Collision Check")]
    [SerializeField] private SphereCollider _collider;
    [SerializeField] private SphereCollider _trigger;

    private ObjectPool<WaterBomb> _objectPool;

    private bool _isExploded;
    private float _lag;
    private float _explosionDuration = 0.2f;
    private WaitForSeconds _explosionDelay;

    public ObjectPool<WaterBomb> ObjectPool { set { _objectPool = value; } }
    public int Range { set { _range = value; } }
    public float Lag { set { _lag = value; } }

    private void OnEnable()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        _isExploded = false;

        InitWaterStream();

        Deactivate();
    }

    private void Start()
    {
        _explosionDelay = new WaitForSeconds(_explosionDuration);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        ResetCollisionIgnorance(other);
    }

    private void OnDisable()
    {
        transform.position = new Vector3(-50, 0, -50);  // setting zone

        if(_deactiveCoroutine != null)
        {
            StopCoroutine(_deactiveCoroutine);
            _deactiveCoroutine = null;
        }
    }

    private void Deactivate() => StartCoroutine(DeactivateRoutine());

    private Coroutine _deactiveCoroutine;
    IEnumerator DeactivateRoutine()
    {
        Debug.Log($"{gameObject.name}({gameObject.GetInstanceID()}) will explode after {_lifeTime - _lag}s.");
        yield return new WaitForSeconds(_lifeTime - _lag);

        Explode();
    }

    IEnumerator WaitExplosionRoutine()
    {
        // Disable renderer and collider
        _renderer.enabled = false;
        _collider.enabled = false;

        yield return _explosionDelay;

        // Return to pool
        _objectPool.Release(this);
    }

    private void InitWaterStream()
    {
        foreach (var effect in _effects)
        {
            effect.gameObject.SetActive(false);
        }
    }

    private void SetCollisionIgnorance()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _collider.radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Debug.Log("Set");
                Physics.IgnoreCollision(_collider, collider);
            }
        }
    }

    private void ResetCollisionIgnorance(Collider other)
    {
        Debug.Log("Reset");
        Physics.IgnoreCollision(_collider, other, false);
    }

    private void Explode()
    {
        _isExploded = true;
        SoundManager.Instance.PlaySFX(SoundManager.E_SFX.BOMB_EXPLOSION);

        // Set ranges with judging interactables
        // center
        ProceedWaterStream();
        // 4-way directions
        int upEnd = ProceedWaterStream(transform.forward, _range) * (int)E_DirectionType.SIZE + (int)E_DirectionType.Up;
        int downEnd = ProceedWaterStream(-transform.forward, _range) * (int)E_DirectionType.SIZE + (int)E_DirectionType.Down;
        int rightEnd = ProceedWaterStream(transform.right, _range) * (int)E_DirectionType.SIZE + (int)E_DirectionType.Right;
        int leftEnd = ProceedWaterStream(-transform.right, _range) * (int)E_DirectionType.SIZE + (int)E_DirectionType.Left;

        // Visual Effect
        // center
        _effects[0].gameObject.SetActive(true);
        // 4-way directions
        for (int range = 1; range < _effects.Length; range += (int)E_DirectionType.SIZE)
        {
            if (range + (int)E_DirectionType.Up <= upEnd)
                _effects[range + (int)E_DirectionType.Up].gameObject.SetActive(true);
            if (range + (int)E_DirectionType.Down <= downEnd)
                _effects[range + (int)E_DirectionType.Down].gameObject.SetActive(true);
            if (range + (int)E_DirectionType.Right <= rightEnd)
                _effects[range + (int)E_DirectionType.Right].gameObject.SetActive(true);
            if (range + (int)E_DirectionType.Left <= leftEnd)
                _effects[range + (int)E_DirectionType.Left].gameObject.SetActive(true);
        }

        // Wait explosion effects
        StartCoroutine(WaitExplosionRoutine());
    }

    private void ProceedWaterStream()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (Collider collider in colliders)
        {
            IExplosionInteractable interactable = null;
            Transform curTransform = collider.transform;
            while (curTransform != null)
            {
                interactable = curTransform.GetComponent<IExplosionInteractable>();
                if (interactable != null)
                    break;

                curTransform = curTransform.parent;
            }

            if (interactable == null)
                continue;

            // Interact
            interactable.Interact();
        }
    }

    private int ProceedWaterStream(Vector3 direction, int maxRange)
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 offset = new Vector3(0, 0.5f, 0);
        bool isContinue = true;

        for(int range = 0; range < maxRange; range++)
        {
            if (Physics.Raycast(origin + offset, direction, out hit, 1f))
            {
                // Find IExplosionInteractable
                IExplosionInteractable interactable = null;
                Transform curTransform = hit.transform;
                while (curTransform != null)
                {
                    interactable = curTransform.GetComponent<IExplosionInteractable>();
                    if (interactable != null)
                        break;

                    curTransform = curTransform.parent;
                }
                
                if (interactable == null)
                    return range;

                // Interact
                isContinue = interactable.Interact();
                if (!isContinue)
                    return range;
            }

            offset += direction;
        }
        return maxRange;
    }

    /// <summary>
    /// 물풍선이 설치될 위치를 지정합니다.
    /// </summary>
    /// <param name="placerPosition">풍선을 설치하는 오브젝트의 position</param>
    /// <returns>해당 위치에 물풍선 설치 가능 여부</returns>
    public bool SetLocation(Vector3 placerPosition)
    {
        Vector3 location = new Vector3(Mathf.RoundToInt(placerPosition.x), 0, Mathf.RoundToInt(placerPosition.z));

        // Inspect validation of location
        Collider[] others = Physics.OverlapSphere(location, 0.3f, _waterBombLayerMask);
        if (others.Length > 0)
        {
            _objectPool.Release(this);
            return false;
        }

        // Move to location
        transform.position = location;

        // Set collision ignorance
        SetCollisionIgnorance();

        return true;
    }

    public bool Interact()
    {
        if (!_isExploded)
        {
            Explode();
        }
        return false;
    }
}
