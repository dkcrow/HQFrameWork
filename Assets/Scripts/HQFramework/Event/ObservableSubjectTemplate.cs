﻿//=====================================================================================/
//=====================================================================================/
using System;

///<summary>
///wending
///观察主题模板 任何数据类都可以成为主题
///<summary>
namespace HQFrameWork
{
    public abstract class ObservableSubjectTemplate<T1, T2, T3>
    {
        protected event Action<T1, T2, T3> m_delegate;

        /// <summary>
        /// 添加
        /// </summary>
        public void Attach(Action<T1, T2, T3> call)
        {
            // Delegate = (QKDelegateAction<T>)System.Delegate.Combine(Delegate, call); 不要用这个, 这个可以添加重复的
            m_delegate -= call;
            m_delegate += call;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="observer"></param>
        public void Detach(Action<T1, T2, T3> call)
        {
            m_delegate = (Action<T1, T2, T3>)Delegate.Remove(m_delegate, call);
        }

        /// <summary>
        /// 通知
        /// </summary>
        public void Notify(T1 arg1, T2 arg2, T3 arg3)
        {
            if (null != m_delegate)
            {
                m_delegate(arg1, arg2, arg3);
            }
        }
    }
}