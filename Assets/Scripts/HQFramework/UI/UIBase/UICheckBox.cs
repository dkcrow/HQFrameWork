namespace HQFrameWork
{
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UICheckBox : UGUICtrlBase
    {
        public Toggle checkBox;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.CHECKBOX;
            }
        }

        public bool IsOn
        {
            get
            {
               return checkBox.isOn;
            }
            set
            {
                checkBox.isOn = value;
            }
        }
        public void OnValueChanged(UnityAction<bool> onValueChanged)
        {
            checkBox.onValueChanged.AddListener(onValueChanged);
        }
        /// <summary>
        /// toggle是否可以点击
        /// </summary>
        public bool Interactable
        {
            get { return checkBox.interactable; }
            set { checkBox.interactable = value; }
        }
    }

}
