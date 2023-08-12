using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour, IProjectile
{
    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private AudioClip _hitSound;

    private MovementController _movementController;

    private float _damage;

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
    }

    public void SetTarget(Transform target, float speed, float damage, float timeToDestroy = 3f)
    {
        if (target == null)
        {
            return; // Target might be dead by the time the projectile hits
        }
        _damage = damage;
        _movementController.Move(speed, target.gameObject);
        Destroy(this, timeToDestroy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();
            zombie.TakeDamage(_damage);
            AudioSource.PlayClipAtPoint(_hitSound, transform.position);
            Destroy(gameObject);
        }
    }
}