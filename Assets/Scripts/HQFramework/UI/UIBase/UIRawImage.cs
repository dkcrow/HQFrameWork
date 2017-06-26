namespace HQFrameWork
{
    using UnityEngine.UI;

    public class UIRawImage : UGUICtrlBase
    {
        public RawImage rawImage;

        public override UGUICtrlType ctrlType
        {
            get
            {
                return UGUICtrlType.RAWIMAGE;
            }
        }
    }

}

