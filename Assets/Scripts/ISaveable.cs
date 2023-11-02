public interface ISaveable
{
    public ISaveableObject GetData();
    /// <summary>
    /// Called to let the component know it should get the relevant data.
    /// </summary>
    public void LoadData(ISaveableObject item);

    public string GetObjectName();
}

public interface ISaveableObject
{
    public string GetObjectType();
    public string GetName();
}