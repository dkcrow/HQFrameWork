using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HQFrameWork
{
    public enum StringValueType
    {
        /// <summary>
        /// 没类型 普通
        /// </summary>
        None = 0,

        /// <summary>
        /// 属性
        /// </summary>
        EffPro = 1,

        /// <summary>
        /// 查询玩家属性-基础属性
        /// </summary>
        FirstActorInfo,

        /// <summary>
        /// 查询玩家属性-战斗属性
        /// </summary>
        SecondActorInfo,

        /// <summary>
        /// 查询玩家属性-抗性属性
        /// </summary>
        ThirdActorInfo
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public sealed class StringValueAttribute : Attribute
    {
        public string StringValue { get; private set; }
        public StringValueType type { get; private set; }

        public StringValueAttribute(string value, StringValueType type = StringValueType.None)
        {
            StringValue = value;
            this.type = type;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public sealed class CsvTitleAttribute : Attribute
    {
        public string[] CsvTitle { get; private set; }

        public CsvTitleAttribute(params string[] value)
        {
            CsvTitle = value;
        }
    }

}

