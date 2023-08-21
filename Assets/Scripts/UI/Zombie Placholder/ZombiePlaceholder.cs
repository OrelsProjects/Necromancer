using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZombiePlaceholder : MonoBehaviour, IEndDragHandler, IDragHandler {

    [SerializeField]
    private SpriteRenderer _zombieSprite;
    [SerializeField]
    private TextMeshProUGUI _amountText;

    private bool _isSpawned = false;
    private bool _isSpawning = false;

    private ZombieType _type;
    private SpriteRenderer _tempZombieSprite;

    private Vector2 _initialSpritePosition;
    private Vector2 _currentTempSpritePosition;

    public bool IsAvailable => !_isSpawned;


    public void SetZombie(ZombieType type) {
        _type = type;
        _amountText.text = CharactersManager.Instance.GetZombieData(_type).AmountSpawned.ToString();
        _zombieSprite.sprite = CharactersManager.Instance.GetZombieSprite(_type);
        _initialSpritePosition = _zombieSprite.transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        if (_isSpawned || _isSpawning) {
            return;
        }

        if (_tempZombieSprite == null) {
            _tempZombieSprite = Instantiate(_zombieSprite, _initialSpritePosition, Quaternion.identity);
            _zombieSprite.color = new Color(1, 1, 1, 0.3f);
            _tempZombieSprite.sortingLayerName = "UI";
            _tempZombieSprite.sortingOrder = 2;
            _tempZombieSprite.transform.localScale = new Vector3(8, 8, 8);
        }

        _currentTempSpritePosition = GetMousePosition();
        _tempZombieSprite.transform.position = _currentTempSpritePosition;

        if (IsGround()) {
            _tempZombieSprite.color = new Color(1, 1, 1, 1);
        } else {
            _tempZombieSprite.color = new Color(1, 1, 1, 0.3f);
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (_isSpawned || _tempZombieSprite == null || _isSpawning) {
            return;
        }

        if (IsGround()) {
            SpawnZombies();
        } else {
            _zombieSprite.color = new Color(1, 1, 1, 1);
        }
    }

    private void SpawnZombies() {
        _isSpawning = true;
        Vector3 circleCenter = _tempZombieSprite.transform.position;

        Destroy(_tempZombieSprite.gameObject);

        int amountToSpawn = CharactersManager.Instance.GetZombieData(_type).AmountSpawned;
        float angleIncrement = 360.0f / amountToSpawn;
        float radius = 0.5f;

        for (int i = 0; i < amountToSpawn; i++) {
            float angle = i * angleIncrement;

            Vector3 spawnPosition = circleCenter + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            Instantiate(CharactersManager.Instance.GetZombiePrefab(_type).gameObject, spawnPosition, Quaternion.identity);
        }

        StartCoroutine(WaitForZombiesSpawn()); // Allow zombies to spawn before checking if there are any zombies left to spawn.
    }

    private IEnumerator WaitForZombiesSpawn() {
        yield return new WaitForSeconds(0.3f);
        _isSpawned = true;
        _isSpawning = false;
    }

    private bool IsGround() {
        Vector2 mousePosition = GetMousePosition();
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

        if (hit.collider != null) {
            int layer = hit.collider.gameObject.layer;
            if (layer == LayerMask.NameToLayer("Ground") || layer == LayerMask.NameToLayer("Zombie")) {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    }
}
