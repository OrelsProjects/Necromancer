using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;

public class SaveManager : MonoBehaviour {
    public static SaveManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> _saveablesObjects;

    public List<ISaveable> _saveables;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            InitSaveables();
            return;
        }
    }

    void Start() {
        StartCoroutine(InitiateLoadCore(0f));
    }

    private void InitSaveables() {
        _saveables = new List<ISaveable>();
        _saveablesObjects.ForEach(saveableObject => {
            if (saveableObject.TryGetComponent<ISaveable>(out var saveable)) {
                _saveables.Add(saveable);
            }
        });
    }

    private string BuildSaveFileName(ISaveableObject saveableObject) {
        return Application.persistentDataPath + "/" + saveableObject.ToString() + ".dat";
    }
    public void InitiateLoad() {
        StartCoroutine(InitiateLoadCore());
    }

    public void InitiateSave() {
        StartCoroutine(InitiateSaveCore());
    }

    public void SaveItem(ISaveableObject item) {
        var saveData = new Dictionary<string, string>
    {
        { "ObjectType", item.GetObjectType() },
        { "Data", JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })
        }
    };
        string stringSaveData = JsonConvert.SerializeObject(saveData);
        File.WriteAllText(BuildSaveFileName(item), stringSaveData);
    }

    private IEnumerator InitiateSaveCore() {
        yield return new WaitForSeconds(0.5f); // Let other processes run before saving
        _saveables?.ForEach(s => {
            ISaveableObject data = s.GetData();
            SaveItem(data);
        });
    }

    /// <summary>
    /// Reads the save file and loads the data into the game. If the file does not exist, 
    /// the load function will be called with the data it got from s.GetData();
    /// </summary>
    /// <param name="delay">How long to wait before initiating the load</param>
    /// <returns></returns>
    private IEnumerator InitiateLoadCore(float delay = 0.5f) {
        yield return new WaitForSeconds(delay); // Let other processes run before loading
        _saveables?.ForEach(s => {
            ISaveableObject data = s.GetData();
            string saveFileLocation = BuildSaveFileName(data);
            if (File.Exists(saveFileLocation)) {
                string stringData = File.ReadAllText(saveFileLocation);
                var saveData = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringData);
                if (saveData.ContainsKey("ObjectType")) {
                    var type = Type.GetType(saveData["ObjectType"]);
                    data = (ISaveableObject)JsonConvert.DeserializeObject(saveData["Data"], type, new JsonSerializerSettings {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                }
            }
            s.LoadData(data);
        });
    }
}
