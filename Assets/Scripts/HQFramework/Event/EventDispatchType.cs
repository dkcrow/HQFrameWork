//=====================================================================================/
/// <summary>
/// weding
/// 所有事件DispatchType
/// </summary>
//=====================================================================================.
public enum EventDispatchType
{
    //input相关
    EventInputPressed, //这里是一直按下的,按住的

    EventInputPressDown,//这里是刚按下那一帧
    EventInputPressedUp, //这里是松开那一帧
    EventInputClick,    //点击

    //技能
    EVENT_PLAY_SKILL,

    //hub
    EvenHudVisible,

    EvenVisualModelVisible,

    //自已死亡, 不是自已 是所有Actor自身死亡消息
    EventSelfDead,

    EventSelfRelive,

    /// <summary>
    /// 计时器运行帧
    /// </summary>
    Timer,

    /// <summary>
    /// 计时器运行状态改变
    /// </summary>
    TimerRunChange,

    //ui相关
    MSG_SKILL_UPDATESKILLINFO,

    MSG_SHOP_UPDATEMONEY,

    //Plot
    PlotEnd,

    //Task
    TASK_NEWCREATE,

    TASK_UPDATE,
    TASK_FINSH,
    TASK_CANCREATE,//可接任务

    //寻路
    MSG_AUTODES,//开始寻路

    //披风动作
    COPE_ACTION,

    //火墙
    FIRE_SKILL
}