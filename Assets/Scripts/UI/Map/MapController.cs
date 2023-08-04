using System.Collections.Generic;
using UnityEngine;

public enum Maps
{
    Area1
}

public class Map : MonoBehaviour
{

    public static Map Instance { get; private set; }

    private Dictionary<Maps, string> _maps = new Dictionary<Maps, string>()
    {
        { Maps.Area1, "Round Scene" }
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public void LoadArea1()
    {
        LoadMap(Maps.Area1);
    }

    public void LoadMap(Maps map)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_maps[map]);
    }
}