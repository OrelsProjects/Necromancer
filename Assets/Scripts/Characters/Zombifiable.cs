using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Zombifiable : MonoBehaviour, IChaseable
{
    [SerializeField]
    private int _hitsToZombify = 3;
    [SerializeField]
    private float _movementBlockedTimeAfterAttack = 1.2f;
    [SerializeField]
    private Zombie _zombiePrefab;

    private float _lastHitTime = 0f;

    MovementController _movementController;
    Animator _animator;
    AnimationHelper _animationHelper;


    private void Awake()
    {
        gameObject.tag = "Zombifiable";

    }

    private void Start()
    {
        RoundManager.Instance.AddZombifiable(this);
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
    }

    public void Zombify(int zombifyDamage = 1)
    {
        _hitsToZombify -= zombifyDamage;
        _animationHelper.PlayAnimation(AnimationType.Hit);
        if (_hitsToZombify > 0)
        {
            StartCoroutine(BlockMovement());
        }
        else
        {
            StartCoroutine(TurnToZombie());
        }
    }

    private IEnumerator TurnToZombie()
    {
        _movementController.Disable();
        _animationHelper.PlayAnimation(AnimationType.Death);
        yield return new WaitForSeconds(1f);
        Instantiate(_zombiePrefab, transform.position, Quaternion.identity);
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
        return _hitsToZombify == 0;
    }

    public bool IsAvailable()
    {
        return !IsZombified();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombifiable(this);
    }
}