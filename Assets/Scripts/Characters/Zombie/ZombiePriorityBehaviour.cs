using UnityEngine;

public class ZombiePriorityBehaviour : MonoBehaviour {

    [Header("Zombie Behaviours")]
    [SerializeField]
    private Zombie _zombie;
    [SerializeField]
    private ZombieChaser _zombieChaser;

    private CircleCollider2D _collider;
    private Rigidbody2D _rigidBody;

    private ZombieLevel data {
        get {
            return _zombie.Data;
        }
    }

    private float _detectionRange {
        get {
            return data.DetectionRange;
        }
    }

    private void Awake() {
        if (gameObject.GetComponent<Rigidbody2D>()) {
            _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        } else {
            _rigidBody = gameObject.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
        }
        if (gameObject.GetComponent<CircleCollider2D>()) {
            _collider = gameObject.GetComponent<CircleCollider2D>();
        } else {
            _collider = gameObject.AddComponent(typeof(CircleCollider2D)) as CircleCollider2D;
        }
    }

    private void Start() {
        InitRigidbody();
        InitCollider();
    }

    private void Update() {
        transform.position = _zombie.transform.position;
    }

    private void InitRigidbody() {
        _rigidBody.gravityScale = 0;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void InitCollider() {
        _collider.isTrigger = true;
        _collider.radius = Mathf.Sqrt(_detectionRange / Mathf.PI) / 2;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Zombifiable")) {
            if (ShouldChangeTargets(collision.gameObject)) {
                _zombieChaser.SetTarget(collision.gameObject.GetComponent<Zombifiable>());
            }
        }
    }

    private bool IsDefender(GameObject gameObject) =>
         gameObject && gameObject.GetComponent<Defender>() != null;

    /// <summary>
    /// If the zombie is chasing a defender, it will continue to do so.
    /// </summary>
    /// <param name="gameObject"> Is the object that entered the detection range </param>
    /// <returns> True if the zombie is chasing a civilian, false otherwise </returns> 
    private bool ShouldChangeTargets(GameObject gameObject) =>
        _zombieChaser.Target == null
        || IsDefender(gameObject) && !IsDefender(_zombieChaser.Target.gameObject);

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Zombifiable")) {
            if (ShouldChangeTargets(collision.gameObject)) {
                _zombieChaser.SetTarget(collision.gameObject.GetComponent<Zombifiable>());
            }
        }
    }
}