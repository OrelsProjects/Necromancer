using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, IProjectile
{
    [SerializeField]
    private Sprite _sprite;

    private MovementController _movementController;

    private Transform _target;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
    }

    public void SetTarget(Transform target, float speed)
    {
        _target = target;
        _movementController.Move(speed, _target.gameObject);
        Destroy(this, 3f);
    }
}