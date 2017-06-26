namespace HQFrameWork
{
    using System;
    using UnityEngine;

    [Serializable]
    public enum UGUICtrlType
    {
        UNKNOW,
        DIALOG,
        IMAGE,
        RAWIMAGE,
        BUTTON,
        PANEL,
        TEXT,
        CHECKBOX,
        SLIDER,
        SCROLLBAR,
        DROPDOWN,
        INPUTFIELD,
        LISTVIEW,
        TABPANEL,      //页签挂载，临时加载的页签
        HyperLinkText, //支持超链接的Text
    }

    public abstract class UGUICtrlBase : UGUIMonoBehaviour
    {
        [HideInInspector]
        public bool isInit = false;//是否已初始化

        public abstract UGUICtrlType ctrlType
        {
            get;
        }

        /// <summary>
        /// 控件加一个空的Init方法，方便扩展控件,获取控件时候就会调用此方法
        /// </summary>
        public virtual void Init()
        {
            isInit = true;
        }
    }
}