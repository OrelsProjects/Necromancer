using UnityEngine;

public class Settings: MonoBehaviour {

    private bool _isCalculating;

    private void Start() {
        Application.targetFrameRate = 144;
    }
}
