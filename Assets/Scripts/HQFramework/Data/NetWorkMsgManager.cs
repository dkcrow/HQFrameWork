using System.Collections;
using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

/// <summary>
/// 网络消息传到后全部放到这里
/// </summary>
public class NetWorkMsgManager:NormalSingleton<NetWorkMsgManager>  {
    public delegate void GetNetWorkMsgDelegate(string name, string data);
    public static event GetNetWorkMsgDelegate GetNetWorkMsgEvent;

    private bool init = false;
    // Use this for initialization
    

    /// <summary>
    /// 所有从网络接收的数据类都放在这里面
    /// </summary>
    void AddAllDataListener()
    {
        //GetNetWorkMsgEvent += DataSubjectManager.GetData<DataTest1>().OnGetNetWorkMsg;
        ////...
        ////...
        
    }

    void Init()
    {
        AddAllDataListener();
        init = true;
    }
    /// <summary>
    /// 派发消息,所有AddAllDataListener中监听的都会收到, 然后他们收到后 根据msgName过滤消息
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="data"></param>
    public  void BroadCastNetWorkMsg(string msgName, string data)
    {
        if(!init)Init();

        if(null==GetNetWorkMsgEvent)return;

        GetNetWorkMsgEvent(msgName, data);
    }
}
