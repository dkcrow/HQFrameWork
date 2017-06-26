using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace HQFrameWork
{
    /// <summary>
    /// 资源不同加载方式
    /// </summary>
    public interface ILoadResource
    {
        /// <summary>
        /// 加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        void Load<T>(string path, string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object;
    }
}
