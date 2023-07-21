using UnityEngine;

public class CharacterManager : MonoBehaviour
{

    public static CharacterManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject CivillianWorkerPrefab;
    public GameObject DefenderMeleePrefab;
    public GameObject CivilianWorkerZombiePrefab;
    public GameObject DefenderMeleeZombiePrefab;
}