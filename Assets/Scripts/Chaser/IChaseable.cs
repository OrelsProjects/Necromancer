public interface IChaseable {
    bool IsPriority();
    /// <summary>
    /// Returns true if the target is available.
    /// </summary>
    /// <returns> 
    ///   true if the target is available; otherwise, false.
    ///   If the target is not available, an exception will be thrown.
    /// </returns>
    bool IsAvailable();
}