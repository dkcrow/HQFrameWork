using UnityEngine;

public enum GameQualityTag
{
    Low = 0,
    Medium = 1,
    High = 2
}
/// <summary>
/// 玩家本地的一些偏好设置
/// </summary>
public class PlayerPreference : NormalSingleton<PlayerPreference>
{
    private string MoviePrefix = "MOVIE_";
    //private QualityTag? quality = null;
    private bool IsFirstTimeLogin = false;

    private static bool IsNotSynchroServer = true;

    public static bool SynchroServer
    {
        get { return IsNotSynchroServer; }
        set { IsNotSynchroServer = value; }
    }

    /// <summary>
    /// 如果要改 获取服务器列表专用 其它地方不要用 请通知一下wending
    /// </summary>
    public bool FirstTimeLogin
    {
        get
        {
            if (PlayerPrefs.GetInt("FirstTimeLogin", 1000) == 1000)
            {
                IsFirstTimeLogin = true;
                PlayerPrefs.SetInt("FirstTimeLogin", 1);
            }
            else
            {
                IsFirstTimeLogin = false;
            }
            return IsFirstTimeLogin;
        }
    }

    /// <summary>
    /// 函数比较特特殊,不要乱用, 主要用于测试服转正式服用的
    /// </summary>
    /// <returns></returns>
    public void SetIsFirstLogin()
    {
        PlayerPrefs.DeleteKey("FirstTimeLogin");
    }

    public bool MusicOn
    {
        get
        {
            return PlayerPrefs.GetInt("MusicOnOff", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("MusicOnOff", value ? 1 : 0);
            Messenger.Instance.BroadCastEventMsg(MessageEventType.OnChangePlaylistToggle, value);
        }
    }

    /// <summary>
    /// 自动喝红药
    /// </summary>
    public float AutoCureHP
    {
        get
        {
            return PlayerPrefs.GetFloat("AutoCureHP",0.0f);
        }
        set
        {
            PlayerPrefs.SetFloat("AutoCureHP", value);
        }
    }
    public bool AutoCureOnOff
    {
        get
        {
            return PlayerPrefs.GetInt("AutoCureOnOff", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoCureOnOff", value ? 1 : 0);
        }
    }
    /// <summary>
    /// 自动喝蓝药
    /// </summary>
    public float AutoCureMP
    {
        get
        {
            return PlayerPrefs.GetFloat("AutoCureMP", 0.0f);
        }
        set
        {
            PlayerPrefs.SetFloat("AutoCureMP", value);
        }
    }
    public bool AutoCureMPOnOff
    {
        get
        {
            return PlayerPrefs.GetInt("AutoCureMPOnOff", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoCureMPOnOff", value ? 1 : 0);
        }
    }
    /// <summary>
    /// 自动拾取金币
    /// </summary>
    public bool AutoPickMoney
    {
        get
        {
            return PlayerPrefs.GetInt("AutoPickMoney", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoPickMoney", value ? 1 : 0);
        }
    }
    /// <summary>
    /// 自动回城
    /// </summary>
    public float AutoBackCityHP
    {
        get
        {
            return PlayerPrefs.GetFloat("AutoBackCityHP", 0.0f);
        }
        set
        {
            PlayerPrefs.SetFloat("AutoBackCityHP", value);
        }
    }
    public bool AutoBackCityOnOff
    {
        get
        {
            return PlayerPrefs.GetInt("AutoBackCityOnOff", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoBackCityOnOff", value ? 1 : 0);
        }
    }
    /// <summary>
    /// 自动随机
    /// </summary>
    public float AutoRandomFlyHP
    {
        get
        {
            return PlayerPrefs.GetFloat("AutoRandomFlyHP", 0.0f);
        }
        set
        {
            PlayerPrefs.SetFloat("AutoRandomFlyHP", value);
        }
    }
    public bool AutoRandomFlyOnOff
    {
        get
        {
            return PlayerPrefs.GetInt("AutoRandomFlyOnOff", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoRandomFlyOnOff", value ? 1 : 0);
        }
    }
    /// <summary>
    /// 自动拾取药品
    /// </summary>
    public bool AutoPickMedicine
    {
        get
        {
            return PlayerPrefs.GetInt("AutoPickMedicine", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoPickMedicine", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动屏蔽玩家
    /// </summary>
    public bool AutoShieldPlayer
    {
        get
        {
            return PlayerPrefs.GetInt("AutoShieldPlayer", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoShieldPlayer", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动拒绝组队邀请
    /// </summary>
    public bool AutoRefuseTeamInvite
    {
        get
        {
            return PlayerPrefs.GetInt("AutoRefuseTeamInvite", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoRefuseTeamInvite", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动屏蔽战神(玩家宠物)
    /// </summary>
    public bool AutoShieldMars
    {
        get
        {
            return PlayerPrefs.GetInt("AutoShieldMars", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoShieldMars", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动屏蔽玩家称号
    /// </summary>
    public bool AutoShieldTitle
    {
        get
        {
            return PlayerPrefs.GetInt("AutoShieldTitle", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoShieldTitle", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动屏蔽玩家披风
    /// </summary>
    public bool AutoShieldCloak
    {
        get
        {
            return PlayerPrefs.GetInt("AutoShieldCloak", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoShieldCloak", value ? 1 : 0);
        }
    }

    /// <summary>
    /// 自动屏蔽怪物
    /// </summary>
    public bool AutoShieldMonster
    {
        get
        {
            return PlayerPrefs.GetInt("AutoShieldMonster", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("AutoShieldMonster", value ? 1 : 0);
        }
    }

    public float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicVolume", 1);
        }

        set
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
            //MasterAudioVolumeController.Instance.PlaylistVolumeDuckRatio = value;
        }
    }

    public bool MusicEffectOnOff
    {
        get
        {
            return PlayerPrefs.GetInt("MusicEffectOnOff", 1) == 1;
        }

        set
        {
            PlayerPrefs.SetInt("MusicEffectOnOff", value ? 1 : 0);
        }
    }

    public float MusicEffectVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("MusicEffectVolume", 1.0f);
        }

        set
        {
            PlayerPrefs.SetFloat("MusicEffectVolume", value);
        }
    }

    public bool ShowElementDamage
    {
        get
        {
            return PlayerPrefs.GetInt("ShowElementDamage", 1) == 1;
        }

        set
        {
            PlayerPrefs.SetInt("ShowElementDamage", value ? 1 : 0);
        }
    }

    public int MaxShowPlayerNum
    {
        get
        {
            return PlayerPrefs.GetInt("MaxShowPlayerNum", 20);
        }

        set
        {
            PlayerPrefs.SetInt("MaxShowPlayerNum", value);
        }
    }

    //private static string settingPath = Application.persistentDataPath + "//PlayerSetting.txt";
    private GameQualityTag m_Qulity = GameQualityTag.High;

    public GameQualityTag QulitySetting
    {
        get {
            return  (GameQualityTag)PlayerPrefs.GetInt("EffectQuality", (int)GameQualityTag.High);
        }
        set
        {
            PlayerPrefs.SetInt("EffectQuality", (int)value);
            m_Qulity = value;
            int qLevel = UnityEngine.QualitySettings.GetQualityLevel();
            int changeLevel = qLevel;
            if (m_Qulity == GameQualityTag.High)
            {
                changeLevel = 3;// Good
            }
            else if (m_Qulity == GameQualityTag.Medium)
            {
                changeLevel = 2;// Simple
            }
            else
                changeLevel = 1;// Fast
            UnityEngine.QualitySettings.SetQualityLevel(changeLevel);
        }
    }

    public bool IsOpenOrCloseShadow
    {
        get;
        set;
    }

    //public QualityTag Quality
    //{
    //    get
    //    {
    //        if (quality == null)
    //        {
    //            if (PlayerPrefs.HasKey("Quality") == false)
    //            {
    //                quality = QualityManager.Instance.GetDeviceQuality();
    //            }
    //            else
    //            {
    //                quality = (QualityTag)PlayerPrefs.GetInt("Quality", (int)QualityTag.HIGH);
    //            }
    //        }

    //        return quality.Value;
    //    }	

    //    set
    //    {
    //        quality = value;
    //        PlayerPrefs.SetInt("Quality", (int)value);
    //    }
    //}

    //public bool UltraQuality
    //{
    //    get
    //    {
    //        return (Quality == QualityTag.ULTRA);
    //    }	
    //}

    //public bool HighQuality
    //{
    //    get
    //    {
    //        return (Quality == QualityTag.HIGH);
    //    }	
    //}

    //public bool MediumQuality
    //{
    //    get
    //    {
    //        return (Quality == QualityTag.MEDIUM);
    //    }	
    //}

    //public bool LowQuality
    //{
    //    get
    //    {
    //        return (Quality == QualityTag.LOW);
    //    }	
    //}

    public string theAccountName
    {
        get
        {
            return PlayerPrefs.GetString("theAccountName");
        }

        set
        {
            PlayerPrefs.SetString("theAccountName", value);
        }
    }

    public string thePassword
    {
        get
        {
            return PlayerPrefs.GetString("thePassword");
        }

        set
        {
            PlayerPrefs.SetString("thePassword", value);
        }
    }

    public string theQID
    {
        get
        {
            return PlayerPrefs.GetString("theQID");
        }

        set
        {
            PlayerPrefs.SetString("theQID", value);
        }
    }

    public string theTID
    {
        get
        {
            return PlayerPrefs.GetString("theTID");
        }

        set
        {
            PlayerPrefs.SetString("theTID", value);
        }
    }

    public string thePF
    {
        get
        {
            return PlayerPrefs.GetString("thePF");
        }

        set
        {
            PlayerPrefs.SetString("thePF", value);
        }
    }

    public string theIP
    {
        get
        {
            return PlayerPrefs.GetString("theIP");
        }

        set
        {
            PlayerPrefs.SetString("theIP", value);
        }
    }

    public string thePort
    {
        get
        {
            return PlayerPrefs.GetString("thePort");
        }

        set
        {
            PlayerPrefs.SetString("thePort", value);
        }
    }

    public string thePFKey
    {
        get
        {
            return PlayerPrefs.GetString("thePFKey");
        }

        set
        {
            PlayerPrefs.SetString("thePFKey", value);
        }
    }

    public bool getMoviePlayed(string moviePath)
    {
        bool b = PlayerPrefs.GetInt(MoviePrefix + moviePath, 0) > 0 ? true : false;
        return b;
    }

    public void setMoviePlayed(string moviePath)
    {
        PlayerPrefs.SetInt(MoviePrefix + moviePath, 1);
    }

    /// <summary>
    /// 镜头特写数据：保存已经展示过的npcid
    /// </summary>
    /// 
    public string getNpcShowMovieData(string playerName)
    {
        return PlayerPrefs.GetString("NpcMovieData_" + playerName, "");
    }
    public void setNpcShowMovieData(string playName, string value)
    {
        PlayerPrefs.SetString("NpcMovieData_" + playName, value);
    }


    /// <summary>
    /// 是否加载CSV文件
    /// </summary>
    public bool IsLoadCsv
    {
        get
        {
            return (PlayerPrefs.GetInt("GameDB_CSV", 1) == 1);
        }
        set
        {
            if (value)
                PlayerPrefs.SetInt("GameDB_CSV", 1);
            else
                PlayerPrefs.SetInt("GameDB_CSV", 0);
        }
    }

    //public bool getBtnState(int _btnID)
    //{
    //    string _str = UserInfo.Instance.m_dwUserOnlyId.ToString() + UserInfo.Instance.m_nCurSelHeroBtnIndex + _btnID + "0";
    //    return PlayerPrefs.GetInt(_str, 0) > 0 ? false : true;
    //}

    //public void setBtnState(int _btnID, bool _bIsStart)
    //{
    //    string _str = UserInfo.Instance.m_dwUserOnlyId.ToString() + UserInfo.Instance.m_nCurSelHeroBtnIndex + _btnID + "0";
    //    PlayerPrefs.SetInt(_str, _bIsStart ? 1 : 0);
    //}

    //public bool getBtnState2(int _btnID)
    //{
    //    string _str = UserInfo.Instance.m_dwUserOnlyId.ToString() + UserInfo.Instance.m_nCurSelHeroBtnIndex + _btnID + "1";
    //    return PlayerPrefs.GetInt(_str, 0) > 0 ? false : true;
    //}

    //public void setBtnState2(int _btnID, bool _bIsStart)
    //{
    //    string _str = UserInfo.Instance.m_dwUserOnlyId.ToString() + UserInfo.Instance.m_nCurSelHeroBtnIndex + _btnID + "1";
    //    PlayerPrefs.SetInt(_str, _bIsStart ? 1 : 0);
    //}

    //wending
    /// <summary>
    /// 所有区是否更新要使用该值
    /// </summary>
    public string UserServerListVer
    {
        get
        {
            return PlayerPrefs.GetString("WDUserServerListVer");
        }
        set
        {
            PlayerPrefs.SetString("WDUserServerListVer", value);
        }
    }

    /// <summary>
    /// 上次登录的区更新要使用该值 和 UserServerListVer 区别是, 
    /// UserServerListVer 只有拉全区表才会比对 保存档
    /// UserServerListItemVer 一更新区item 就会改成当前抓的版的值
    /// </summary>
    public string UserServerListItemVer
    {
        get
        {
            return PlayerPrefs.GetString("WDUserServerListItemVer");
        }
        set
        {
            PlayerPrefs.SetString("WDUserServerListItemVer", value);
        }
    }

    ///// <summary>
    ///// 保存登录的列表
    ///// </summary>
    //public ServerItemInfo UserSeverItem
    //{
    //    set
    //    {
    //        PlayerPrefs.SetString("WDServerTrueZoneID" + value.id, value.truezoneid);
    //        PlayerPrefs.SetString("WDServerCDN" + value.id, value.cdn);
    //        PlayerPrefs.SetString("WDServerIP" + value.id, value.ip);
    //        PlayerPrefs.SetString("WDServerPort" + value.id, value.port);
    //        PlayerPrefs.SetString("WDServerTitle" + value.id, WWW.EscapeURL(value.title));
    //    }
    //}

    public void DeleLastUsedServerIDKey()
    {
        PlayerPrefs.DeleteKey("WDLastUsedServerID");
    }

    public void DelePreLastUsedServerIDKey()
    {
        PlayerPrefs.DeleteKey("WDPreLastUsedServerID");
    }

    public void DelePrePreLastUsedServerIDKey()
    {
        PlayerPrefs.DeleteKey("WDPrePreLastUsedServerID");
    }

    /// <summary>
    /// 上次使用的服务器id
    /// </summary>
    public string LastUsedServerID
    {
        get
        {
            return PlayerPrefs.GetString("WDLastUsedServerID", "");
        }
        set
        {
            PlayerPrefs.SetString("WDLastUsedServerID", value);
        }
    }

    /// <summary>
    /// 上上次使用的服务器id
    /// </summary>
    public string PreLastUsedServerID
    {
        get
        {
            return PlayerPrefs.GetString("WDPreLastUsedServerID", "");
        }
        set
        {
            PlayerPrefs.SetString("WDPreLastUsedServerID", value);
        }
    }

    /// <summary>
    /// 上上上次使用的服务器id 
    /// </summary>
    public string PrePreLastUsedServerID
    {
        get
        {
            return PlayerPrefs.GetString("WDPrePreLastUsedServerID", "");
        }
        set
        {
            PlayerPrefs.SetString("WDPrePreLastUsedServerID", value);
        }
    }

    //    /// <summary>
    //    /// 当要修改字典中的一条 用该属性来修改
    //    /// </summary>
    //    public ServerItemInfo SaveItemOne
    //    {
    //        set 
    //        {
    //            if (null == value)
    //            {
    //                return;
    //            }
    //            Dictionary<string, ServerItemInfo> ItemDictionary = PlayerPreference.Instance.UseSeverItemDictionary;

    //            PlayerPrefs.SetString("WDServerTrueZoneID" + value.id, value.truezoneid);
    //            PlayerPrefs.SetString("WDServerCDN" + value.id, value.cdn);
    //            PlayerPrefs.SetString("WDServerIP" + value.id, value.ip);
    //            PlayerPrefs.SetString("WDServerPort" + value.id, value.port);
    //            PlayerPrefs.SetString("WDServerTitle" + value.id, WWW.EscapeURL(value.title));

    //            if (null != ItemDictionary && ItemDictionary.Count > 0)
    //            {
    //                if (!ItemDictionary.ContainsKey(value.id))
    //                {
    //      //              QKNGUITools.Instance.AddShow("要存的不在字典中");
    //                    int nCount = PlayerPrefs.GetInt("WDServerItemCount", 0);
    //                    PlayerPrefs.SetInt("WDServerItemCount", (nCount + 1));
    //                    PlayerPrefs.SetString(nCount.ToString() + "WDServerItemID", value.id);
    ////                    QKNGUITools.Instance.AddShow(nCount.ToString());
    //                }
    //            }
    //            else
    //            {
    //                PlayerPrefs.SetInt("WDServerItemCount", 1);
    //                PlayerPrefs.SetString("0" + "WDServerItemID", value.id);
    //            }
    //        }
    //}


    //private static void ClearSaverItem()
    //{
    //    Dictionary<string, ServerItemInfo> ItemDictionary = PlayerPreference.Instance.UseSeverItemDictionary;
    //    if (null == ItemDictionary || ItemDictionary.Count < 1)
    //    {
    //        PlayerPrefs.SetInt("WDServerItemCount", 0);
    //        return;
    //    }

    //    int i = 0;
    //    foreach (KeyValuePair<string, ServerItemInfo> theItem in ItemDictionary)
    //    {
    //        PlayerPrefs.DeleteKey("WDServerTrueZoneID" + theItem.Key);
    //        PlayerPrefs.DeleteKey("WDServerCDN" + theItem.Key);
    //        PlayerPrefs.DeleteKey("WDServerIP" + theItem.Key);
    //        PlayerPrefs.DeleteKey("WDServerPort" + theItem.Key);
    //        PlayerPrefs.DeleteKey("WDServerTitle" + theItem.Key);
    //        PlayerPrefs.DeleteKey(i.ToString() + "WDServerItemID");
    //        ++i;
    //    }
    //    PlayerPrefs.SetInt("WDServerItemCount", 0);
    //}

    ///// <summary>
    ///// 所有服务器列表
    ///// </summary>
    //public Dictionary<string, ServerItemInfo> UseSeverItemDictionary
    //{
    //    get
    //    {
    //        int nCount = PlayerPrefs.GetInt("WDServerItemCount", 0);
    //        //QKNGUITools.Instance.AddShow("字典中的数量" + nCount.ToString());
    //        if (nCount > 0)
    //        {
    //            Dictionary<string, ServerItemInfo> ItemDictionary = new Dictionary<string, ServerItemInfo>();
    //            for (int i = 0; i < nCount; ++i)
    //            {
    //                ServerItemInfo info = new ServerItemInfo();
    //                info.id = PlayerPrefs.GetString(i.ToString() + "WDServerItemID", "");
    //                info.truezoneid = PlayerPrefs.GetString("WDServerTrueZoneID" + info.id, "");
    //                info.cdn = PlayerPrefs.GetString("WDServerCDN" + info.id, "");
    //                info.ip = PlayerPrefs.GetString("WDServerIP" + info.id, "");
    //                info.port = PlayerPrefs.GetString("WDServerPort" + info.id, "");
    //                info.title = WWW.UnEscapeURL(PlayerPrefs.GetString("WDServerTitle" + info.id, ""));
    //                ItemDictionary[info.id] = info;

    //            }
    //            return ItemDictionary;
    //        }
    //        return null;
    //    }
    //    set
    //    {

    //        if (null != value && value.Count > 0)
    //        {
    //            //QKNGUITools.Instance.AddShow("设值~字典中的数量" + value.Count.ToString());
    //            PlayerPrefs.SetInt("WDServerItemCount", value.Count);

    //            int i = 0;
    //            foreach (KeyValuePair<string, ServerItemInfo> theItem in value)
    //            {
    //                PlayerPrefs.SetString("WDServerTrueZoneID" + theItem.Key, theItem.Value.truezoneid);
    //                PlayerPrefs.SetString("WDServerCDN" + theItem.Key, theItem.Value.cdn);
    //                PlayerPrefs.SetString("WDServerIP" + theItem.Key, theItem.Value.ip);
    //                PlayerPrefs.SetString("WDServerPort" + theItem.Key, theItem.Value.port);
    //                PlayerPrefs.SetString("WDServerTitle" + theItem.Key, WWW.EscapeURL(theItem.Value.title));
    //                PlayerPrefs.SetString(i.ToString() + "WDServerItemID", theItem.Key);
    //                ++i;
    //            }
    //        }
    //        else
    //        {
    //            //QKNGUITools.Instance.AddShow("ClearSaverItem~");
    //            ClearSaverItem();
    //        }
    //    }
    //}

    //1 QQ, 2 威信
    public int LastLoginType
    {
        get
        {
            return PlayerPrefs.GetInt("WDThirdLastLoginType", -1);
        }
        set
        {
            PlayerPrefs.SetInt("WDThirdLastLoginType", value);
        }
    }

    /// <summary>
    /// 登录游戏后有返回角色界面,要用到,其它不要用
    /// </summary>
    public string strCurUsePort
    {
        get
        {
            return PlayerPrefs.GetString("WDThirdLastLoginCurPort", "");
        }
        set
        {
            PlayerPrefs.SetString("WDThirdLastLoginCurPort", value);
        }
    }

    //wending end
    public bool lastLoginIsTestServer
    {
        get
        {
            return (PlayerPrefs.GetInt("WDLastLoginIsTestServer", 0) == 1);
        }
        set
        {
            PlayerPrefs.SetInt("WDLastLoginIsTestServer", value ? 1 : 0);
        }
    }

    public string lastLoginPakageVer
    {
        get
        {
            return (PlayerPrefs.GetString("WDLastLoginPakageVer", ""));
        }
        set
        {
            PlayerPrefs.SetString("WDLastLoginPakageVer", value);
        }
    }
}