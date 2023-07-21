using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DefenderType
{
    Melee,
    Archer,
    Mage,
    Shooter
}

public class Defender : MonoBehaviour
{

    [SerializeField]
    private DefenderType _defenderType;

    // Start is called before the first frame update
    void Start()
    {
        RoundManager.Instance.AddDefender(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        RoundManager.Instance.RemoveDefender(this);
    }
}
