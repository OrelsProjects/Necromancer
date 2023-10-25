using System.Collections;
using UnityEngine;

public enum ZombifiableState
{
    Default,
    Turning,
}

[RequireComponent(typeof(Animator))]
public class Zombifiable : MonoBehaviour, IChaseable
{

    [SerializeField]
    private ZombifiableData _data;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip _hitScreamSound;
    [SerializeField]
    private AudioClip _deathSound;

    private int _maxHealth;
    public int _currentHealth;
    private float _movementBlockedTimeAfterAttack;
    private float _lastHitTime = 0f;
    private readonly float _timeToZombify = 1f;
    private readonly float _chanceToScreamOnHit = 0.3f;

    public ZombifiableState _state = ZombifiableState.Default;

    MovementController _movementController;
    Animator _animator;
    AnimationHelper _animationHelper;


    private void Awake()
    {
        gameObject.tag = "Zombifiable";
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
        _maxHealth = _data.Health;
        _currentHealth = _maxHealth;
        _movementBlockedTimeAfterAttack = _data.MovementBlockedTimeAfterAttack;
    }

    public void Zombify(ZombieType zombieType, int damage)
    {
        if (_state == ZombifiableState.Turning)
        {
            return;
        }
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        PlayHitSound();
        _animationHelper.PlayAnimation(AnimationType.Hit);
        if (_currentHealth > 0)
        {
            StartCoroutine(BlockMovement());
        }
        else
        {
            var zombiePrefab = CharactersManager.Instance.GetZombiePrefab(zombieType);
            _state = ZombifiableState.Turning;
            StartCoroutine(TurnToZombie(zombiePrefab.gameObject));
        }
    }

    private void PlayHitSound()
    {
        if (_hitScreamSound != null && Random.Range(0, 1) < _chanceToScreamOnHit)
        {
            AudioSource.PlayClipAtPoint(_hitScreamSound, transform.position);
        }
    }

    private IEnumerator TurnToZombie(GameObject zombiePrefab)
    {
        GameObject zombie = null;
        _movementController.Disable(true);
        _state = ZombifiableState.Turning;
        if (Random.Range(0f, 1f) < _data.ChanceToBecomeZombie)
        {
            zombie = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            zombie.SetActive(false);
        }

        _movementController.Disable(forced: true);
        _animationHelper.PlayAnimation(AnimationType.Death);
        AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        yield return new WaitForSeconds(_timeToZombify);
        if (zombie != null)
        {
            zombie.SetActive(true);
        }
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
        return _currentHealth <= 0 || _state == ZombifiableState.Turning;
    }

    #region IChaseable

    public bool IsAvailable()
    {
        return !IsZombified();
    }

    public bool IsPriority() => GetComponent<Defender>() != null;

    #endregion

    private void OnDestroy() => RoundManager.Instance.RemoveZombifiable(this);

}