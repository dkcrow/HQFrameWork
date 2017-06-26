
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HQFrameWork
{

    public class HQSceneManager : MBSingleton<HQSceneManager>
    {

        public string m_strCurLoadMainSceneName = ""; //当前要加载的主场景含navmesh的场景(如果有的话) 
        private BaseScene m_CurScene = null;
        private BaseScene m_ToLoadScene;

        void Awake()
        {
            SceneManager.sceneLoaded += (scene,sceneLoadMode) => {
                if (m_ToLoadScene != null)
                {
                    m_CurScene = m_ToLoadScene;//进入场景后才正式赋值
                    m_CurScene.OnEnterScene();//调用前一场景的卸载函数 
                    Messenger.Instance.BroadCastEventMsg(MessageEventType.OnEnterScene);
                }
            };

            SceneManager.sceneUnloaded += (scene) => {
                if (m_CurScene != null)
                {
                    m_CurScene.OnLeaveScene();//调用前一场景的卸载函数 
                }
            };
        }

        void OnDestroy()
        {
//            SceneManager.sceneLoaded -= (scene, sceneLoadMode) => {
//                if (m_CurScene != null)
//                {
//                    m_CurScene.OnEnterScene();//调用前一场景的卸载函数 
//                }
//            };
//
//            SceneManager.sceneUnloaded -= (scene) => {
//                if (m_CurScene != null)
//                {
//                    m_CurScene.OnLeaveScene();//调用前一场景的卸载函数 
//                }
//            };
        }

        
        /// <summary>
        /// 切切换场景 说明 战斗场景和主城全部使用预加载条
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bLoadResource"></param>
        /// <param name="_LevelData"></param>
        public void ChangeScene<T>() where T : BaseScene, new()
        {
           UGUIDialogManager.Instance.OnChangeScene();

            m_ToLoadScene = new T();

            m_strCurLoadMainSceneName = m_ToLoadScene.SceneType.GetStringValue();

            StartCoroutine(UGUIDialogManager.Instance.GetInstance<LoadingDialog>(false,LoadOptions.LocalPath).ShowLoading());//假进度条,等待OnEnterScene消息关闭

           
           SceneManager.LoadSceneAsync(m_strCurLoadMainSceneName);
            
           
          
        }
        

        /// <summary>
        /// 当前场景
        /// </summary>
        public BaseScene CurScene
        {
            get { return m_CurScene; }
        }

      


    }
} 

