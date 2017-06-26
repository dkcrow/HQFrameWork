//=====================================================================================/
/// <summary>
/// 
/// 事件分发处理类,一种固定格式的messenger而已,暂时不用
/// </summary>
//=====================================================================================.

namespace HQFrameWork
{
    public class EventDispatcher
    {
        public delegate void OnEventDelegate(object sender, EventArgs data);

        public event OnEventDelegate eventListener;
        /// <summary>
        /// 分发消息
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void DispatchEvent(EventDispatchType eventType, object data = null)
        {
            if (null != eventListener)
            {
                eventListener(this, new EventArgs(eventType, data));
            }
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        public void RegistEvent(OnEventDelegate fuc)
        {
            eventListener -= fuc;
            eventListener += fuc;
        }

        /// <summary>
        /// 注销监听
        /// </summary>
        public void UnRegistEvent(OnEventDelegate fuc)
        {
            eventListener -= fuc;
        }
    }
}

