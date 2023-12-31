using System;
using UnityEngine;
using UnityEngine.UI;

public enum WaveState
{
    NotStarted,
    Started,
    Stopped,
    Done,
}

public class WaveController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private Slider _waveSlider;
    [SerializeField]
    private TMPro.TextMeshProUGUI _waveText;

    private RoundData _data;
    private int _currentWave = 0;
    private WaveState _state = WaveState.NotStarted;

    private Lazy<float> TimeBetweenWaves => new(() => _data.TimeBetweenWaves);

    private Lazy<int> WavesCount => new(() => _data.WavesCount);

    public WaveState State => _state;
    public bool IsWaveInProgress => _state == WaveState.Started;
    
    private void Start()
    {
        SpawnWave(); // Spawn first wave on start.
        _currentWave = 1;
        _waveSlider.value = 0;
        UpdateWaveText();
    }

    public void Initialize(Areas area)
    {
        _data = AreasManager.Instance.GetAreaData(area).RoundData;
    }

    /// <summary>
    /// Spawn wave after round start.
    /// </summary>
    // public void StartWaves()
    // {
    //     SpawnWave();
    //     _state = WaveState.Started;
    //     _currentWave = 1;
    //     UpdateWaveText();
    // }

    /// <summary>
    /// Spawn first wave on start.
    /// </summary>
    public void StartWaves()
    {
        _state = WaveState.Started;
    }


    public void StopWaves() => _state = WaveState.Stopped;

    private void Update()
    {
        if (_state == WaveState.Stopped)
        {
            return;
        }
        if (WavesCount.Value == 1 && _state == WaveState.Started)
        {
            _state = WaveState.Done;
            _waveSlider.value = 1;
            UpdateWaveText();
            return;
        }
        if (_state == WaveState.Started)
        {
            if (_currentWave > WavesCount.Value)
            {
                _state = WaveState.Done;
                _waveSlider.value = 1;
                UpdateWaveText();
                return;
            }
            else
            {
                _waveSlider.value += 1 / TimeBetweenWaves.Value * Time.deltaTime;
                if (_waveSlider.value >= 1)
                {
                    _currentWave++;
                    UpdateWaveText();
                    if (_currentWave > WavesCount.Value)
                    {
                        _waveSlider.value = 1;
                    }
                    else
                    {
                        SpawnWave();
                        _waveSlider.value = 0;
                    }
                }
            }
        }
    }

    private void SpawnWave()
    {
        RoundManager.Instance.SpawnDefenders();
    }

    private void UpdateWaveText()
    {
        int currentWave = Mathf.Clamp(_currentWave, 0, WavesCount.Value);
        int maxWaves = Mathf.Clamp(WavesCount.Value, 1, WavesCount.Value);
        _waveText.text = $"Wave   {currentWave}/{maxWaves}";
    }
}