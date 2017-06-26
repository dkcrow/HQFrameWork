using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MakeAtlas : MonoBehaviour {

    [MenuItem("Tools/HQ_GenerateAtlas")]
    static private void CreateAtlas()
    {
        string spDir = Application.dataPath + "/Sprites/SrcSprite"; //源目录
        string spriteDir = Application.dataPath + "/Sprites/DstSprite";//目标目录

        //创建对应目录
        if (!Directory.Exists(spriteDir))
        {
            Directory.CreateDirectory(spriteDir);
        }

        DirectoryInfo rootDirInfo = new DirectoryInfo(spDir);
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())//这下面必须还有一级文件夹,否则无效
        {
            string atlasPath = spriteDir + "/" + dirInfo.Name;
            if (!Directory.Exists(atlasPath))
            {
                //创建对应的图集目录
                Directory.CreateDirectory(atlasPath);
            }
            string fullPath = dirInfo.FullName;
            if (Directory.Exists(fullPath))
            {
                DirectoryInfo direction = new DirectoryInfo(fullPath);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
                if (files.Length == 0) continue;
                GameObject obj = new GameObject();
                obj.name = "test";
                Atlas at = obj.AddComponent<Atlas>();
                at.listSprite.Clear();
                at.atlasName = dirInfo.Name;
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Name.EndsWith(".png")|| files[i].Name.EndsWith(".jpg"))
                    {
                        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(files[i].FullName.Substring(files[i].FullName.IndexOf("Assets")));
                        at.listSprite.Add(sp);
                        Debug.Log(sp);                 
                    }
                }
                string prefabPath = atlasPath + "/" + dirInfo.Name;
                PrefabUtility.CreatePrefab(prefabPath.Substring(prefabPath.IndexOf("Assets")) + ".prefab", obj);
                DestroyImmediate(obj);
            }
        }   
    }
}
