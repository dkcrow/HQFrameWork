using System.Collections.Generic;

namespace HQFrameWork
{
    /// <summary>
    /// 加载器,用来做平台差异,判断资源那种方式加载
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// 加载单个资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="resName"></param>
        /// <returns></returns>
        void Load<T>(string resName, ResDelegate.LoadedDone<T> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args) where T : UnityEngine.Object;
        /// <summary>
        /// 加载文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="callBack"></param>
        /// <param name="args"></param>
        void LoadFile(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params System.Object[] args);
        /// <summary>
        /// 加载资源列表
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="resList"></param>
        /// <param name="loadedDone"></param>
        /// <param name="loadingProgress"></param>
        /// <param name="args"></param>
        void LoadPackage(string packageName, List<string> resList, ResDelegate.ResListLoadedDone loadedDone, ResDelegate.LoadingProgress loadingProgress, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args);
    }
}
