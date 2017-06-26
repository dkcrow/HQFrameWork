using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

namespace HQFrameWork
{
    /// <summary>
    /// 从resources中加载资源
    /// </summary>
    public class LoadFromResources : ILoadResource
    {
        public void Load<T>(string path, string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object
        {
            string loadName = GetResName(resName);
            T res = Resources.Load<T>(path + loadName);

            if (null != res)
            {
                ResManager.Instance.AddLoadedSingleRes(resName, res, options);
            }
            if (null != callBack) callBack(res, args);
        }

        /// <summary>
        /// 把传过来的包含后缀名的完整路径转换成resources路径
        /// </summary>
        /// <returns></returns>
        private string GetResName(string resName)
        {
            string[] rN = resName.Split('.');
            string loadName = resName.Replace("." + rN[rN.Length - 1], "");
            return loadName;
        }
    }
}