namespace HQFrameWork
{
    using UnityEngine.UI;

    public class UIDropdown : UGUICtrlBase
    {
        public Dropdown dropdown;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.DROPDOWN;
            }
        }
    }

}
