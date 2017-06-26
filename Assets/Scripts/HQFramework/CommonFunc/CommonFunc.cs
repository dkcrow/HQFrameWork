using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using LumenWorks.Framework.IO.Csv;
using MobileBaseCommon.UnityBase;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HQFrameWork
{
    public static class CommonFunc
    {
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static string getNowDateString()
        {
            DateTime date = DateTime.Now;
            string year = (date.Year - 2000).ToString();
            string mon;
            if (date.Month < 10)
                mon = "0" + date.Month.ToString();
            else
                mon = date.Month.ToString();
            string day;
            if (date.Day < 10)
                day = "0" + date.Day.ToString();
            else
                day = date.Day.ToString();
            string hour;
            if (date.Hour < 10)
                hour = "0" + date.Hour.ToString();
            else
                hour = date.Hour.ToString();
            string min;
            if (date.Minute < 10)
                min = "0" + date.Minute.ToString();
            else
                min = date.Minute.ToString();
            return year + mon + day + "-" + hour + min;
        }



        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attribs =
                fieldInfo.GetCustomAttributes(typeof (StringValueAttribute), false) as StringValueAttribute[];
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }



        /// <summary>
        /// 获取该坐标对应navmesh上的点
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 toNavMeshPoint(this Vector3 vec)
        {
            RaycastHit hit;
            Ray ray = new Ray(new Vector3(vec.x, 500, vec.z), Vector3.down);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                vec.y = hit.point.y;
            }
            UnityEngine.AI.NavMeshHit nHit;
            if (UnityEngine.AI.NavMesh.SamplePosition(vec, out nHit, 100f, UnityEngine.AI.NavMesh.AllAreas))
            {
                vec = nHit.position;
            }
            return vec;
        }

        /// <summary>
        /// 单个CSV直接转数据类
        /// 根据CSV标签读取的  千万不要多个字段用同一个标签
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static T toData<T>(this CsvReader csv) where T : new()
        {
            T ret = new T();
            return csv.toData(ret);
        }

        public static T toData<T>(this CsvReader csv, T data)
        {
            T ret = data;
            MemberInfo[] members = ret.GetType().GetMembers();
            foreach (MemberInfo member in members)
            {
                CsvTitleAttribute[] attribs = member.GetCustomAttributes(typeof(CsvTitleAttribute), false) as CsvTitleAttribute[];
                if (attribs.Length == 0)
                {
                    continue;
                }
                for (int i = 0; i < attribs[0].CsvTitle.Length; i++)
                {
                    int index = csv.GetFieldIndex(attribs[0].CsvTitle[i]);
                    if (index >= 0)
                    {
                        string text = csv[index];
                        object value = text;
                        Type memberType;
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                memberType = (member as FieldInfo).FieldType;
                                break;

                            case MemberTypes.Property:
                                memberType = (member as PropertyInfo).PropertyType;
                                break;

                            default:
                                continue;
                        }
                        //toSafe越界时会报错 所以注意不要越界
                        if (memberType == typeof(byte))
                        {
                            value = text.ToSafeByte();
                        }
                        else
                        if (memberType == typeof(short))
                        {
                            value = text.ToSafeShort();
                        }
                        else
                        if (memberType == typeof(ushort))
                        {
                            value = text.ToSafeUshort();
                        }
                        else
                        if (memberType == typeof(int))
                        {
                            value = text.ToSafeInt32();
                        }
                        else
                        if (memberType == typeof(uint))
                        {
                            value = text.ToSafeUint32();
                        }
                        else
                        if (memberType == typeof(long))
                        {
                            value = text.ToSafeInt64();
                        }
                        else
                        if (memberType == typeof(ulong))
                        {
                            value = text.ToSafeUInt64();
                        }
                        else
                        if (memberType == typeof(float))
                        {
                            value = text.ToSafeFloat();
                        }
                        else
                        if (memberType == typeof(string))
                        {
                            value = text.ToSafeString();
                        }
                        else
                        if (memberType == typeof(bool))
                        {
                            value = text.ToSafeBool();
                        }
                        else
                        if (memberType.BaseType == typeof(Enum))
                        {
                            value = Enum.ToObject(memberType, text.ToSafeInt32());
                        }
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                (member as FieldInfo).SetValue(ret, value);
                                break;

                            case MemberTypes.Property:
                                (member as PropertyInfo).SetValue(ret, value, null);
                                break;
                        }
                    }
                }
            }
            return ret;
        }


    }


}

