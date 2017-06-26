using UnityEngine;
using System.Collections;
namespace HQFrameWork
{
    public class LoadedBundleInfo
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
        private AssetBundle bundle;

        public AssetBundle Bundle
        {
            get
            {
                return bundle;
            }

            set
            {
                bundle = value;
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
        //private int refCount = 0;
        //public int RefCount
        //{
        //    get
        //    {
        //        return refCount;
        //    }

        //    set
        //    {
        //        refCount = value;
        //    }
        //}
        /// <summary>
        /// 加载选项
        /// </summary>
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



        private LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath;
    }
}