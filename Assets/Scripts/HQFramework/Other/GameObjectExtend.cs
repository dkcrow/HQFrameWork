using UnityEngine;

public static class GameObjectExtend
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        if (go == null)
        {
            return null;
        }
        T component = go.GetComponent<T>();
        if (component != null)
        {
            return component;
        }
        return go.AddComponent<T>();
    }

    public static RectTransform GetRectTransform(this GameObject go)
    {
        if (go == null)
        {
            return null;
        }

        return go.GetComponent<RectTransform>();
    }
}
