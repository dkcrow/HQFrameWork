using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFrameWork
{
    public class ResManager : NormalSingleton<ResManager>
    {
        /// <summary>
        /// bundle配置信息
        /// </summary>
        public AssetBundleManifest manifest;
        /// <summary>
        /// 获取的服务器资源列表信息,注意此处键值是资源名，不是bundle文件名称!!!!!
        /// </summary>
        public Dictionary<string, BundleInfo> dicAllBundleInfo = new Dictionary<string, BundleInfo>();
        /// <summary>
        /// 已经存在加载完成列表的bundle信息,注意此处键值是bundle文件名称!!!!!
        /// </summary>
        public Dictionary<string, LoadedBundleInfo> dicLoadedBundle = new Dictionary<string, LoadedBundleInfo>();
        /// <summary>
        /// 加载好了的单个资源保存
        /// </summary>
        private Dictionary<string, LoadedSingleRes> dicLoadedSingleRes = new Dictionary<string, LoadedSingleRes>();


        /// <summary>
        /// 边玩变下资源后缀
        /// </summary>
        public const string downloadingSuffix = ".download";
        public const string unManagedBundle = ".unmanagerd";
        /// <summary>
        /// 加载完成自动卸载bundle
        /// </summary>
        public bool autoUnloadBundle = true;
        /// <summary>
        /// 使用本地资源
        /// </summary>
        public bool useLocalAssets = true;

#if UNITY_EDITOR
        private bool bundleEsimulate = false;

        /// <summary>
        /// bundle模拟模式(只在Editor下生效)，此模式下会使用本地资源代替对应的bundle
        /// </summary>
        public bool BundleEsimulate
        {
            get
            {
                return bundleEsimulate;
            }
            set
            {
                bundleEsimulate = value;
            }
        }

#endif
        private ILoader resLoad;

        /// <summary>
        /// 资源加载器
        /// </summary>
        private ILoader loader
        {
            get
            {
                if (null == resLoad)
                {
#if UNITY_EDITOR
                    //resLoad = new AndroidLoader();
                    resLoad = new EditorLoader();
#elif UNITY_ANDROID && !UNITY_EDITOR
                 resLoad = new AndroidLoader();
#elif UNITY_IPHONE && !UNITY_EDITOR
                 resLoad = new IOSLoader();
#else
                resLoad = new PCLoader();
#endif
                }
                return resLoad;
            }
        }

        public ResManager()
        {
            
        }

        /// <summary>
        /// 添加已经加载的资源到资源列表
        /// </summary>
        /// <param name="path"></param>
        /// <param name="resName"></param>
        /// <param name="obj"></param>
        public void AddLoadedSingleRes(string resName, UnityEngine.Object obj, LoadOptions options)
        {
            //保存的资源名字都是小写
            resName = resName.ToLower();
            LoadedSingleRes loadedSingleRes = new LoadedSingleRes();
            loadedSingleRes.resName = resName;
            loadedSingleRes.obj = obj;
            loadedSingleRes.options = options;
            dicLoadedSingleRes[resName] = loadedSingleRes;
        }
        /// <summary>
        /// 加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName">相对于资源文件夹全路径,带后缀,such as : ResourceAB/UI/MainDialogs.png</param>
        /// <param name="callBack">回调</param>
        /// <returns></returns>
        public void Load<T>(string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            string rName = resName.ToLower();
            if (dicLoadedSingleRes.ContainsKey(rName))
            {
                callBack(dicLoadedSingleRes[rName].obj as T, args);
            }
            else
            {
                loader.Load<T>(resName, (t, ags) =>
                {
                    callBack(t, ags);
                    if (autoUnloadBundle)
                    {
                        MonoEvent.Instance.StartCoroutine(UnloadBundleInternal(rName));
                    }
                }, options, args);
            }
        }
        /// <summary>
        /// 加载streamingassets文件中文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="callBack"></param>
        /// <param name="args"></param>
        public void LoadFile(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params System.Object[] args)
        {
            loader.LoadFile(fileName, callBack, args);
        }

        public void LoadPackage(string packageName, List<string> resList, ResDelegate.ResListLoadedDone loadedDone, ResDelegate.LoadingProgress loadingProgress, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args)
        {
            loader.LoadPackage(packageName, resList, loadedDone, loadingProgress, options, args);
        }
        /// <summary>
        /// 卸载bundle
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="includeUnManagedRes"></param>
        /// <returns></returns>
        public IEnumerator UnloadBundleInternal(string resName, bool includeUnManagedRes = false)
        {
            yield return new WaitForEndOfFrame();
            string rName = resName.ToLower();
            if (dicAllBundleInfo.ContainsKey(rName))
            {
                string bundleName = dicAllBundleInfo[rName].BundleName;
                if (dicLoadedBundle.ContainsKey(bundleName))
                {
                    LoadedBundleInfo info = dicLoadedBundle[bundleName];
                    if ((info.Options & LoadOptions.UnManagedRes) != LoadOptions.UnManagedRes || includeUnManagedRes)
                    {
                        info.Bundle.Unload(false);
                        dicLoadedBundle.Remove(bundleName);
                        UnloadDe(dicAllBundleInfo[rName].Dependencies);
                    }
                }
            }

        }
        /// <summary>
        /// 卸载依赖
        /// </summary>
        /// <param name="deBundles"></param>
        private void UnloadDe(string[] deBundles)
        {
            for (int i = 0; i < deBundles.Length; i++)
            {
                string bundleName = deBundles[i];
                if (dicLoadedBundle.ContainsKey(bundleName))
                {
                    LoadedBundleInfo info = dicLoadedBundle[bundleName];
                    if ((info.Options & LoadOptions.UnManagedRes) != LoadOptions.UnManagedRes)
                    {
                        info.Bundle.Unload(false);
                        dicLoadedBundle.Remove(bundleName);
                    }
                }
            }
        }
        /// <summary>
        /// 卸载全部资源,过场景调用
        /// </summary>
        public void UnLoadAllRes()
        {
            List<string> unloadRes = new List<string>();
            foreach (var item in dicLoadedSingleRes)
            {
                // if ((item.Value.options & LoadOptions.ManagedRes) == LoadOptions.ManagedRes)
                {
                    unloadRes.Add(item.Key);
                }
            }
            //卸载单个托管资源
            for (int i = unloadRes.Count - 1; i >= 0; i--)
            {
                UnLoadSingleRes(unloadRes[i]);
            }
            unloadRes.Clear();
            foreach (var item in dicLoadedBundle)
            {
                if ((item.Value.Options & LoadOptions.UnManagedRes) != LoadOptions.UnManagedRes)
                {
                    unloadRes.Add(item.Key);

                }
            }
            for (int i = unloadRes.Count - 1; i >= 0; i--)
            {
                UnloadBundleInternal(unloadRes[i]);
            }
            Resources.UnloadUnusedAssets();
        }
        /// <summary>
        /// 卸载单个资源
        /// </summary>
        /// <param name="path"></param>
        public void UnLoadSingleRes(string path)
        {
            path = path.ToLower();
            if (dicLoadedSingleRes.ContainsKey(path))
            {
                dicLoadedSingleRes.Remove(path);
            }
        }
    }

}

