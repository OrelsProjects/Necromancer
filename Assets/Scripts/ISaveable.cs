public interface ISaveable
{
    public ISaveableObject GetData();
    /// <summary>
    /// Called to let the component know it should get the relevant data.
    /// </summary>
    public void LoadData();
}

public interface ISaveableObject
{

}