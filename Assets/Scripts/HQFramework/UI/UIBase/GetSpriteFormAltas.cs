using UnityEngine;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine.Events;

public class GetSpriteFormAtlas
{
    /// <summary>
    /// 从图集中获取sprite
    /// </summary>
    /// <param name="path"></param>
    /// <param name="atlasName"></param>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public static void GetSprite(string path, string atlasName, string spriteName, Vector4 border, UnityAction<Sprite> callBack)
    {
        Sprite sp = null;
        ResManager.Instance.Load<GameObject>(path + atlasName + ".prefab", (ats, args) =>
        {
            if (null != ats)
            {
                sp = ats.GetComponent<Atlas>().GetSprite(spriteName);
            }
        });
        if(null==sp)
        {

            ResManager.Instance.Load<Texture2D>("ResourcesAB/UI/nosprite.png", (t, args) =>
            {
                if (null != callBack && null != t)
                {
                    sp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one / 2, 100, 1);
                }
                else
                {
                    sp = null;
                }
            });
        }
        callBack(sp);
    }
}
