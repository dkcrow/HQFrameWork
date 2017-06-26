using System.Collections.Generic;
using HQFrameWork;
using UnityEngine;

/// <summary>
/// wending
/// 游戏场景 如判断当前是哪个游戏场景用 这种场景都是作为activscene的要注意
/// </summary>
public enum EnumSceneType : byte
{
    [StringValue("DownLoadScene")] //没有实体场景
    STAGE_DOWNLOAD,
    [StringValue("InitScene")]//没有实体场景
    INIT,
    [StringValue("LoginScene")]
    LOGIN,
    [StringValue("BattleScene")]
    BATTLE,
    [StringValue("TestBattleScene")]
    TEST_BATTLE_SCENE,

}

/// <summary>
/// 场景驱动类抽象类
/// </summary>
abstract public class BaseScene
{
    public abstract EnumSceneType SceneType { get; }

   /// <summary>
   /// 资源加载 监听的add
   /// </summary>
    public abstract void OnEnterScene();
    /// <summary>
    /// 资源卸载,监听的remove
    /// </summary>
    public abstract void OnLeaveScene();

}
