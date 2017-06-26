using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有全局信息定义在这里
/// </summary>
public class CustomDefine
{
    //public enum EntityType
    //{
    //    Monster,
    //    Car,
    //    House,
    //    /// <summary>
    //    /// 高楼
    //    /// </summary>
    //    Building
    //}
    public const string Pool_Entity = "Entity";

    public const int WorldHalfWidth = 25;//一半的长宽,用来随机生成
    public const int WorldHalfHeight = 25;

    public const int RebornTime = 5;
    public const int AiCount = 20;

    public const string Tag_Player_Monster = "Player_Monster";
    public const string Tag_Player_Car = "Player_Car";
    public const string Tag_Building = "Building";
    public const string Tag_Wall = "Wall";
    public const string Tag_Plane = "Plane";

    /// <summary>
    /// 同步人物位置
    /// </summary>
    public struct PlayerPosSyncMsg
    {
        public int id;
        public float posX;
        public float posZ;
        
    }
    /// <summary>
    /// 创建实体
    /// </summary>
    public struct EntityOpMsg
    {
        public int type;
        public int id;
        public float posX;
        public float posY;

    }
    #region old 
    public const string ModelPrefabPath = "ResourcesAB/Prefabs/Model/";
    public const string OtherPrefabPath = "ResourcesAB/Prefabs/Other/";

    public static EnumDefine.ControlType CurrentControlType;

    public static string PlayerName;

    /// <summary>
    /// 自己控制的玩家
    /// </summary>
    public static PlayerControl SelfPlayerControl;

    public static int PlayerId=10;//表里的id从1开始需要本地保存 根据这个id从GameDBConfig.m_playerDataDic中拿属性和模型

    public static int Gold=99999;
    /// <summary>
    /// 放在resource目录下,供photon来生成实例,换肤时修改
    /// </summary>
//    public static string PlayerModelName= "Model/Characters/SF_Character_Human_Bartender";
    public const string PlayerModelPath= "Model/Characters/";

    /// <summary>
    /// 不加任何脚本的原生模型,用来商店展示
    /// </summary>
    public const string OrignalPlayerModelPath= "Model/OrignalCharacters/";
    //武器模型路径
    public const string WeaponModelPath = "Model/Weapon/";

    public const string BuffPath = "Model/Buff/";

    public const string Tag_Arrow = "Arrow";
    public const string Tag_Buff = "Buff";
    public const string Tag_Player = "Player";

    public const string LoginSceneName = "LoginScene";

    public const string SFX_attack = "attack";
    public const string SFX_buffGet = "buffGet";
    public const string SFX_buttonClick = "buttonClick";
    public const string SFX_death = "death";
    public const string SFX_hit = "hit";
    public const string SFX_jump = "jump";
    public const string SFX_Walk = "walk";
    /// <summary>
    /// 场地大小,用来随机放入buff,后期大小变化时也记得改
    /// </summary>
    public const int PlaneWidth = 4;

#endregion

}
