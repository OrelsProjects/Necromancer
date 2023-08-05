using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> _saveablesObjects;

    public List<ISaveable> _saveables;

    private Dictionary<string, string> _data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
    }

    void Start()
    {
        _saveablesObjects.ForEach(saveableObject =>
        {
            _saveables.AddRange(saveableObject.GetComponentsInChildren<ISaveable>());
        });
        
        string saveFileLocation = Application.persistentDataPath + "/savefile.dat";
        if (File.Exists(saveFileLocation))
        {
            StreamReader reader = new StreamReader(saveFileLocation);
            string saveData = reader.ReadToEnd();
            reader.Close();
            _data = JsonUtility.FromJson<Dictionary<string, string>>(saveData);
        }
        else
        {
            _data = new Dictionary<string, string>();
        }
        InitiateLoad();
    }

    private void InitiateLoad()
    {
        _saveables.ForEach(s =>
        {
            s.LoadData();
        });
    }

    public void InitiateSave()
    {
        _data = new Dictionary<string, string>();
        _saveables = new List<ISaveable>();

        _saveables.ForEach(s =>
        {
            Dictionary<string, string> data = s.GetData();
            foreach (KeyValuePair<string, string> d in data)
            {
                _data.Add(d.Key, d.Value);
            }
        });
        string saveData = JsonUtility.ToJson(_data);
        string saveFileLocation = Application.persistentDataPath + "/savefile.dat";
        StreamWriter writer = new StreamWriter(saveFileLocation, false);
        writer.Write(saveData);
        writer.Close();
    }

    public Dictionary<string, string> GetData(List<string> keys)
    {
        Dictionary<string, string> dataRequested = new();
        keys.ForEach(k =>
        {
            if (_data.ContainsKey(k))
            {
                dataRequested.Add(k, _data[k]);
            }
        });
        return dataRequested;
    }

    private void OnDestroy()
    {
        InitiateSave();
    }

}