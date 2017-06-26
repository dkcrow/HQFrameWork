using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Atlas : MonoBehaviour {
    public string atlasName = "";
    public List<Sprite> listSprite = new List<Sprite>();
    /// <summary>
    /// 获取Sprite
    /// </summary>
    /// <param name="spriteName"></param>
    /// <returns></returns> 
    public Sprite GetSprite(string spriteName)
    {
        int count = listSprite.Count;
        for (int i = 0; i < count; i++)
        {
            if (listSprite[i].name == spriteName)
                return listSprite[i];
        }
        return null;
    }
}
