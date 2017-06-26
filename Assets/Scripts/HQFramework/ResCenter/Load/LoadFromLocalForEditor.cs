#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using UnityEditor;
namespace HQFrameWork
{
    /// <summary>
    /// 编辑器加载本地资源,开发阶段可以使用此模式
    /// </summary>
    public class LoadFromLocalForEditor : ILoadResource
    {
        public void Load<T>(string path, string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            T res = AssetDatabase.LoadAssetAtPath<T>(path + resName);
            if (null != res)
            {
                ResManager.Instance.AddLoadedSingleRes(resName, res, options);
            }
            if (null != callBack) callBack(res, args);
        }
    }
}
#endif