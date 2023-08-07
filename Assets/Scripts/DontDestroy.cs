using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}