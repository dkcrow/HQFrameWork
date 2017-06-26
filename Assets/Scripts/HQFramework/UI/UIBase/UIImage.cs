namespace HQFrameWork
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIImage : UGUICtrlBase
    {
        public Image image;
        private Vector2 sizeDelta;

        /// <summary>
        /// 方向默认0代表横向，1代表竖向
        /// </summary>
        public int dir = 0;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.IMAGE;
            }
        }
        public void SetSprite(Sprite sp)
        {
            image.sprite = sp;
        }
        public void SetAlpha(float a)
        {
            Color c = image.color;
            c.a = 1;
            image.color = c;
        }
       

        /// <summary>
        /// 从图集设置图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="atlasName"></param>
        /// <param name="spriteName"></param>
        public void SetSprite(string path, string atlasName, string spriteName)
        {
            image.SetRes(path, atlasName, spriteName);
        }
        /// <summary>
        /// 设置单独的图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="spriteName"></param>
        public void SetSingleSprite(string path, string spriteName)
        {
            image.SetResSingle(path, spriteName);
        }
        public override void Init()
        {
            base.Init();
            sizeDelta = image.rectTransform.sizeDelta;
        }

        public float FillAmount
        {
            get
            {
                if (image.type == Image.Type.Filled)
                {
                    //填充
                    return image.fillAmount;
                }
                else
                {
                    if (dir == 0)
                    {
                        return sizeDelta.x == 0 ? 0 : image.rectTransform.sizeDelta.x / sizeDelta.x;
                    }
                    else
                    {
                        return sizeDelta.y == 0 ? 0 : image.rectTransform.sizeDelta.y / sizeDelta.y;
                    }
                }
            }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                if (image.type == Image.Type.Filled)
                {
                    //填充
                    image.fillAmount = value;
                }
                else
                {
                    Vector2 delta = image.rectTransform.sizeDelta;
                    if (dir == 0)
                    {
                        delta.x = sizeDelta.x * value;
                        image.rectTransform.sizeDelta = delta;
                    }
                    else
                    {
                        delta.y = sizeDelta.y * value;
                        image.rectTransform.sizeDelta = delta;
                    }
                }
            }
        }

        protected  void OnDestroy()
        {
         
            if(null != image)
            {
                image.sprite = null;
            }
        }
    }

}