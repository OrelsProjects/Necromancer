using UnityEngine;

public interface IChaser<T> where T : MonoBehaviour, IChaseable
{
    void SetTarget();
    T GetTarget();
    bool IsTargetReached();
}