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
    [SerializeField] private float _speed = 2.5f;
    [SerializeField] private float _attackSpeed = 1f;
    [SerializeField] private float _health = 100f;

    [Header("Sound")]
    [SerializeField]
    private List<AudioClip> _attackSounds;
    [SerializeField]
    private AudioClip _deathSound;

    private bool _isAttackSoundPlaying = false;

    private MovementController _movementController;
    private Animator _animator;
    private ZombieChaser _chaser;
    private WanderController _wanderController;
    private AnimationHelper _animationHelper;
    private ZombieState _state = ZombieState.Idle;

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
    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveZombie(this);
    }

    private void Update()
    {
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
        if (_chaser.IsTargetReached())
        {
            _movementController.Move(0, _chaser.Target.gameObject);
            SetState(ZombieState.AboutToAttack);
        }
        else
        {
            _movementController.Move(_speed, _chaser.Target.gameObject);
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
        _chaser.Target.Zombify();

        yield return new WaitForSeconds(1 / _attackSpeed);

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
        yield return new WaitForSeconds(1 / _attackSpeed);
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
        _health -= damage;
        if (_health <= 0 && _state != ZombieState.AboutToDie)
        {
            SetState(ZombieState.AboutToDie);
        }
    }

    public void Heal(float health)
    {
        _animationHelper.PlayAnimation(AnimationType.Heal);
        _health += health;
    }

    public bool IsAlive() => _health > 0;

    public bool IsAvailable() => _health > 0;

    public Transform GetTransform() => transform;
}
