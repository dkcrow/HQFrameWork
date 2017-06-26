using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

namespace HQFrameWork
{
    public class LoadBundleFromFile : ILoadResource
    {
        public void Load<T>(string path, string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            T res = null;
            //获取依赖
            string[] de = ResManager.Instance.dicAllBundleInfo[resName].Dependencies;
            for (int i = 0; i < de.Length; i++)
            {
                AssetBundle deBundle = LoadSingleBundle(path, de[i], options);
            }
            LoadSingleBundle(path, ResManager.Instance.dicAllBundleInfo[resName].BundleName, options);
            AssetBundle bundle = ResManager.Instance.dicLoadedBundle[ResManager.Instance.dicAllBundleInfo[resName].BundleName].Bundle;
            string[] s = null;
            if (ResManager.Instance.dicAllBundleInfo[resName].BundleName.Contains(ResManager.downloadingSuffix))
            {
                s = resName.Replace(ResManager.downloadingSuffix, "").Split('/');
            }
            else
            {
                s = resName.Split('/');
            }
            res = bundle.LoadAsset<T>(s[s.Length - 1]);
            SaveRes(res, resName, options);
            if (null != callBack) callBack(res, args);
        }
        private void SaveRes<T>(T res, string resName, LoadOptions options) where T : UnityEngine.Object
        {
            if (null != res)
            {
                //保存加载好的资源
                if (ResManager.Instance.dicAllBundleInfo[resName].BundleName.Contains(ResManager.downloadingSuffix))
                {
                    ResManager.Instance.AddLoadedSingleRes(resName.Replace(ResManager.downloadingSuffix, ""), res, (ResManager.Instance.dicAllBundleInfo[resName].Options | options));
                }
                else
                {
                    ResManager.Instance.AddLoadedSingleRes(resName, res, (ResManager.Instance.dicAllBundleInfo[resName].Options | options));
                }
            }
        }
        /// <summary>
        /// 加载单个bundle
        /// </summary>
        /// <param name="bundleName"></param>
        private AssetBundle LoadSingleBundle(string path, string bundleName, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath)
        {
            AssetBundle bundle = null;
            if (!ResManager.Instance.dicLoadedBundle.ContainsKey(bundleName))
            {
                bundle = AssetBundle.LoadFromFile(path + bundleName);
                if (null != bundle)
                {
                    LoadedBundleInfo loadedBundle = new LoadedBundleInfo();
                    loadedBundle.Bundle = bundle;
                    loadedBundle.BundleName = bundleName;
                    loadedBundle.Options = (bundleName.Contains(ResManager.unManagedBundle) ? LoadOptions.UnManagedRes : LoadOptions.ManagedRes) | options;
                    ResManager.Instance.dicLoadedBundle[bundleName] = loadedBundle;
                }
                else
                {
                    Debug.LogError("bundle加载失败   " + path + bundleName);
                }
            }
            else
            {
                LoadedBundleInfo info = ResManager.Instance.dicLoadedBundle[bundleName];
                bundle = info.Bundle;
            }
            return bundle;
        }
        private string GetResName(string bundleName)
        {
            string resName = "";
            if (bundleName.Contains(ResManager.unManagedBundle))
            {
                resName = bundleName.Replace(ResManager.unManagedBundle, "").Replace("_" + ResManager.Instance.manifest.GetAssetBundleHash(bundleName), "");
                Debug.Log("我是resName   " + resName);
            }
            else
            {
                resName = bundleName.Replace("_" + ResManager.Instance.manifest.GetAssetBundleHash(bundleName), "");
            }
            return resName;
        }
    }
}