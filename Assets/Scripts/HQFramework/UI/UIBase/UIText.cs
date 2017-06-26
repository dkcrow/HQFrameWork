using UnityEngine.UI;
using System;
using UnityEngine;
namespace HQFrameWork
{
  

    public class UIText : UGUICtrlBase
    {
        public Text text;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.TEXT;
            }
        }
        /// <summary>
        /// 设置文本
        /// </summary>
        public string Text
        {
            get
            {
                return text.text;
            }
            set
            {
                text.text = value;
            }
        }
    }

}
