

namespace HQFrameWork
{
    using UnityEngine.UI;

    public class UIInputField : UGUICtrlBase
    {
        public InputField inputField;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.INPUTFIELD;
            }
        }
        /// <summary>
        /// InputField是否可用
        /// </summary>
        public bool Interactable
        {
            get { return inputField.interactable; }
            set { inputField.interactable = value; }
        }
        public string Text
        {
            get
            {
                return inputField.text;
            }
            set
            {
                inputField.text = value;
            }
        }
    }

}
