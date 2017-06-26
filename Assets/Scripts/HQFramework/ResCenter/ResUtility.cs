using UnityEngine;
using System.IO;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace HQFrameWork
{
    public class ResUtility
    {
        /// <summary>
        /// 服务器bundle地址
        /// </summary>
        public const string SERVER_PATH = "http://172.16.10.52:8080/abtest/";

        //缓存目录
        public static string CACHE_PATH = Application.temporaryCachePath + "/Bundles/";

        //streamingassets目录
        public static string STREAMING_PATH = Application.streamingAssetsPath + "/";

        public const string AssetBundlesOutputPath = "AssetBundles";

        public static string GetPlatformName()
        {
            //#if UNITY_ANDROID
            //            return "Android";
            //#elif UNITY_IPHONE
            //            return "iOS";
            //#elif UNITY_STANDALONE_WIN 
            //             return "Windows";
            //#else
            //            return "";
            //#endif
            //EditorUserBuildSettings.activeBuildTarget不能在子线程调用，这里直接写死
#if UNITY_EDITOR
            return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
#else
            return GetPlatformForAssetBundles(Application.platform);
#endif
        }

#if UNITY_EDITOR

        private static string GetPlatformForAssetBundles(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "Android";

                case BuildTarget.iOS:
                    return "iOS";

                case BuildTarget.WebGL:
                    return "WebGL";

                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "Windows";

                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

#endif

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";

                case RuntimePlatform.IPhonePlayer:
                    return "iOS";

                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";

                case RuntimePlatform.WindowsPlayer:
                    return "Windows";

                case RuntimePlatform.OSXPlayer:
                    return "OSX";
                // Add more build targets for your own.
                // If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
                default:
                    return null;
            }
        }

        public static string ResourcesPath
        {
            get
            {
#if UNITY_EDITOR
                if (ResManager.Instance.useLocalAssets)
                {
                    return "";
                }
                else
                {
                    if (ResManager.Instance.BundleEsimulate)
                    {
                        return "Assets/";
                        // return Path.GetDirectoryName(Application.dataPath) + "/AssetBundles/" + Utility.GetPlatformName() + "/";
                    }
                    else
                    {
                        return Application.temporaryCachePath + "/Bundles/" + ResUtility.GetPlatformName() + "/";
                    }
                }
#elif UNITY_ANDROID && !UNITY_EDITOR
                 return Application.temporaryCachePath+ "/Bundles/" +ResUtility.GetPlatformName()+ "/";
#elif UNITY_IPHONE && !UNITY_EDITOR
                 return Application.temporaryCachePath+ "/Bundles/" +ResUtility.GetPlatformName()+ "/";
#else
                return "";
#endif
            }
        }
    }
}