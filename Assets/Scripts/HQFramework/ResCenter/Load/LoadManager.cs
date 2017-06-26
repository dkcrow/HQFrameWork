using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace HQFrameWork
{

    /// <summary>
    /// 资源加载管理器,用来处理不同方式的加载
    /// </summary>
    public class LoadManager : NormalSingleton<LoadManager>
    {
#if UNITY_EDITOR
        private LoadFromLocalForEditor loadFromLocalForEditor;
        /// <summary>
        /// 编辑器本地加载
        /// </summary>
        public LoadFromLocalForEditor LoadFromLocalForEditor
        {
            get
            {
                if (null == loadFromLocalForEditor)
                {
                    loadFromLocalForEditor = new LoadFromLocalForEditor();
                }
                return loadFromLocalForEditor;
            }
        }

#endif
        private LoadBundleFromFile loadBundleFromFile;
        private LoadFromResources loadFromResources;

        /// <summary>
        /// 通过loadfromfile加载bundle
        /// </summary>
        public LoadBundleFromFile LoadBundleFromFile
        {
            get
            {
                if (null == loadBundleFromFile)
                {
                    loadBundleFromFile = new LoadBundleFromFile();
                }
                return loadBundleFromFile;
            }

        }
        /// <summary>
        /// 从resource中加载
        /// </summary>
        public LoadFromResources LoadFromResources
        {
            get
            {
                if (null == loadFromResources)
                {
                    loadFromResources = new LoadFromResources();
                }
                return loadFromResources;
            }
        }
        private LoadFile loadFile;
        /// <summary>
        /// 加载文件
        /// </summary>
        public LoadFile LoadFile
        {
            get
            {
                if (null == loadFile)
                {
                    loadFile = new LoadFile();
                }
                return loadFile;
            }
        }
    }
}
