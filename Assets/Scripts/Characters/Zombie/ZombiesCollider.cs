using UnityEngine;
using System.Collections;

public class ZombiesCollider : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        GameObject zombiesColliderObject = new("Zombies Collider");
        Rigidbody2D rb = zombiesColliderObject.AddComponent<Rigidbody2D>();
        BoxCollider2D collider = zombiesColliderObject.AddComponent<BoxCollider2D>();

        zombiesColliderObject.transform.SetParent(transform);
        zombiesColliderObject.transform.localPosition = Vector3.zero;
        zombiesColliderObject.transform.localScale = Vector3.one;
        zombiesColliderObject.transform.localRotation = Quaternion.identity;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.mass = 999999;

        collider.isTrigger = false;
        collider.size = new(0.1f, 0.05f);
        collider.excludeLayers = LayerMask.GetMask("Zombifiable");
        collider.includeLayers = LayerMask.GetMask("Zombie");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.parent.position;
    }
}

