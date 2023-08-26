using System.Collections;
using UnityEngine;

public enum ZombifiableState
{
    Default,
    Turning,
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Zombifiable : MonoBehaviour, IChaseable
{

    [SerializeField]
    private ZombifiableData _data;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _hitScreamSound;
    [SerializeField]
    private AudioClip _deathSound;

    private int _hitsToZombify;
    private float _movementBlockedTimeAfterAttack;
    private float _lastHitTime = 0f;
    private float _timeToZombify = 1f;
    private float _chanceToScreamOnHit = 0.3f;

    private ZombifiableState _state = ZombifiableState.Default;

    MovementController _movementController;
    Animator _animator;
    AnimationHelper _animationHelper;


    private void Awake()
    {
        gameObject.tag = "Zombifiable";
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
        _hitsToZombify = _data.HitsToZombify;
        _movementBlockedTimeAfterAttack = _data.MovementBlockedTimeAfterAttack;
    }

    public void Zombify(GameObject zombiePrefab, int zombifyDamage)
    {
        _hitsToZombify -= zombifyDamage;
        PlayHitSound();
        _animationHelper.PlayAnimation(AnimationType.Hit);
        if (_hitsToZombify > 0)
        {
            StartCoroutine(BlockMovement());
        }
        else
        {
            StartCoroutine(TurnToZombie(zombiePrefab));
        }
    }

    private void PlayHitSound()
    {
        if (_hitScreamSound != null && Random.Range(0, 1) < _chanceToScreamOnHit)
        {
            SoundsManager.Instance.PlaySFX(_hitScreamSound);
        }
    }

    private IEnumerator TurnToZombie(GameObject zombiePrefab)
    {
        GameObject zombie;
        if (_state != ZombifiableState.Turning)
        {
            _state = ZombifiableState.Turning;
            if (Random.Range(0f, 1f) < _data.TurnChance)
            {
                zombie = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
                zombie.SetActive(false);
            }
        }
        else
        {
            yield break;
        }
        _movementController.Disable(forced: true);
        _animationHelper.PlayAnimation(AnimationType.Death);
        SoundsManager.Instance.PlaySFX(_deathSound);
        yield return new WaitForSeconds(_timeToZombify);
        Destroy(gameObject);
    }

    private IEnumerator BlockMovement()
    {
        _lastHitTime = Time.time;
        _animationHelper.PlayAnimation(AnimationType.Idle);
        _movementController.Disable();
        yield return new WaitForSeconds(_movementBlockedTimeAfterAttack);
        if (Time.time - _lastHitTime >= _movementBlockedTimeAfterAttack && !IsZombified())
        {
            _movementController.Enable();
        }
    }

    public bool IsZombified()
    {
        return _hitsToZombify == 0 || _state == ZombifiableState.Turning;
    }

    public bool IsAvailable()
    {
        return !IsZombified();
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombifiable(this);
    }
}