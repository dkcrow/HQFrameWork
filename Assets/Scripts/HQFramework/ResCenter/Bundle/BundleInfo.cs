using UnityEngine;
using System.Collections;
using System.Xml.Linq;

namespace HQFrameWork
{
    public class BundleInfo
    {
        private string bundleName;
        /// <summary>
        /// bundle名称(带hashcode)
        /// </summary>
        public string BundleName
        {
            get
            {
                return bundleName;
            }

            set
            {
                bundleName = value;
            }
        }
        private Hash128 hashCode;
        public Hash128 HashCode
        {
            get
            {
                return hashCode;
            }
            set
            {
                hashCode = value;
            }
        }
        private string[] dependencies;
        /// <summary>
        /// 获取此bundle所有依赖bundle名称
        /// </summary>
        public string[] Dependencies
        {
            get
            {
                return dependencies;
            }
            set
            {
                dependencies = value;
            }
        }
        private int refCount = 0;     //引用数量
        /// <summary>
        /// 资源引用数量
        /// </summary>
        public int RefCount
        {
            get
            {
                return refCount;
            }

            set
            {
                refCount = value;
            }
        }

        public LoadOptions Options
        {
            get
            {
                return options;
            }

            set
            {
                options = value;
            }
        }

        private LoadOptions options;


    }
}
