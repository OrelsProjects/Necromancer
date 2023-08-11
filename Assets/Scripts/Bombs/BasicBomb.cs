using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ZombieBomb : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10)]
    private float _range = 0.4f;

    [SerializeField]
    [Range(0, 0.1f)]
    private float _explodeSpeed = 0.05f;

    private CircleCollider2D _circleCollider;

    void Awake()
    {
        transform.localScale = Vector3.zero;
        _circleCollider = GetComponent<CircleCollider2D>();
        _circleCollider.radius = 1;
        ReleaseBomb();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Explode());
    }

    public void ReleaseBomb()
    {
        enabled = true;
        _circleCollider.radius = 1;
        gameObject.SetActive(true);
    }

    private IEnumerator Explode()
    {
        if (_circleCollider.radius >= _range)
        {
            Destroy(gameObject);
        }
        else
        {
            Vector3 currentScale = transform.localScale;
            transform.localScale = new(currentScale.x + _explodeSpeed, currentScale.y + _explodeSpeed, currentScale.z);
            _circleCollider.radius += (1 / 2 * Mathf.PI) + _explodeSpeed;
            yield return new WaitForSeconds(0.07F);
            StartCoroutine(Explode());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombifiable"))
        {
            Zombifiable zombifiable = collision.gameObject.GetComponent<Zombifiable>();
            //zombifiable.Zombify(3); TODO: Fix if needed
        }
    }
}