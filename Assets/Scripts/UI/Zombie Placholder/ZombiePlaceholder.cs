using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZombiePlaceholder : MonoBehaviour, IEndDragHandler, IDragHandler {
    [SerializeField]
    private ZombieType _type;
    [SerializeField]
    private SpriteRenderer _zombieSprite;

    [SerializeField]
    private TextMeshProUGUI _amountText;

    private bool _isSpawned = false;
    private bool _isSpawning = false;
    private Vector2 _initialSpritePosition;
    private Vector2 _currentTempSpritePosition;
    private SpriteRenderer _tempZombieSprite;

    public bool IsAvailable => !_isSpawned;

    void Start() {
        _amountText.text = CharactersManager.Instance.GetZombieData(_type).AmountSpawned.ToString();
        _zombieSprite.sprite = CharactersManager.Instance.GetZombieSprite(_type);
        _initialSpritePosition = _zombieSprite.transform.position;
    }

    public void Init() {
        _isSpawned = false;
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
            _tempZombieSprite.transform.localScale = new Vector3(5, 5, 5);
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
            _isSpawning = true;
            Vector2 spawnPosition = _tempZombieSprite.transform.position;
            Destroy(_tempZombieSprite.gameObject);
            _tempZombieSprite = null;
            int amountToSpawn = CharactersManager.Instance.GetZombieData(_type).AmountSpawned;
            for (int i = 0; i < amountToSpawn; i++) {
                Instantiate(CharactersManager.Instance.GetZombiePrefab(_type).gameObject, spawnPosition, Quaternion.identity);
            }

            _amountText.text = "0";
            StartCoroutine(WaitForZombiesSpawn()); // Allow zombies to spawn before checking if there are any zombies left to spawn.
        } else {
            _zombieSprite.color = new Color(1, 1, 1, 1);
        }
    }

    private IEnumerator WaitForZombiesSpawn() {
        yield return new WaitForSeconds(0.3f);
        _isSpawned = true;
        _isSpawning = false;
    }

    private bool IsGround() {
        Vector2 mousePosition = GetMousePosition();
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                return true;
            }
        }
        return false;
    }

    private Vector3 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
    }
}
