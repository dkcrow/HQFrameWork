using System.Collections;
using UnityEngine;
namespace HQFrameWork
{
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UIButton : UGUICtrlBase
    {
        public Button button;
        public bool bExecute=true;
        private UnityAction onClick;
        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.BUTTON;
            }
        }
        void Start()
        {
            button.onClick.AddListener(BtnClick);
        }
        private void BtnClick()
        {
            if (null != onClick)
            {
                SoundManager.PlaySFX(CustomDefine.SFX_buttonClick);
                onClick();
            }
        }
        /// <summary>
        /// 按钮是否可用
        /// </summary>
        public bool Interactable
        {
            get { return button.interactable; }
            set { button.interactable = value; }
        }
        /// <summary>
        /// 恢复按钮的bExecute值
        /// </summary>
        /// <param name="time"></param>
        public void RecoverBtn(float time)
        {
            bExecute = false;
            StartCoroutine(RecoverBtnExecute(time));
        }
        IEnumerator RecoverBtnExecute(float time)
        {
            yield return new WaitForSeconds(time);
            bExecute = true;
        }
        public void AddOnClickListener(UnityAction click)
        {
            onClick -= click;
            onClick += click;
        }
        public void RemoveListener()
        {
            onClick = null;
        }
        public Graphic TargetGraphic
        {
            get { return button.targetGraphic; }
            set { button.targetGraphic = value; }    
        }
        /// <summary>
        //  Convenience function that converts the referenced Graphic to a Image, if possible.
        /// </summary>
        public Image img
        {
            get { return button.image;}
            set { button.image = img; }
        }
    }

}
