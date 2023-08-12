using UnityEngine;

public interface IProjectile
{
    public void SetTarget(Transform target, float speed, float damage, float timeToDestroy = 3f);
}
