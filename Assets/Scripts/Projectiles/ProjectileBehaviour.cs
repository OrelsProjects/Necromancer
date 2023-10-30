using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, IProjectile
{

    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private AudioClip _hitSound; // TODO: Make it according to the layer it hits

    private const float ProjectileLifetime = 3f;
    private float _damage;
    private bool _isUsed;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = false;
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
        Vector2 direction = target.transform.position - transform.position;
        Vector2 directionNormalized = direction.normalized;
        GetComponent<Rigidbody2D>().velocity = directionNormalized * speed;
        Destroy(gameObject, ProjectileLifetime);
    }

    public void PlayHitSound()
    {
        // 25% Chance to play the hit sound
        if (UnityEngine.Random.Range(0, 4) == 0)
        {
            AudioSource.PlayClipAtPoint(_hitSound, transform.position);
        }
    }

    public void AddAttackCallback(Action onAttack)
    {
        onAttack();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (_isUsed)
        {
            return;
        }
        if (collider.gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            if (collider.gameObject.TryGetComponent<ZombieBehaviour>(out var zombie))
            {
                _isUsed = true;
                zombie.TakeDamage(_damage);
                PlayHitSound();
                Destroy(gameObject);
            }
        }
    }
}