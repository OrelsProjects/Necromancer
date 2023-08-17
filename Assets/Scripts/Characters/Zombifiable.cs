using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class Zombifiable : MonoBehaviour, IChaseable {

    [SerializeField]
    private ZombifiableData _data;

    private int _hitsToZombify;
    private float _movementBlockedTimeAfterAttack;
    private float _lastHitTime = 0f;
    private bool _isTurning = false;

    MovementController _movementController;
    Animator _animator;
    AnimationHelper _animationHelper;


    private void Awake() {
        gameObject.tag = "Zombifiable";
        _animator = GetComponent<Animator>();
        _movementController = GetComponent<MovementController>();
        _animationHelper = new AnimationHelper(_animator);
        _hitsToZombify = _data.HitsToZombify;
        _movementBlockedTimeAfterAttack = _data.MovementBlockedTimeAfterAttack;
    }

    public void Zombify(GameObject zombiePrefab, int zombifyDamage) {
        _hitsToZombify -= zombifyDamage;
        _animationHelper.PlayAnimation(AnimationType.Hit);
        if (_hitsToZombify > 0) {
            StartCoroutine(BlockMovement());
        } else {
            StartCoroutine(TurnToZombie(zombiePrefab));
        }
    }

    private IEnumerator TurnToZombie(GameObject zombiePrefab) {
        GameObject zombie;
        if (!_isTurning) {
            zombie = Instantiate(zombiePrefab, transform.position, Quaternion.identity);
            zombie.SetActive(false);
            _isTurning = true;
        } else {
            yield break;
        }
        _movementController.Disable(forced: true);
        _animationHelper.PlayAnimation(AnimationType.Death);
        AudioSource.PlayClipAtPoint(_data.DeathSound, transform.position);
        yield return new WaitForSeconds(1f);
        zombie.SetActive(true);
        Destroy(gameObject);
    }

    private IEnumerator BlockMovement() {
        _lastHitTime = Time.time;
        _animationHelper.PlayAnimation(AnimationType.Idle);
        _movementController.Disable();
        yield return new WaitForSeconds(_movementBlockedTimeAfterAttack);
        if (Time.time - _lastHitTime >= _movementBlockedTimeAfterAttack && !IsZombified()) {
            _movementController.Enable();
        }
    }

    public bool IsZombified() {
        return _hitsToZombify == 0 || _isTurning;
    }

    public bool IsAvailable() {
        return !IsZombified();
    }

    public Transform GetTransform() {
        return transform;
    }

    private void OnDestroy() {
        RoundManager.Instance.RemoveZombifiable(this);
    }
}