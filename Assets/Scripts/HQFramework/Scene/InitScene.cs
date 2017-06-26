
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HQFrameWork;
//=====================================================================================/
/// <summary>
/// weding
/// 初始化全局数据阶段, 该阶段在login之前  在其它阶段跳转到该login不用重新准备数据
/// </summary>
//=====================================================================================.
public class InitScene : BaseScene
{
    public enum InitStagePhaseType
    {
       
        [StringValue("LoadGameDBConfigPhase")]
        LOAD_GAME_DB_CONFIG_PHASE, //加载csv
        /// <summary>
        /// 场景引导相关
        /// </summary>
        [StringValue("InitGuidePhase")]
        INIT_GUIDE_PHASE,

        //加载所有配置文件
        [StringValue("LoadAllSettingPhase")]
        LOAD_ALL_SETTING_PHASE,

        //游戏UIXML配置
        [StringValue("LoadAllUIXML")]
        LOAD_ALL_UI_XML,

        //游戏初始全局对象生成
        [StringValue("InitGlobeObject")]
        INIT_GLOBE_OBJECT,

        //加载下个场景 暂时是login场景
        [StringValue("GoToNextStagePhase")]
        GOTO_NEXT_STAGE_PHASE
    }

    //执行顺序例表
    private Queue<InitStagePhaseType> m_quePhaseOrder = new Queue<InitStagePhaseType>();
    private int mUIXmlFileCount = 0;

    
    /// <summary>
    /// 改变初始化stage
    /// </summary>
    /// <param name="phaseType"></param>
    private void ChangePhase()
    {
        if (m_quePhaseOrder.Count < 1)
        {
            //GameDebugLog.LogWarning("初始化场景流程出错,请检查!");
            return;
        }
        InitStagePhaseType phaseType = m_quePhaseOrder.Dequeue();
        Type stageClassType = GetType();
        //根据 enum值 或得方法
        MethodInfo phaseMethod = stageClassType.GetMethod(phaseType.GetStringValue());
        if (null == phaseMethod) return;
        phaseMethod.Invoke(this, null);
    }

    /// <summary>
    /// 加载全部配置文件
    /// </summary>
    public void LoadGameDBConfigPhase()
    {
        GameDBConfig.Instance.Init(OnOnePhaseComplete);
    }

    /// <summary>
    /// 一个流程完成
    /// </summary>
    private void OnOnePhaseComplete()
    {
        ChangePhase();
    }
    
    /// <summary>
    /// 现在只有这一个以后添加再修改
    /// </summary>
    public void LoadAllSettingPhase()
    {
       
        OnOnePhaseComplete();
    }

    public void InitGlobeObject()
    {
        OnOnePhaseComplete();
    }
    /// <summary>
    /// 下个场景
    /// </summary>
    public void GoToNextStagePhase()
    {
        MonoEvent.Instance.StartCoroutine(Change());
    }
    System.Collections.IEnumerator Change()
    {
        yield return new WaitForEndOfFrame();
        //HQSceneManager.Instance.ChangeScene<LoginScene>();//todo:暂时跳过登录进入测试场景
        HQSceneManager.Instance.ChangeScene<BattleScene>();//todo:暂时跳过登录进入测试场景
    }
    ///注意 初始化场景是不要加场景的 只是类转到初始化类, 除非初始化场景需要单独场景
    //=================================================================================================

    #region 抽象基类方法
    /// <summary>
    /// 初始化
    /// </summary>
  

    public override EnumSceneType SceneType
    {
        get
        {
            return EnumSceneType.INIT;
        }
    }

    public override void OnEnterScene()
    {
        m_quePhaseOrder.Enqueue(InitStagePhaseType.LOAD_GAME_DB_CONFIG_PHASE);
        m_quePhaseOrder.Enqueue(InitStagePhaseType.GOTO_NEXT_STAGE_PHASE);//这里不用像原来那样,直接加载完csv就进入下一场景,以后用到别的步骤就再enqueue,按流程顺序来add

        ChangePhase();
    }

    public override void OnLeaveScene()
    {
        
    }

    #endregion
}