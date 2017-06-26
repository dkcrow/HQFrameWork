using System;

namespace  HQFrameWork
{
    public class ResDelegate 
    {

        /// <summary>
        /// 加载完成回调
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public delegate void LoadedDone<T>(T obj, params object[] args);
        /// <summary>
        /// 列表加载完成
        /// </summary>
        /// <param name="listName"></param>
        /// <param name="args"></param>
        public delegate void ResListLoadedDone(string listName, params System.Object[] args);
        /// <summary>
        /// 加载进度
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="progress"></param>
        /// <param name="args"></param>
        public delegate void LoadingProgress(string resName, float progress, params System.Object[] args);
        /// <summary>
        /// 下载完成
        /// </summary>
        public delegate void DownloadFinish(string fileName, Exception e = null);
        /// <summary>
        /// 下载进度
        /// </summary>
        /// <param name="t"></param>
        public delegate void DownloadProgress(long bytesReceived, long totalBytesToReceive, int progressPercentage);
    }

}

