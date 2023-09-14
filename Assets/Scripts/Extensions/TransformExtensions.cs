using System;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform transform)
    {
        int childCount = transform.childCount;

        // Loop through all children and destroy them.
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            // Check if the child has a MonoBehaviour component attached.
            
            if (child.TryGetComponent<MonoBehaviour>(out var _))
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            else
            {
                throw new InvalidOperationException("Child must have a MonoBehaviour component to be destroyed.");
            }
        }
    }
}
