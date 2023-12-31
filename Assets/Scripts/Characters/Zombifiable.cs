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
    private readonly float _timeToZombify = 1f;
    private readonly float _chanceToScreamOnHit = 0.3f;

    public ZombifiableState _state = ZombifiableState.Default;

    MovementController _movementController;
    Animator _animator;
    AnimationHelper _animationHelper;


    private void Start()
    {
        gameObject.tag = "Zombifiable";
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
        if (IsDefender())
        {
            SetDefenderData();
        }
        _maxHealth = _data.Health;
        _movementBlockedTimeAfterAttack = _data.MovementBlockedTimeAfterAttack;
        _currentHealth = _maxHealth;
    }

    private bool IsDefender() => GetComponent<Defender>() != null;
    private void SetDefenderData()
    {
        var defender = GetComponent<Defender>();
        _data.Health = defender.Data.Health;
        _data.ChanceToBecomeZombie = 1;
        _data.MovementBlockedTimeAfterAttack = 0;
    }

    /// <summary>
    /// Deals damage to the zombifiable and turns it into a zombie if it's health is 0 or less.
    /// </summary>
    /// <param name="zombieType">Is used to determine which zombie prefab to instantiate</param> // Maybe in the future
    /// <param name="damage">The amount of damage to deal</param>
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
            var zombiePrefab = CharactersManager.Instance.GetZombiePrefab(zombieType);// Maybe in the future
            // var zombiePrefab = CharactersManager.Instance.GetZombiePrefab(ZombieType.Small);
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
        if (_movementBlockedTimeAfterAttack <= 0)
        {
            yield break;
        }
        float _lastHitTime = Time.time;
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