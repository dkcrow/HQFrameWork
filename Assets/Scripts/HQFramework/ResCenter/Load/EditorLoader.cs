#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HQFrameWork
{
    /// <summary>
    /// 编辑器加载器
    /// </summary>
    public class EditorLoader : ILoader
    {
        public void Load<T>(string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            if (ResManager.Instance.useLocalAssets)
            {
                LoadManager.Instance.LoadFromResources.Load<T>(ResUtility.ResourcesPath, resName, callBack, options, args);
            }
           
        }
        public void LoadFile(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params object[] args)
        {
            if (ResManager.Instance.useLocalAssets)
            {
                //本地资源从StreamingAssets加载
                LoadManager.Instance.LoadFile.LoadFromStreamingAssets(fileName, callBack, args);
            }
          
        }

        public void LoadPackage(string packageName, List<string> resList, ResDelegate.ResListLoadedDone loadedDone, ResDelegate.LoadingProgress loadingProgress, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params object[] args)
        {
            int index = 0;
            for (int i = 0; i < resList.Count; i++)
            {
                Load<GameObject>(resList[i], (obj, ags) =>
                {
                    index++;
                    loadingProgress(packageName, (float)index / resList.Count);
                    if (index == resList.Count)
                    {
                        loadedDone(packageName, args);
                    }
                });
            }
        }
    }
}

#endif