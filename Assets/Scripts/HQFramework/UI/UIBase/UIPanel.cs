using UnityEngine;

namespace HQFrameWork
{
    public class UIPanel : UGUICtrlBase
    {
        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.PANEL;
            }
        }
    }
}