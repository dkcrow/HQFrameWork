//using LumenWorks.Framework.IO.Csv;
//using MobileBaseCommon.UnityBase;
//using QK;
//using QKResource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HQFrameWork;
using LumenWorks.Framework.IO.Csv;
using UnityEngine;

/// <summary>
/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// !!!!!!!!!!!!UNITY不支持BOM头!!!!!!!!
/// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// </summary>
public class GameDBLoadingParam
{
    public delegate void LoadingBytesFinishedDelegate(string fileName, byte[] buff);

    public LoadingBytesFinishedDelegate finishDelegate = null;
    public string path = "";

    public GameDBLoadingParam(string _path, LoadingBytesFinishedDelegate _finishDelegate)
    {
        path = _path;
        finishDelegate = _finishDelegate;
    }
}

public class FileConfig
{
    public string fileName;
    public ResDelegate.LoadedDone<byte[]> callBack;


    public FileConfig(string fileName, ResDelegate.LoadedDone<byte[]> callBack)
    {
        this.fileName = fileName;
        this.callBack = callBack;
    }
}

public class GameDBConfig
{
    private int MAXCSVFile = 0;
    private int loadDoneCSVAmount = 0;
    private List<GameDBLoadingParam> loadingList = new List<GameDBLoadingParam>();
    private int CSV_COUNT = 0;
    private List<FileConfig> listLoadFile = new List<FileConfig>();
#if UNITY_EDITOR
    private string strAssetBundlePath = "file://" + Application.streamingAssetsPath + "/csvData/GameDBAsset/";
#else
    private string strAssetBundlePath = Application.streamingAssetsPath + "/csvData/GameDBAsset/";
#endif
    private static GameDBConfig instance;

    public static GameDBConfig Instance
    {
        get
        {
            if (null == instance)
                instance = new GameDBConfig();
            return instance;
        }
    }

    private GameDBConfig()
    {
    }

    public bool NormalMode
    {
        get { return true; }
    }



    /// <summary>
    /// Sound表
    /// </summary>
    public Dictionary<string, UISoundInfo> m_dicUISound = new Dictionary<string, UISoundInfo>();

    /// <summary>
    /// Effect表
    /// </summary>
    public Dictionary<uint, AudioData> m_dicAudioData = new Dictionary<uint, AudioData>();

    /// <summary>
    /// 人物属性表
    /// </summary>
    public Dictionary<int, PlayerData> m_playerDataDic = new Dictionary<int, PlayerData>();

    /// <summary>
    /// 实体表 服务器下发typeid(表里的第一列,从dic中根据type获取名称,poolmanager根据
    /// </summary>
    public Dictionary<int, EntityData> m_entityDataDic = new Dictionary<int, EntityData>();



    private Action onCallBack;

    private DateTime recordTime;//用于记录时间

    /// <summary>
    /// 初始化 把要加载的文件添加到表中
    /// </summary>
    public void Init(Action callback)
    {
        onCallBack = callback;
        loadDoneCSVAmount = 0;
        loadingList.Clear();
        listLoadFile.Clear();
        if (NormalMode)
        {
            Debug.Log("读取CSV文件 .....");
        }
        else
            Debug.Log("读取Script文件 .....");
        
        //可以在这打印加载时间

        listLoadFile.Add(new FileConfig("player_property_db.csv", LoadPlayerPropertyDone));
        listLoadFile.Add(new FileConfig("Entity_db.csv", LoadBuildingsDone));
        LoadDataFile();
    }

    private void LoadDataFile()
    {
        MAXCSVFile = listLoadFile.Count;
        if (MAXCSVFile <= 0)
        {
            if (null != onCallBack)
            {
                onCallBack();
            }
        }
        for (int i = 0; i < listLoadFile.Count; i++)//把上面add的一次性加载
        {
            ResManager.Instance.LoadFile(listLoadFile[i].fileName, listLoadFile[i].callBack, listLoadFile[i].fileName);
        }
       
    }

   

    #region 音效表

    private void LoadUISound(byte[] bytes, params System.Object[] args)
    {
        MemoryStream stream = new MemoryStream(bytes);
        //csv读取
        //CsvReader reader = new CsvReader(new StreamReader(stream, Encoding.UTF8), true);
        //while (reader.ReadNextRecord())
        //{
        //    UISoundInfo info = reader.toData<UISoundInfo>();
        //    m_dicUISound[info.type] = info;
        //}
        LoadOneCSVDone();
    }

    #endregion 音效表

   

    #region 人物属性

    public void LoadPlayerPropertyDone(byte[] bytes, params System.Object[] args)
    {
        MemoryStream stream = new MemoryStream(bytes);
//        //csv读取
        CsvReader reader = new CsvReader(new StreamReader(stream, Encoding.UTF8), true);
        while (reader.ReadNextRecord())
        {
//            string[] temp=new string[100];
//            reader.CopyCurrentRecordTo(temp);
//            Debug.Log(temp);
            PlayerData data = reader.toData<PlayerData>(); 
            m_playerDataDic[data.Type] = data;
        }
        LoadOneCSVDone();
    }

    #endregion 人物属性


    #region 建筑物表

    public void LoadBuildingsDone(byte[] bytes, params System.Object[] args)
    {
        MemoryStream stream = new MemoryStream(bytes);
        //        //csv读取
        CsvReader reader = new CsvReader(new StreamReader(stream, Encoding.UTF8), true);
        while (reader.ReadNextRecord())
        {
            //            string[] temp=new string[100];
            //            reader.CopyCurrentRecordTo(temp);
            //            Debug.Log(temp);
            EntityData data = reader.toData<EntityData>();
            m_entityDataDic[data.Type] = data;
        }
        LoadOneCSVDone();
    }

    #endregion 建筑物表


    private int completeLoadCount = 0;
    /// <summary>
    /// 一张表读取完毕后统一调用下面的函数
    /// </summary>
    public void LoadOneCSVDone()
    {
        //可以在这里计算加载该csv的时间   
        completeLoadCount++;
        if (completeLoadCount >= listLoadFile.Count)
        {
            //全部表加载完成,回调到initscene里进入下一步的初始化
            if (null != onCallBack)
            {
                onCallBack();
            }
        }
        
                                          
    }
    
}