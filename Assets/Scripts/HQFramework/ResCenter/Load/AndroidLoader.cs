using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.IO;
using System.Collections.Generic;


namespace HQFrameWork
{
    public class AndroidLoader : ILoader
    {
        public void Load<T>(string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            if (ResManager.Instance.useLocalAssets)
            {
                LoadManager.Instance.LoadFromResources.Load<T>("", resName, callBack,options, args);
            }
            else
            {
                if ((options & LoadOptions.LocalPath) == LoadOptions.LocalPath)
                {   //加载本地资源
                    LoadManager.Instance.LoadFromResources.Load<T>("", resName, callBack, options, args);
                    return;
                }  
               
            }
        }

        public void LoadFile(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params object[] args)
        {
            if (ResManager.Instance.useLocalAssets)
            {
                //本地资源从StreamingAssets加载
                LoadManager.Instance.LoadFile.LoadFromStreamingAssets(fileName, callBack, args);
            }
            else
            {
                    string rName = "resourcesab/" + fileName.ToLower();
                    if (ResManager.Instance.dicAllBundleInfo.ContainsKey(rName))
                    {
                        //从缓存加载bundle
                        LoadManager.Instance.LoadFile.LoadFromBundle(rName, callBack, LoadOptions.ManagedRes, args);
                    }
                    else
                    {
                        LoadManager.Instance.LoadFile.LoadFromStreamingAssets(fileName, callBack, args);
                    }
            }
        }

        public void LoadPackage(string packageName, List<string> resList, ResDelegate.ResListLoadedDone loadedDone, ResDelegate.LoadingProgress loadingProgress, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params object[] args)
        {
            int index = 0;
            for (int i = 0; i < resList.Count; i++)
            {
                Load<GameObject>(resList[i], (obj, ags) => {
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
