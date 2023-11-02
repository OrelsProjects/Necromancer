using UnityEngine;

public enum TutorialItemType
{
    Press,
    PressAnywhere, // Press any key to continue
    Hover,
}

[System.Serializable]
public struct TutorialItem
{
    public GameObject gameObject;
    public TutorialItemType type;
    public bool isHighlighted;
    public int index; // Index of the item in the steps array, to know which item to show first
    public bool isCompleted;
    public string text;

    public void Complete()
    {
        isCompleted = true;
    }
}