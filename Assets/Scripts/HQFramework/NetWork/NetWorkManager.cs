using HQFrameWork;

public class NetWorkManager : MBSingleton<NetWorkManager>
{
    NetWorkManager()
    {
        singletonType=SingletonType.GlobalInstance;
    }

    public void Init()
    {
  

    }
    #region 接收部分

    public void OnReceiveMsgFromServer(object msg)
    {
        ///...规则
    }
    #endregion

    #region 请求部分
    public void SendMovLeftRequest()
    {
        
    }

    public void SendMovRightRequest()
    {

    }

    public void SendAccelerateRequest()
    {

    }

   
#endregion
   

}
