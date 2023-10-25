using System.Collections;
using UnityEngine;

public enum WanderState
{
    MovingToTarget,
    ReachedTarget,
    Waiting,
}

public class WanderController : MonoBehaviour
{

    public Vector2 nextPosition;

    [SerializeField]
    private float _speed = 0.8f;
    [SerializeField]
    private float _range = 0.5f;
    [SerializeField]
    private float _maxDistanceY = 4f;
    [SerializeField]
    private float _maxDistanceX = 10f;
    [SerializeField]
    private MovementController _movementController;

    private Vector3 _target;
    private WanderState _state;

    void Start()
    {
        SetNewDestination();
        _state = WanderState.MovingToTarget;
    }

    void Update()
    {
        switch (_state)
        {
            case WanderState.MovingToTarget:
                float distance = Vector2.Distance(transform.position, _target);
                if (distance < _range)
                {
                    _state = WanderState.ReachedTarget;
                }
                break;
            case WanderState.ReachedTarget:
                _state = WanderState.Waiting;
                StartCoroutine(WaitRandomTime());
                break;
            case WanderState.Waiting:
                break;
        }
    }

    private IEnumerator WaitRandomTime()
    {
        float randomDelay = Random.Range(1f, 5f);
        _movementController.Stop();
        yield return new WaitForSeconds(randomDelay);
        SetNewDestination();
        _state = WanderState.MovingToTarget;
    }

    private void SetNewDestination()
    {
        _target = new Vector2(Random.Range(-_maxDistanceX, _maxDistanceX), Random.Range(-_maxDistanceY, _maxDistanceY));
        Vector2 direction = _target - transform.position;
        Vector2 directionNormalized = direction.normalized;
        _movementController.Move(_speed, directionNormalized);
        nextPosition = _target;
    }

    public void Enable()
    {
        if (enabled)
        {
            return;
        }
        enabled = true;
        SetNewDestination();
        _state = WanderState.MovingToTarget;
    }

    public void Disable() => enabled = false;
}
