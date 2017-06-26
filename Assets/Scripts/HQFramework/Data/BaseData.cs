﻿using System;
using System.Collections.Generic;

//=====================================================================================/
///<summary>
///wending
///数据类基类,只有一份
///<summary>
//=====================================================================================/
namespace HQFrameWork
{
    public enum DataType
    {
        TestData,

        PlayerData,

        Task,

        DataPet,

        DataEvent,

        EntityData,

    }

    public abstract class BaseData
    {
        private static Dictionary<Type, BaseData> s_uiDataByInstanceTypeDic = new Dictionary<Type, BaseData>();

        protected BaseData()
        {
            InitData();
        }

        /// <summary>
        /// 需要初始化的数据，如本地读表数据等可以在这里初始化
        /// </summary>
        public virtual void InitData()
        {
        }

        public static T DataConvert<T>(BaseData data) where T : class
        {
            T tDate = data as T;
            if (tDate == null)
                throw new ArgumentException(string.Format("类型转换失败:源类型: {0} 目标类型:{1}", data.GetType(), typeof(T)));
            return tDate;
        }

        private static bool ContainsKey(Type type)
        {
            return s_uiDataByInstanceTypeDic.ContainsKey(type);
        }

        private static void AddData(BaseData data)
        {
            s_uiDataByInstanceTypeDic[data.GetType()] = data;
        }
        /// <summary>
        /// 给lua调取，jyj
        /// </summary>
        /// <param name="dataName"></param>
        /// <returns></returns>
        public static BaseData GetData(string dataName)
        {
            Type t = Type.GetType(dataName);
            if (!s_uiDataByInstanceTypeDic.ContainsKey(t))
            {
                s_uiDataByInstanceTypeDic[t] = Activator.CreateInstance(t) as BaseData;
            }
            return s_uiDataByInstanceTypeDic[t];
        }
        public static T GetData<T>() where T : BaseData, new()
        {
            T data = null;
            Type type = typeof(T);
            if (ContainsKey(type))
            {
                data = s_uiDataByInstanceTypeDic[type] as T;
            }
            else
            {
                data = new T();//这里new的时候会调用initdata
                s_uiDataByInstanceTypeDic[type] = data;
            }
            return data;
        }

        public static void ClearAllData()
        {
            foreach (var item in s_uiDataByInstanceTypeDic)
            {
                item.Value.ClearData();
            }
        }

        public abstract DataType dataType
        {
            get;
        }

        public virtual void ClearData()
        {
        }

     

    }
}