using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class MovementController : MonoBehaviour
{

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private SpriteRenderer _spriteSheet;

    private AnimationHelper _animatorHelper;

    private float _speed = 0;
    private Vector2 _direction = Vector2.zero;
    private float _previousSpeed = 0f;
    private Vector2 _previousDirection = Vector2.zero;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteSheet = GetComponent<SpriteRenderer>();
        _animatorHelper = new AnimationHelper(_animator);
    }

    public bool Move(float speed, Vector2 direction)
    {
        try
        {
            _speed = speed;
            _direction = direction;
            _rigidbody.velocity = _direction * _speed;
            _animatorHelper.Running(_speed > 1);
            _animatorHelper.Walking(_speed > 0 && _speed <= 1);
            if (_speed > 0)
            {
                _spriteSheet.flipX = _direction.x < 0;
            }
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Game object with name: " + gameObject.name + " has thrown an exception: " + ex.Message);
            return false;
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
    }

    public void Enable()
    {
        if (enabled)
        {
            return;
        }
        Debug.Log("Enabled, speed and direction: " + _speed + " " + _direction);
        enabled = true;
    }

    public void Disable()
    {
        if (!enabled)
        {
            return;
        }
        Debug.Log("Disabled, speed and direction: " + _speed + " " + _direction);
        enabled = false;
    }

    private void OnDisable()
    {
        _previousSpeed = _speed;
        _previousDirection = _direction;
        Move(0, Vector2.zero);
    }

    private void OnEnable()
    {
        Move(_previousSpeed, _previousDirection);
    }

}