namespace HQFrameWork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class UGUIDialogBase : UGUIMonoBehaviour
    {
        private Dictionary<string, UGUICtrlBase> m_ctrlDic = new Dictionary<string, UGUICtrlBase>();
        public string dlgName = "";
        /// <summary>
        ///
        /// </summary>
        /// <seealso cref="GetCtrlByName"/>
        /// <param name="ctrlName"></param>
        /// <returns></returns>
        public virtual UGUICtrlBase this[string ctrlName]
        {
            get
            {
                if (m_ctrlDic.ContainsKey(ctrlName))
                {
                    if (!m_ctrlDic[ctrlName].isInit)
                    {
                        m_ctrlDic[ctrlName].Init();
                        return m_ctrlDic[ctrlName];
                    }
                    else
                    {
                        return m_ctrlDic[ctrlName];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 获取控件
        /// </summary>
        /// <param name="ctrlName">控件名称</param>
        /// <returns></returns>
        [SerializeField]
        public UGUICtrlBase GetCtrlByName(string ctrlName)
        {
            return this[ctrlName];
        }

        public override void Init(GameObject obj)
        {
            base.Init(obj);//init 里会把单例添加到uihelper里,所以base.init不能少
            UGUICtrlBase[] ctrls = GetComponentsInChildren<UGUICtrlBase>(true);
            foreach (UGUICtrlBase ctrlBase in ctrls)
            {
                m_ctrlDic[ctrlBase.gameObject.name] = ctrlBase;
            }
        }
        public virtual void ShowDlg(bool bShow /*动作回调 是否用动作*/)
        {
            gameObject.SetActive(bShow);      
        }

        /// <summary>
        /// 过场景时候调用这个方法,过场景需要隐藏或者销毁的可以在这里操作
        /// </summary>
        public virtual void OnChangeScene()
        {
           
        }
        /// <summary>
        /// 清除UI数据
        /// </summary>
        public virtual void ClearUI()
        {
        }
       
    }

}