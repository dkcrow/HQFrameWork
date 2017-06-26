//using MobileBaseCommon.Decrypt;
using System.Collections;
using MobileBaseCommon.Decrypt;
using UnityEngine;

namespace HQFrameWork
{
    /// <summary>
    /// 加载单个文件,暂未支持加密和解压缩
    /// </summary>
    public class LoadFile
    {
        public void LoadFromEsimulate(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params System.Object[] args)
        {
           
        }

        public void LoadFromStreamingAssets(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params System.Object[] args)
        {
            MonoEvent.Instance.StartCoroutine(Load(fileName, callBack, args));
        }

        public void LoadFromBundle(string fileName, ResDelegate.LoadedDone<byte[]> callBack, LoadOptions options = LoadOptions.ManagedRes | LoadOptions.AutoPath, params System.Object[] args)
        {
            LoadManager.Instance.LoadBundleFromFile.Load<TextAsset>(ResUtility.ResourcesPath, fileName, (t, a) => {
                callBack(t.bytes);
            }, options, args);
        }

        private IEnumerator Load(string fileName, ResDelegate.LoadedDone<byte[]> callBack, params System.Object[] args)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
              WWW www = new WWW(Application.streamingAssetsPath+"/"+fileName);         
#else
            WWW www = new WWW("file://" + Application.streamingAssetsPath + "/" + fileName);
#endif
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                byte[] data = www.bytes;
                data = Decrypt.DecryptData(data);//解密 todo:解密
                if (null != callBack) callBack(data, args);
            }
            else
            {
                Debug.LogError("文件" + Application.streamingAssetsPath + "/" + fileName + "加载错误" + www.error);
                if (null != callBack) callBack(null, args);
            }
            www.Dispose();
        }
    }
}