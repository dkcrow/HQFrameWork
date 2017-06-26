using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace HQFrameWork
{
    /// <summary>
    /// 保存文件到本地
    /// </summary>
    public class SaveBundle
    {
        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        public void SaveBundleToCache(string fileName, byte[] bytes)
        {
            CreateNeedPathDir(fileName);
            //FileInfo fi = new FileInfo(Utility.CACHE_PATH +Utility.GetPlatformName() +"/"+ fileName);
            FileStream file_stream = File.Open(ResUtility.CACHE_PATH + ResUtility.GetPlatformName() + "/" + fileName, FileMode.OpenOrCreate);
            file_stream.Write(bytes, 0, bytes.Length);
            file_stream.Close();
            file_stream.Dispose();
        }
        /// <summary>
        /// 创建需要的文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CreateNeedPathDir(string path)
        {
            string[] s = path.Split('/');
            if (s.Length > 0)
            {
                path = path.Replace(s[s.Length - 1], "");
            }
            //创建文件夹
            if (!string.IsNullOrEmpty(path))
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (!dir.Exists)
                {
                    dir.CreateSubdirectory(path);
                }
            }
        }
    }
}
