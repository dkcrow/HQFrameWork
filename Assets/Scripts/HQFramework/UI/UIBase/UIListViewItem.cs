namespace HQFrameWork
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    // [RequireComponent(typeof(LayoutElement))]
    public class UIListViewItem : MonoBehaviour
    {

        [HideInInspector]
        private RectTransform rectTrans;
        public RectTransform RectTrans
        {
            get {
                if (null == rectTrans)
                {
                    rectTrans = GetComponent<RectTransform>();
                }
                return rectTrans;
            }
        }
        private int index = 0;
        [HideInInspector]
        public UIListView listBox;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                if (listBox)
                {
                    listBox.SetItemPostionForReUse(this);
                    listBox.onUpdateItem(this, index);
                }             
            }
        }
        void Awake()
        {      
            //Init();
        }
        /// <summary>
        /// 初始化方法
        /// </summary>
        public virtual void Init()
        {
            UGUICtrlBase[] ctrls = GetComponentsInChildren<UGUICtrlBase>(true);
            foreach (UGUICtrlBase c in ctrls)
            {
                m_ctrlDic[c.gameObject.name] = c;
            }
            UGUICtrlBase cTemp = GetComponent<UGUICtrlBase>();
            m_ctrlDic[gameObject.name.Replace("(Clone)","")] = cTemp;
        }
        public bool isChecked;
        public virtual void SetChecked(bool isChecked)
        {
            if (isChecked)
            {
                if(null!= listBox.nowSelectedItem && listBox.nowSelectedItem != this) listBox.nowSelectedItem.SetChecked(false);
                listBox.nowSelectedItem = this;
            } 
            this.isChecked = isChecked;                  
        }
        public UGUICtrlBase GetCtrlByName(string ctrlName)
        {
            if (m_ctrlDic.ContainsKey(ctrlName)) return this[ctrlName];
            return null;
        }
        private Dictionary<string, UGUICtrlBase> m_ctrlDic = new Dictionary<string, UGUICtrlBase>();
        public virtual UGUICtrlBase this[string transName]
        {
            get
            {
                if (m_ctrlDic.ContainsKey(transName))
                {
                    UGUICtrlBase ctr = m_ctrlDic[transName];
                    if (null != ctr)
                    {
                        ctr.Init();
                    }
                    return m_ctrlDic[transName];
                }
                return null;
            }
        }
        /// <summary>
        /// 添加时候调用
        /// </summary>
        public virtual void OnAdd()
        {

        }
        /// <summary>
        /// 移除时候调用
        /// </summary>
        public virtual void OnRemove()
        {

        }
    }

}
