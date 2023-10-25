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
    Wandering,
}

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

    public ZombieLevel Data
    {
        get { return CharactersManager.Instance.GetZombieData(_type); }
    }

    private bool _isAttackSoundPlaying = false;

    private MovementController _movementController;
    private Animator _animator;
    private ZombieChaser _chaser;
    private WanderController _wanderController;
    private AnimationHelper _animationHelper;
    public ZombieState _state = ZombieState.Idle;
    public Zombifiable _target;
    public string targetName;

    // Used to check if the target is dead after the attack animaion is played
    // So that the defender won't attack a target that is not reached
    private bool _isTargetAttackedDead;
    private float _currentHealth;

    private void Awake()
    {
        tag = "Zombie";
        _movementController = GetComponent<MovementController>();
        _animator = GetComponent<Animator>();
        _chaser = GetComponent<ZombieChaser>();
        _wanderController = GetComponent<WanderController>();
        _animationHelper = new AnimationHelper(_animator, true);
        SetCollider();
    }

    private void Start()
    {
        // Relying on the fact that the zombie will be spawned on command and not instantly. (It's in Awake instead of Start)
        _animationHelper.SetAttackSpeed(Data.AttackSpeed);
        _currentHealth = Data.Health;

        _chaser.SubscribeToTargetChanges(OnTargetChange);
        RoundManager.Instance.AddZombie(this);
        AudioSource.PlayClipAtPoint(_spawnSound, transform.position);
        if (_playground)
        {
            return;
        }
    }

    private void OnDestroy()
    {
        _chaser.UnsubscribeFromTargetChanges(OnTargetChange);
        RoundManager.Instance.RemoveZombie(this);
    }

    private void Update()
    {
        if (_playground)
        {
            return;
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
            case ZombieState.Attacking:
                if (_target == null)
                {
                    _isTargetAttackedDead = true;
                    SetState(ZombieState.Idle);
                }
                break;
            case ZombieState.Wandering:
                _movementController.Enable();
                _wanderController.Enable();
                break;

        }
    }

    /// <summary>
    /// Creates a collider between the zombies so that they won't overlap.
    /// The collider's parent is this zombie.
    /// </summary>
    private void SetCollider()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        //collider.includeLayers = LayerMask.GetMask("Zombie");
        collider.isTrigger = true;
        collider.size = new Vector2(0.1f, 0.05f);
    }

    private void HandleIdleState()
    {
        if (_target == null || !_target.IsAvailable())
        {
            SetState(ZombieState.Wandering);
            _chaser.FindNewTarget();
        }
    }

    private void Chase()
    {
        if (_target == null || !_target.IsAvailable())
        {
            SetState(ZombieState.Idle);
            return;
        }
        if (_chaser.GetTargetDistanceState() == TargetDistanceState.Reached)
        {
            _movementController.Move(0, _target.gameObject);
            SetState(ZombieState.AboutToAttack);
        }
        else
        {
            _movementController.Move(Data.Speed, _target.gameObject);
            _animationHelper.PlayAnimation(AnimationType.Running);
        }
    }

    private void OnTargetChange(Zombifiable target)
    {
        _target = target;
        if (_target == null)
        {
            SetState(ZombieState.Wandering);
            return;
        }
        SetState(ZombieState.Chasing);
    }

    private IEnumerator ZombifyTarget()
    {
        if (!_target.IsAvailable() || _target.IsZombified())
        {
            SetState(ZombieState.Idle);
            yield break;
        }

        SetState(ZombieState.Attacking);
        _animationHelper.PlayAnimation(AnimationType.AttackZombie);
        AudioSource.PlayClipAtPoint(_hitSound, transform.position);
        yield return new WaitForSeconds(1 / Data.AttackSpeed);

        if (_state == ZombieState.Attacking)
        {
            SetState(ZombieState.Chasing);
        }
    }

    // For the animator to use
    public void InitiateAttack()
    {
        if (_target)
        {
            Vector2 targetPosition = _target.transform.position;
            // If the distance between the target and the zombie is too big, debuglog it
            if (Vector2.Distance(transform.position, targetPosition) > 1.5f)
            {
                Debug.LogWarning("Zombie is too far from the target");
            }
        }
        if (_state != ZombieState.Attacking)
        {
            _isTargetAttackedDead = false;
            return;
        }
        if (_target.IsAvailable())
        {
            _target.Zombify(_type, Data.Damage);
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
        yield return new WaitForSeconds(1 / Data.AttackSpeed);
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
        _movementController.Disable();
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
        if (_currentHealth <= 0)
        {
            return;
        }
        _animationHelper.PlayAnimation(AnimationType.Hit);
        _currentHealth -= damage;
        if (_currentHealth <= 0 && _state != ZombieState.AboutToDie)
        {
            SetState(ZombieState.AboutToDie);
        }
    }

    public bool IsAlive() => _currentHealth > 0;

    public bool IsAvailable() => _currentHealth > 0;

    public bool IsPriority() => true;
}
