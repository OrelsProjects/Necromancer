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

    public delegate void DataFetchingLoadingDelegate(bool isLoading);
    public event DataFetchingLoadingDelegate OnDataFetchLoading;

    public delegate void DataSavingLoadingDelegate(bool isLoading);
    public event DataSavingLoadingDelegate OnDataSaveLoading;

    public List<ISaveable> _saveables;

    private bool IsLoadingFetch
    {
        set
        {
            OnDataFetchLoading?.Invoke(value);
        }
    }

    private bool IsLoadingSave
    {
        set
        {
            OnDataSaveLoading?.Invoke(value);
        }
    }

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

    #region subscriptions
    public void SubscribeToDataFetchLoading(DataFetchingLoadingDelegate dataFetchingLoadingDelegate)
    {
        OnDataFetchLoading += dataFetchingLoadingDelegate;
    }

    public void UnsubscribeToDataFetchLoading(DataFetchingLoadingDelegate dataFetchingLoadingDelegate)
    {
        OnDataFetchLoading -= dataFetchingLoadingDelegate;
    }

    public void SubscribeToDataSaveLoading(DataSavingLoadingDelegate dataSavingLoadingDelegate)
    {
        OnDataSaveLoading += dataSavingLoadingDelegate;
    }

    public void UnsubscribeToDataSaveLoading(DataSavingLoadingDelegate dataSavingLoadingDelegate)
    {
        OnDataSaveLoading -= dataSavingLoadingDelegate;
    }
    #endregion

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

    private string BuildSaveFileName(ISaveableObject saveableObject) => Application.persistentDataPath + "/" + saveableObject.GetName() + ".dat";
    private string BuildSaveFileName(string saveableObjectName) => Application.persistentDataPath + "/" + saveableObjectName + ".dat";

    private void InitiateLoad()
    {
        Game.Instance.SetState(GameState.Loading);
        StartCoroutine(InitiateLoadCore());
    }

    public void LoadItem(ISaveable item) => StartCoroutine(LoadItemCore(item));

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
                TypeNameHandling = TypeNameHandling.Auto,
                Converters = new List<JsonConverter> { new ZombieTypeConverter() }
            })
        }
    };
        string stringSaveData = JsonConvert.SerializeObject(saveData);
        File.WriteAllText(BuildSaveFileName(item), stringSaveData);
    }

    private IEnumerator LoadItemCore(ISaveable saveable)
    {
        try
        {
            ISaveableObject data = saveable.GetData();
            string saveFileLocation = BuildSaveFileName(saveable.GetObjectName());
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
            saveable.LoadData(data);
        }
        catch (Exception e)
        {
            // Debug.LogError(e);
            // Log error
            Debug.Log("Save file not found, loading default data: " + e);
        }
        yield return null;
    }

    private IEnumerator InitiateSaveCore(bool closeAppAfterSave = false)
    {
        IsLoadingSave = true;
        yield return new WaitForSeconds(0.5f); // Let other processes run before saving
        _saveables?.ForEach(s =>
        {
            ISaveableObject data = s.GetData();
            SaveItem(data);
        });
        IsLoadingSave = false;
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
        IsLoadingFetch = true;
        yield return new WaitForSeconds(delay); // Let other processes run before loading
        _saveables?.ForEach(saveable =>
        {
            try
            {
                ISaveableObject data = saveable.GetData();
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
                saveable.LoadData(data);
            }
            catch (Exception e)
            {
                // Debug.LogError(e);
                // Log error
                Debug.Log("Save file not found, loading default data: " + e);
            }
            finally
            {
                Game.Instance.SetState(GameState.Playing);
                IsLoadingFetch = false;
            }
        });
    }
}
