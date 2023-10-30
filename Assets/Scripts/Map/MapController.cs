using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [SerializeField]
    private DragCamera _dragCamera;
    [SerializeField]
    private List<AreaData> _areas = new();

    public AreaData SelectedArea;

    private AudioSource _audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        PlayMapSound();
    }

    private void PlayMapSound()
    {
        _audioSource.clip = SoundsManager.GetBackgroundSound(BackgroundSoundTypes.Map);
        _audioSource.loop = true;
        _audioSource.volume = 0.35f;
        _audioSource.Play();
    }

    public void DisableMovement()
    {
        _dragCamera.DisableMovement();
    }

    public void EnableMovement()
    {
        _dragCamera.EnableMovement();
    }

    public void LoadArea1()
    {
        LoadArea(Areas.Area1);
    }

    public void LoadArea2()
    {
        LoadArea(Areas.Area2);
    }

    public void LoadArea3()
    {
        LoadArea(Areas.Area3);
    }

    public void LoadArea4()
    {
        LoadArea(Areas.Area4);
    }

    public void LoadArea5()
    {
        LoadArea(Areas.Area5);
    }

    public void LoadArea(Areas area)
    {
        AreaData areaData = _areas.Find(a => a.Area == area);
        if (areaData != null)
        {
            SelectedArea = areaData;
            UnityEngine.SceneManagement.SceneManager.LoadScene(areaData.AreaNameString);
        }
    }

    private void OnEnable()
    {
        if (SoundsManager.Instance)
        {
            PlayMapSound();
        }
    }
}