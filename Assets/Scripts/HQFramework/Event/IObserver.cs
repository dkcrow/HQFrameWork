﻿//=====================================================================================/
///<summary>
///wending
///观察者接口
///<summary>
//=====================================================================================/
namespace HQFrameWork
{
    public interface IObserver
    {
        void OnDataChange(BaseData eventData, int type, object obj);
    }
}