using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {
    public static Map Instance { get; private set; }

    [SerializeField]
    private DragCamera _dragCamera;

    [SerializeField]
    private List<AreaData> _areas = new();

    public AreaData SelectedArea;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void DisableMovement() {
        _dragCamera.DisableMovement();
    }

    public void EnableMovement() {
        _dragCamera.EnableMovement();
    }

    public void LoadArea1() {
        LoadArea(Areas.Area1);
    }

    public void LoadArea2() {
        LoadArea(Areas.Area2);
    }

    public void LoadArea(Areas area) {
        AreaData areaData = _areas.Find(a => a.Area == area);
        if (areaData != null) {
            SelectedArea = areaData;
            UnityEngine.SceneManagement.SceneManager.LoadScene(areaData.AreaNameString);
        }
    }
}