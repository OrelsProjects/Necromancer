using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZombieSpawnBehaviour : MonoBehaviour
{
    public ZombieType? _type;
    private bool _isSpawned = false;
    [SerializeField]
    private Image _selected;

    public Image ZombieImage;

    public bool IsAvailable => !_isSpawned;

    public void Start()
    {
        if (_type == null)
        {
            throw new("Did you call SetZombieType?");
        }
        var zombieImageData = CharactersManager.Instance.GetZombieSprite(_type.Value).Value;
        ZombieImage.sprite = zombieImageData.sprite;
        ZombieImage.transform.localScale = new Vector3(zombieImageData.xDim, zombieImageData.yDim, 1);
    }

    public void SetZombieType(ZombieType type)
    {
        _type = type;
    }

    public void SpawnZombies(Vector2 position)
    {
        if (!IsAvailable) { return; }

        Vector3 circleCenter = position;

        int amountToSpawn = CharactersManager.Instance.GetZombieLevelData(_type.Value).AmountSpawned;
        float angleIncrement = 360.0f / amountToSpawn;
        float radius = 0.5f;

        for (int i = 0; i < amountToSpawn; i++)
        {
            float angle = i * angleIncrement;

            Vector3 spawnPosition = circleCenter + new Vector3(radius * Mathf.Cos(angle * Mathf.Deg2Rad), radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            Instantiate(CharactersManager.Instance.GetZombiePrefab(_type.Value).gameObject, spawnPosition, Quaternion.identity);
        }
        _isSpawned = true;
        StartCoroutine(WaitForZombiesSpawn()); // Allow zombies to spawn before checking if there are any zombies left to spawn.
    }

    public void Select(bool isSelected)
    {
        _selected.gameObject.SetActive(isSelected);
        _selected.color = new Color(1, 1, 1, 0);
        if (isSelected)
        {
            StartCoroutine(ChangeImageOpacity());
        }
    }

    private IEnumerator ChangeImageOpacity()
    {
        float targetOpacity = 1;
        float currentOpacity = 0;
        float timeElapsed = 0;
        float duration = 0.3f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float newOpacity = Mathf.Lerp(currentOpacity, targetOpacity, timeElapsed / duration);
            _selected.color = new Color(1, 1, 1, newOpacity);
            yield return null;
        }
    }

    private IEnumerator WaitForZombiesSpawn()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(this);
    }
}
