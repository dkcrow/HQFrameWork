namespace HQFrameWork
{
    using UnityEngine.UI;

    public class UIScrollbar : UGUICtrlBase
    {
        public Scrollbar scrollbar;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.SCROLLBAR;
            }
        }
    }

}
