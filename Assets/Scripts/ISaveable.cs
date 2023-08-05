using System.Collections.Generic;

public interface ISaveable
{
    public IDTO GetData();
    /// <summary>
    /// Called to let the component know it should get the relevant data.
    /// </summary>
    public void LoadData();
}