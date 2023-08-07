using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> _saveablesObjects;

    public List<ISaveable> _saveables;

    private List<ISaveableObject> _data;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitSaveables();
            DontDestroyOnLoad(gameObject);
            return;
        }
    }

    void Start()
    {
        InitiateLoad();
    }

    private void InitSaveables()
    {
        _saveables = new List<ISaveable>();
        _saveablesObjects.ForEach(saveableObject =>
        {
            ISaveable saveable = saveableObject.GetComponent<ISaveable>();
            if (saveable != null)
            {
                _saveables.Add(saveable);
            }
        });
    }

    public void InitiateLoad()
    {
        string saveFileLocation = Application.persistentDataPath + "/savefile.dat";
        File.Delete(saveFileLocation);
        if (File.Exists(saveFileLocation))
        {
            string saveData = File.ReadAllText(saveFileLocation);
            _data = JsonConvert.DeserializeObject<List<ISaveableObject>>(saveData, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
        else
        {
            _data = new List<ISaveableObject>();
        }

        _saveables.ForEach(s =>
        {
            s.LoadData();
        });
        UIController.Instance.UpdateUI();
    }


    public void InitiateSave()
    {
        _data = new List<ISaveableObject>();
        _saveables.ForEach(s =>
        {
            ISaveableObject data = s.GetData();
            _data.Add(data);
        });

        string saveData = JsonConvert.SerializeObject(_data, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        string saveFileLocation = Application.persistentDataPath + "/savefile.dat";
        File.WriteAllText(saveFileLocation, saveData);
    }

    public T GetData<T>()
    {
        foreach (var item in _data)
        {
            if (item is T tItem)
            {
                return tItem;
            }
        }

        return default;
    }

    private void OnDestroy()
    {
        InitiateSave();
    }
}
