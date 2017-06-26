using HQFrameWork;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Image扩展,jyj
/// </summary>
public static class ImageExtend
{
    public static void SetRes(this Image img, string path, string atlasName, string spriteName)
    {
        if (null == img)
        {
            //GameDebugLog.LogError("ImageExtend.CS Line14:   " + img.name + "为NULL!!!!!");
            return;
        }
        Vector4 border = Vector4.zero;
        if (null != img.sprite)
        {
            border = img.sprite.border;
        }
        GetSpriteFormAtlas.GetSprite(path, atlasName, spriteName, border, (sprite) =>
        {
            if (null != img)
            {
                img.sprite = sprite;
            }
            else
            {
                //GameDebugLog.LogError("ImageExtend.CS Line33:   " + path + atlasName + "/" + spriteName + "为NULL!!!!!");
            }
        });
    }

    public static void SetSpriteNULL(this Image img)
    {
        if (null != img)
        {
            ResManager.Instance.Load<Texture2D>("ResourcesAB/UI/nosprite.png", (t, args) => {
             if(null!= img) img.sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one / 2, 100, 1);
            });                     
        }
    }

    public static void SetResSingle(this Image img, string path, string spriteName)
    {
        img.sprite = null;
        Sprite sp = null;
        ResManager.Instance.Load<Texture2D>(path + spriteName, (texture, args) => {

            if (null != texture)
            {
                sp = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2);
                sp.name = spriteName;
               if(null!=img) img.sprite = sp;
            }
            else
            {
                //GameDebugLog.LogError(spriteName + "为NULL!!!!!");
            }
        });
    }
    /// <summary>
    /// 设置图片alpha
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="alpha"></param>
    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        Color c = graphic.color;
        c.a = alpha;
        graphic.color = c;
    }
}