using System;
using System.Collections;
using UnityEngine;

namespace HQFrameWork
{
    public class MonoEvent : MBSingleton<MonoEvent>
{
        public event Action UPDATE;
        public event Action FIXEDUPDATE;
        public event Action ONGUI;
        public event Action LATEUPDATE;

        public MonoEvent()
        {
           
        }

        void Update()
        {
            if (UPDATE != null)
            {
                UPDATE();
            }
        }

        void FixedUpdate()
        {
            if (FIXEDUPDATE != null)
            {
                FIXEDUPDATE();
            }
        }

        void OnGUI()
        {
            if (ONGUI != null)
            {
                ONGUI();
            }
        }

        void LateUpdate()
        {
            if (LATEUPDATE != null)
            {
                LATEUPDATE();
            }
        }

        public void WaitForSeconds(float time, Action callBack)
        {
            StartCoroutine(WaitForSecondsCallBack(time, callBack));
        }

        IEnumerator WaitForSecondsCallBack(float time, Action callBack)
        {
            yield return new WaitForSeconds(time);
            if (callBack != null)
            {
                callBack();
            }
        }
    }

}
