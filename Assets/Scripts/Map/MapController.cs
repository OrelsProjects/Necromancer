using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [SerializeField]
    private List<AreaData> _areas = new();


    [SerializeField]
    private GameObject _area1;
    [SerializeField]
    private GameObject _area1Zombified;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void LoadArea1()
    {
        LoadArea(Areas.Area1);
    }

    public void LoadArea(Areas area)
    {
        AreaData areaData = _areas.Find(a => a.Area == area);
        if (areaData != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(areaData.AreaNameString);
        }
    }
}