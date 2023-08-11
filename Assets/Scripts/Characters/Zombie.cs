using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public enum ZombieState
{
    Idle,
    Chasing,
    AboutToAttack,
    Attacking,
    AboutToDie,
    Dead,
    RoundOver,
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Animator))]
public class Zombie : MonoBehaviour, IChaseable
{

    [SerializeField]
    private ZombieType _type;

    [Header("Sound")]
    [SerializeField]
    private AudioClip _spawnSound;
    [SerializeField]
    private List<AudioClip> _attackSounds;
    [SerializeField]
    private AudioClip _hitSound;
    [SerializeField]
    private AudioClip _deathSound;

    [Header("Playground")]
    [Tooltip("To have zombies that are not attacking the player, set this to true")]
    [SerializeField]
    private bool _playground = false;

    private ZombieLevel _data;

    private bool _isAttackSoundPlaying = false;

    private MovementController _movementController;
    private Animator _animator;
    private ZombieChaser _chaser;
    private WanderController _wanderController;
    private AnimationHelper _animationHelper;
    private ZombieState _state = ZombieState.Idle;

    private float _currentHealth;

    private void Awake()
    {
        tag = "Zombie";
        _movementController = GetComponent<MovementController>();
        _animator = GetComponent<Animator>();
        _chaser = GetComponent<ZombieChaser>();
        _wanderController = GetComponent<WanderController>();
        _animationHelper = new AnimationHelper(_animator);
    }

    private void Start()
    {
        RoundManager.Instance.AddZombie(this);
        AudioSource.PlayClipAtPoint(_spawnSound, transform.position);
        if (_playground)
        {
            return;
        }
        _data = CharactersManager.Instance.GetZombieData(_type);
        _currentHealth = _data.Health;

    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombie(this);
    }

    private void Update()
    {
        if (_playground)
        {
            return;
        }
        if (_chaser.Target == null && _state != ZombieState.RoundOver)
        {
            SetState(ZombieState.Idle);
            _animationHelper.PlayAnimation(AnimationType.Idle);
        }

        switch (_state)
        {
            case ZombieState.Idle:
                HandleIdleState();
                break;
            case ZombieState.Chasing:
                Chase();
                break;
            case ZombieState.AboutToAttack:
                StartCoroutine(ZombifyTarget());
                break;
            case ZombieState.AboutToDie:
                HandleDeath();
                break;
        }
    }

    private void HandleIdleState()
    {
        _movementController.Stop();
        _chaser.SetTarget();
        if (_chaser.Target != null)
        {
            SetState(ZombieState.Chasing);
        }
        else
        {
            FinishRound();
        }
    }

    private void Chase()
    {
        if (!_chaser.Target.IsAvailable())
        {
            SetState(ZombieState.Idle);
        }
        if (_chaser.IsTargetReached())
        {
            _movementController.Move(0, _chaser.Target.gameObject);
            SetState(ZombieState.AboutToAttack);
        }
        else
        {
            _movementController.Move(_data.Speed, _chaser.Target.gameObject);
            _animationHelper.PlayAnimation(AnimationType.Running);
        }
    }

    private void FinishRound()
    {
        _wanderController.Enable();
        SetState(ZombieState.RoundOver);
    }

    private IEnumerator ZombifyTarget()
    {
        if (!_chaser.Target.IsAvailable() || _chaser.Target.IsZombified())
        {
            SetState(ZombieState.Idle);
            yield break;
        }

        SetState(ZombieState.Attacking);
        _animationHelper.PlayAnimation(AnimationType.AttackMelee);
        _chaser.Target.Zombify(gameObject, _data.Damage);
        AudioSource.PlayClipAtPoint(_hitSound, transform.position);
        yield return new WaitForSeconds(1 / _data.AttackSpeed);

        if (_state == ZombieState.Attacking)
        {
            SetState(ZombieState.Chasing);
        }
    }

    /// <summary>
    /// Plays a random attack sound with 30% chance if a sound is not already playing.
    /// </summary>
    private void PlayRandomAttackSound()
    {
        bool shouldPlaySound = Random.Range(0, 100) < 30;
        if (!_isAttackSoundPlaying && shouldPlaySound && IsAlive())
        {
            _isAttackSoundPlaying = true;
            AudioSource.PlayClipAtPoint(_attackSounds[Random.Range(0, _attackSounds.Count)], transform.position);
            StartCoroutine(AttackSoundCooldown());
        }
    }

    private IEnumerator AttackSoundCooldown()
    {
        yield return new WaitForSeconds(1 / _data.AttackSpeed);
        _isAttackSoundPlaying = false;
    }

    private void HandleDeath()
    {
        if (_state == ZombieState.Dead)
        {
            return;
        }
        SetState(ZombieState.Dead);
        AudioSource.PlayClipAtPoint(_deathSound, transform.position);
        _movementController.Stop();
        _wanderController.Disable();
        _animationHelper.PlayAnimation(AnimationType.Death);
        Destroy(gameObject, 1f);
    }

    private void SetState(ZombieState state)
    {
        _state = state;
    }

    public void TakeDamage(float damage)
    {
        _animationHelper.PlayAnimation(AnimationType.Hit);
        _currentHealth -= damage;
        if (_currentHealth <= 0 && _state != ZombieState.AboutToDie)
        {
            SetState(ZombieState.AboutToDie);
        }
    }

    public void Heal(float health)
    {
        _animationHelper.PlayAnimation(AnimationType.Heal);
        _currentHealth += health;
    }

    public bool IsAlive() => _currentHealth > 0;

    public bool IsAvailable() => _currentHealth > 0;

    public Transform GetTransform() => transform;
}
