namespace HQFrameWork
{
    using UnityEngine;

    public static class UGUIEditorUtility
    {
        public static GameObject LoadObject(string resourceName)
        {
            UnityEngine.Object obj = Resources.Load(resourceName);
            if (obj == null)
            {
                Debug.Log("资源加载失败:" + resourceName);
                return null;
            }
            else
            {
                return GameObject.Instantiate(obj) as GameObject;
            }
        }
    }

}
