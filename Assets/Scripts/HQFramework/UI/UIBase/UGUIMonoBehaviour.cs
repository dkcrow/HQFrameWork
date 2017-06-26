using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFrameWork
{
    public class UGUIMonoBehaviour :UIBehaviour{

        private RectTransform rectTrans;
        public RectTransform RectTrans
        {
            get
            {
                if (null == rectTrans) rectTrans = GetComponent<RectTransform>();
                return rectTrans;
            }
        }
        public virtual void Init(GameObject obj)
        {
            UGUIDialogManager.Instance.AddInstance(this);
        }
    }

}

