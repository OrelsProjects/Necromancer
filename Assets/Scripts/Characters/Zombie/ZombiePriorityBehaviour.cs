using System.Collections.Generic;
using UnityEngine;

public class ZombiePriorityBehaviour : MonoBehaviour
{

    [Header("Zombie Behaviours")]
    [SerializeField]
    private ZombieBehaviour _zombie;
    [SerializeField]
    private ZombieChaser _zombieChaser;

    private CircleCollider2D _collider;
    private Rigidbody2D _rigidBody;
    private List<GameObject> _defendersInSight = new();

    private Owner _owner;

    private ZombieLevel Data
    {
        get
        {
            return _zombie.Data;
        }
    }

    private float DetectionRange
    {
        get
        {
            return Data.DetectionRange;
        }
    }

    private void Awake()
    {
        if (gameObject.GetComponent<Rigidbody2D>())
        {
            _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        }
        else
        {
            _rigidBody = gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        }
        if (gameObject.GetComponent<CircleCollider2D>())
        {
            _collider = gameObject.GetComponent<CircleCollider2D>();
        }
        else
        {
            _collider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
        }
        _owner = new() { gameObject = gameObject, priority = OwnerPriority.Default };
    }

    private void Start()
    {
        InitRigidbody();
        InitCollider();
        _zombieChaser.SetOwner(_owner);
    }

    private void Update()
    {
        transform.position = _zombie.transform.position;
        if (_defendersInSight.Count == 0)
        {
            return;
        }
        UpdateTarget();
    }

    private void UpdateTarget()
    {
        
    }

    private GameObject FindClosestDefender()
    {
        GameObject closestDefender = null;
        List<GameObject> defendersToRemove = new();
        float closestDistance = Mathf.Infinity;
        foreach (GameObject defender in _defendersInSight)
        {
            if (defender == null)
            {
                defendersToRemove.Add(defender);
                continue;
            }
            float distance = Vector2.Distance(transform.position, defender.transform.position);
            if (distance < closestDistance)
            {
                closestDefender = defender;
                closestDistance = distance;
            }
        }
        foreach (GameObject defender in defendersToRemove)
        {
            _defendersInSight.Remove(defender);
        }
        return closestDefender;
    }

    private void InitRigidbody()
    {
        _rigidBody.gravityScale = 0;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void InitCollider()
    {
        //_collider.isTrigger = true;
        //_collider.radius = Mathf.Sqrt(DetectionRange / Mathf.PI) / 2;
        _collider.radius = 0; // TODO: Decide whether to keep it here or not.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombifiable"))
        {
            if (ShouldChangeTargets(collision.gameObject))
            {
                _zombieChaser.SetTarget(collision.gameObject.GetComponent<Zombifiable>(), _owner);
            }
        }
    }

    private bool IsDefender(GameObject gameObject) =>
         gameObject && gameObject.GetComponent<Defender>() != null;

    private bool IsClosestDefender(GameObject gameObject)
    {
        float currentTargetDistance = Vector3.Distance(transform.position, _zombieChaser.Target.transform.position);
        float closestTargetDistance = Vector3.Distance(transform.position, gameObject.transform.position);
        if (currentTargetDistance > closestTargetDistance)
        {
            return false;
        }
        foreach (var defender in _defendersInSight)
        {
            if (defender == null)
            {
                continue;
            }
            float distance = Vector3.Distance(transform.position, defender.transform.position);
            if (distance < closestTargetDistance)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// If the zombie is chasing a defender, it will continue to do so.
    /// </summary>
    /// <param name="gameObject"> Is the object that entered the detection range </param>
    /// <returns> True if the zombie is chasing a civilian, false otherwise </returns> 
    private bool ShouldChangeTargets(GameObject gameObject) =>
        _zombieChaser.Target == null
        || IsDefender(gameObject) && !IsClosestDefender(gameObject);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombifiable"))
        {
            if (IsDefender(collision.gameObject))
            {
                _defendersInSight.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombifiable"))
        {
            if (IsDefender(collision.gameObject))
            {
                _defendersInSight.Remove(collision.gameObject);
            }
        }
    }
}