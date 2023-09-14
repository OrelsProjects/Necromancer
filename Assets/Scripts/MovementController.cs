using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MovementController : MonoBehaviour
{

    [SerializeField]
    private AudioClip _stepSound;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteSheet;

    private AnimationHelper _animationHelper;

    private float _speed = 0;
    private Vector2 _direction = Vector2.zero;
    private float _previousSpeed = 0f;
    private Vector2 _previousDirection = Vector2.zero;
    private bool _forcedDisabled = false;

    public bool isEnabled;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteSheet = GetComponent<SpriteRenderer>();
        _animationHelper = new AnimationHelper(_animator);
    }

    public void FaceTarget(Transform target)
    {
        _direction = target.position;
        FlipCharacter(force: true);
    }

    public bool Move(float speed, Vector2 direction)
    {
        if (_forcedDisabled)
        {
            return false;
        }
        try
        {
            _speed = speed;
            _direction = direction;
            _rigidbody.velocity = _direction * _speed;
            _animationHelper.PlayAnimation(AnimationType.Running, _speed > 1);
            _animationHelper.PlayAnimation(AnimationType.Walking, _speed > 0 && _speed <= 1);
            FlipCharacter();
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Game object with name: " + gameObject.name + " has thrown an exception: " + ex.Message);
            return false;
        }
    }

    private void FlipCharacter(bool force = false)
    {
        if (_speed > 0 || force)
        {
            _spriteSheet.flipX = _direction.x < 0;
        }
    }

    public bool Move(float speed, GameObject target)
    {
        _direction = (target.transform.position - transform.position).normalized;
        _speed = speed;
        return Move(speed, _direction);
    }

    public void Stop()
    {
        Move(0, Vector2.zero);
        _animationHelper.PlayAnimation(AnimationType.Idle);
    }

    public void Enable()
    {
        if (enabled || _forcedDisabled)
        {
            return;
        }
        enabled = true;
        isEnabled = true;
    }

    /// <summary>
    ///  Disable movement of character
    /// </summary>
    /// <param name="forced">Disable forever. No way to return.</param>
    public void Disable(bool forced = false)
    {
        if (forced || _forcedDisabled)
        {
            Move(0, Vector2.zero);
            _forcedDisabled = true;
            return;
        }
        if (!enabled)
        {
            return;
        }
        Stop();
        enabled = false;
    }

    private void PlayStepSound()
    {
        // SoundsManager.Instance.PlayStepSound(_stepSound);
    }

    private void OnDisable()
    {
        _previousSpeed = _speed;
        _previousDirection = _direction;
        Move(0, Vector2.zero);
        isEnabled = false;
    }

    private void OnEnable()
    {
        if (_forcedDisabled)
        {
            return;
        }
        Move(_previousSpeed, _previousDirection);
        isEnabled = true;
    }

}