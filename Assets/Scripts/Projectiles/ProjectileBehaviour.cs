using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, IProjectile
{

    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private AudioClip _hitSound;

    private MovementController _movementController;

    private const float ProjectileLifetime = 3f;
    private float _damage;
    private bool _isUsed;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        Destroy(this, ProjectileLifetime);
    }

    private void FaceTarget(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, angle + 180f);
        transform.rotation = desiredRotation;
    }

    public void SetTarget(Transform target, float speed, float damage)
    {
        if (target == null)
        {
            Debug.LogWarning("Projectile target is null.");
            Destroy(gameObject);
            return;
        }
        FaceTarget(target);
        _damage = damage;
        _movementController.Move(speed, target.gameObject);
    }

    public void AddAttackCallback(Action onAttack)
    {
        onAttack();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isUsed)
        {
            return;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            if (collision.gameObject.TryGetComponent<Zombie>(out var zombie))
            {
                _isUsed = true;
                zombie.TakeDamage(_damage);
                AudioSource.PlayClipAtPoint(_hitSound, transform.position);
                Destroy(gameObject);
            }
        }
    }
}