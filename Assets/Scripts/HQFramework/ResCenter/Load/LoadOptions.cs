using UnityEngine;
using System.Collections;
namespace HQFrameWork
{
    /// <summary>
    /// 加载选项
    /// </summary>
    public enum LoadOptions
    {
        None = 0,
        /// <summary>
        /// 托管的资源
        /// </summary>
        ManagedRes = 1 << 1,
        /// <summary>
        /// 非托管的资源
        /// </summary>
        UnManagedRes = 1 << 2,
        /// <summary>
        /// 从本地加载资源
        /// </summary>
        LocalPath = 1 << 3,
        /// <summary>
        /// 自动决定加载哪种资源
        /// </summary>
        AutoPath = 1 << 4,
    }
}