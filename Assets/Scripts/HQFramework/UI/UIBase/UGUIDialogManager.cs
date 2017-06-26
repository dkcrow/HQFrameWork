
namespace HQFrameWork
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;
    using UnityEngine.Events;

    public static class UGUIPath
    {
        public const string strPathUIPrefab = "ResourcesAB/Prefabs/UI/";
        public const string strLocalUIPrefab = "Local/Prefabs/UI/";
    }

    /// <summary>
    /// 统管所有ui
    /// </summary>
    public class UGUIDialogManager : MBSingleton<UGUIDialogManager>
    {
        private Dictionary<System.Type, UnityEngine.Object> m_instanceDIc = new Dictionary<System.Type, UnityEngine.Object>();
        public bool m_bIsUseLog = true;

       
        /// <summary>
        /// 获取对话框
        /// </summary>
        /// <param name="dlgName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public UGUIDialogBase GetDialog(string dlgName, LoadOptions options = LoadOptions.AutoPath | LoadOptions.ManagedRes)
        {
            UGUIDialogBase dlgTemp = null;
           
            {
                string path = UGUIPath.strPathUIPrefab;
                if ((options & LoadOptions.LocalPath) == LoadOptions.LocalPath)
                {
                    path = UGUIPath.strLocalUIPrefab;
                }
                ResManager.Instance.Load<GameObject>(path + dlgName + ".prefab", (obj, args) => {
                    if (null != obj)
                    {
                        GameObject gmObj = GameObject.Instantiate(obj) as GameObject;
                        dlgTemp = gmObj.AddComponent<UGUIDialogBase>();
                        dlgTemp.dlgName = obj.name;
                        dlgTemp.RectTrans.SetParent(this.GetInstance<UIRoot>().gmobjCanvas2D.transform, false);//uiroot 路径
                        dlgTemp.Init(gmObj);
                        
                    }
                }, options);
            }
            return dlgTemp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isCommonDialog">通常dialog就挂在Dialog下</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public UGUIMonoBehaviour GetInstance(Type type,bool isCommonDialog=true, LoadOptions options = LoadOptions.AutoPath | LoadOptions.ManagedRes)
        {
            if (m_instanceDIc.ContainsKey(type))
            {
                return m_instanceDIc[type] as UGUIMonoBehaviour;
            }
            else
            {
                UGUIDialogBase instance = null;
                string path = UGUIPath.strPathUIPrefab;
                if ((options & LoadOptions.LocalPath) == LoadOptions.LocalPath)
                {
                    path = UGUIPath.strLocalUIPrefab;
                }
                if (typeof(UGUIDialogBase).IsAssignableFrom(type))
                {
                    ResManager.Instance.Load<GameObject>(path + type.FullName + ".prefab", (obj, args) => {
                        if (null != obj)
                        {
                            GameObject gmObj = GameObject.Instantiate(obj) as GameObject;
                            instance = gmObj.AddComponent(type) as UGUIDialogBase;
                            instance.dlgName = obj.name;
                            gmObj.GetRectTransform().SetParent((isCommonDialog? Instance.GetInstance<UIRoot>().DialogRoot:Instance.GetInstance<UIRoot>().gmobjCanvas2D).GetRectTransform(), false);
                            instance.Init(gmObj);
                        }
                    }, options);
#if UNITY_EDITOR
                    if (null == type)
                    {
                        Debug.Log("资源没加载到! 检查资源是否存在, 如果存在是否是名子空间问题,对话框类不要有名子空间!," + type.FullName);
                    }
#endif
                    return instance;
                }
                else
                {
                    GameObject go = new GameObject();
                    instance = go.AddComponent(type) as UGUIDialogBase;
                    instance.Init(go);
                    return instance;
                }
            }
        }
        /// 是获取的类必须是继承 QKUGUIMonoBehaviour
        /// 如果是对话框在没有加载的情况这里直接得到实例是同步的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>(bool isCommonDialog = true,LoadOptions options = LoadOptions.AutoPath | LoadOptions.ManagedRes) where T : UGUIMonoBehaviour
        {
            System.Type type = typeof(T);
            UGUIMonoBehaviour mono = GetInstance(type, isCommonDialog, options);
            if (mono != null)
            {
                return mono as T;
            }
            else
            {
                return default(T);
            }
        }

        public void RemoveInstance<T>()
        {
            if (m_instanceDIc.ContainsKey(typeof(T)))
            {
                m_instanceDIc.Remove(typeof(T));
            }
        }
       
        /// <summary>
        /// 是否存在UGUI界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ContainsInstance<T>()
        {
            return ContainsInstance(typeof(T));
        }

        /// <summary>
        /// 是否存在UGUI界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool ContainsInstance(Type t)
        {
            return m_instanceDIc.ContainsKey(t);
        }

        /// <summary>
        /// 销毁或者隐藏所有对话框,过场景时候调用
        /// </summary>
        public void OnChangeScene()
        {
            List<System.Type> listTemp = new List<System.Type>();
            foreach (var item in m_instanceDIc)
            {
                if (typeof(UGUIDialogBase).IsAssignableFrom(item.Key))
                {
                    UGUIDialogBase dlg = ((UGUIDialogBase)item.Value);
                    if (null != dlg) dlg.OnChangeScene();
                    listTemp.Add(item.Key);
                }
            }
            for (int i = 0; i < listTemp.Count; i++)
            {
                if (null == m_instanceDIc[listTemp[i]])     //清除已经销毁的对话框
                {
                    m_instanceDIc.Remove(listTemp[i]);
                    ResManager.Instance.UnLoadSingleRes(UGUIPath.strPathUIPrefab + listTemp[i].FullName + ".prefab");
                    // StartCoroutine(QKResourceManager.Instance.UnloadBundleInternal(UGUIPath.strPathUIPrefab + listTemp[i].FullName + ".prefab"));
                }
            }       
          
        }

        /// <summary>
        /// 清除所有UI界面显示的数据
        /// </summary>
        public void ClearAllUI()
        {
            foreach (var item in m_instanceDIc)
            {
                if (typeof(UGUIDialogBase).IsAssignableFrom(item.Key))
                {
                    UGUIDialogBase dlg = ((UGUIDialogBase)item.Value);
                    if (null != dlg) dlg.ClearUI();
                }
            }
        }
        public void AddInstance(UGUIMonoBehaviour instance)
        {
            m_instanceDIc[instance.GetType()] = instance;
        }

        public bool IsContainsDialog(System.Type type)
        {
            return m_instanceDIc.ContainsKey(type);
        }

        /// <summary>
        /// 判断界面是否可见
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool DialogVisible<T>()
        {
            System.Type type = typeof(T);
            if (m_instanceDIc.ContainsKey(type))
            {
                return (m_instanceDIc[type] as UGUIDialogBase).isActiveAndEnabled;
            }

            return false;
        }
    }
}