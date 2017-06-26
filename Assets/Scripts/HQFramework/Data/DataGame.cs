
/// <summary>
/// 整个游戏的一个流程
/// </summary>
using HQFrameWork;

public class DataGame : BaseData
{
    public enum EventType
    {
        /// <summary>
        /// 断线重连事件
        /// </summary>
        OnReconnect = 0,
        /// <summary>
        /// 选角界面进入游戏,主要是切换角色后通知界面更改数据用
        /// </summary>
        OnEnterGame = 1,
        /// <summary>
        /// 主角技能状态改变
        /// </summary>
        OnPlayerSkillChange = 2,
        /// <summary>
        /// 返回登录界面
        /// </summary>
        OnReturnLogin = 3,
        /// <summary>
        /// 创建角色
        /// </summary>
        OnCreatePlayer = 4,
        /// <summary>
        /// 初始化地图
        /// </summary>
        OnCreateMap = 5,
        /// <summary>
        /// 切换地图
        /// </summary>
        OnMapChanged,
    }


    public override DataType dataType
    {
        get
        {
            return DataType.DataEvent;
        }
    }


}

