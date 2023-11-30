using System;
using UnityEngine;

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
        SetCollider();
    }

    private void SetCollider()
    {
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        int zombieLayerMask = 1 << LayerMask.NameToLayer("Zombie");
        int collidersLayerMask = 1 << LayerMask.NameToLayer("Colliders");
        collider.excludeLayers = ~zombieLayerMask & ~collidersLayerMask;
        collider.includeLayers = zombieLayerMask | collidersLayerMask;
        collider.contactCaptureLayers = zombieLayerMask;
        collider.callbackLayers = zombieLayerMask;
        collider.isTrigger = true;
        collider.size = new Vector2(0.14f, 0.05f);
    }

    private void FaceTarget(Transform target)
    {
        Vector3 targetDir = target.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.Euler(0, 0, angle);
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
        Debug.Log("Projectile hit: " + collider.gameObject.name);
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