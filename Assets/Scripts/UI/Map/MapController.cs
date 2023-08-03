using UnityEngine;

public class Map : MonoBehaviour
{

    public static Map Instance { get; private set; }

    [SerializeField]
    private Grid _map;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _map = new Grid(14, 8, 10f);
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);        // On mouse click, color the cell under the mouse
        if (Input.GetMouseButtonDown(0))
        {
            _map.ColorCell(mouseWorldPosition, Color.cyan);
        }
    }
}