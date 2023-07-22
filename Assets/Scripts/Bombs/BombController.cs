using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField]
    private ZombieBomb _zombieBombPrefab;
    [SerializeField]
    private AudioClip _releaseSound;

    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height)
        {
            Instantiate(_zombieBombPrefab, transform.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(_releaseSound, transform.position);
            Destroy(gameObject);
        }
        DisplayBombOnPosition();
    }

    private void DisplayBombOnPosition()
    {
        // Get the current mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;

        // Convert the screen coordinates to world coordinates
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f; // Make sure the object stays on the same Z-axis

        // Update the position of the object
        transform.position = worldPosition;
    }
}
