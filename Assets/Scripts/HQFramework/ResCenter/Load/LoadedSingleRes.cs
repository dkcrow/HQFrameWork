using UnityEngine;
using System.Collections;
namespace HQFrameWork
{
    public struct LoadedSingleRes
    {
        /// <summary>
        /// 资源名称
        /// </summary>
        public string resName;
        /// <summary>
        /// 加载好的单个资源
        /// </summary>
        public UnityEngine.Object obj;
        /// <summary>
        /// 资源管理选项
        /// </summary>
        public LoadOptions options;
    }
}
