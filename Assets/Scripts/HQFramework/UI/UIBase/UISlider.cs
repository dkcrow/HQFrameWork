namespace HQFrameWork
{
    using UnityEngine.UI;

    public class UISlider : UGUICtrlBase
    {
        public Slider slider;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.SLIDER;
            }
        }
    }

}
