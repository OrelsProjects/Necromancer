using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEditor;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public List<ISaveable> _saveables;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitSaveables();
            return;
        }
    }

    void Start()
    {
        InitiateLoad();
    }

    private void InitSaveables()
    {
        _saveables = FindAllISaveables();
    }

    private List<ISaveable> FindAllISaveables()
    {
        List<ISaveable> saveables = new();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.TryGetComponent<ISaveable>(out var saveable))
            {
                saveables.Add(saveable);
            }
        }
        return saveables;
    }

    private string BuildSaveFileName(ISaveableObject saveableObject)
    {
        return Application.persistentDataPath + "/" + saveableObject.ToString() + ".dat";
    }

    private void InitiateLoad()
    {
        StartCoroutine(InitiateLoadCore());
    }

    public void InitiateSave(bool closeAppAfterSave = false)
    {
        StartCoroutine(InitiateSaveCore(closeAppAfterSave));
    }

    public void SaveItem(ISaveableObject item)
    {
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

    private IEnumerator InitiateSaveCore(bool closeAppAfterSave = false)
    {
        yield return new WaitForSeconds(0.5f); // Let other processes run before saving
        _saveables?.ForEach(s =>
        {
            ISaveableObject data = s.GetData();
            SaveItem(data);
        });
        if (closeAppAfterSave)
        {
            Application.Quit();
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
#endif
        }
    }

    /// <summary>
    /// Reads the save file and loads the data into the game. If the file does not exist, 
    /// the load function will be called with the data it got from s.GetData();
    /// </summary>
    /// <param name="delay">How long to wait before initiating the load</param>
    /// <returns></returns>
    private IEnumerator InitiateLoadCore(float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay); // Let other processes run before loading
        _saveables?.ForEach(s =>
        {
            ISaveableObject data = s.GetData();
            string saveFileLocation = BuildSaveFileName(data);
            if (File.Exists(saveFileLocation))
            {
                string stringData = File.ReadAllText(saveFileLocation);
                var saveData = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringData);
                if (saveData.ContainsKey("ObjectType"))
                {
                    var type = Type.GetType(saveData["ObjectType"]);
                    data = (ISaveableObject)JsonConvert.DeserializeObject(saveData["Data"], type, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });
                }
            }
            s.LoadData(data);
        });
    }
}
