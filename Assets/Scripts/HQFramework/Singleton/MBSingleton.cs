using UnityEngine;
using System.Collections;

namespace HQFrameWork
{
    public enum SingletonType
    {
        //The singleton is global in game & not destory when scene change.
        GlobalInstance,
        //The singleton is global in game & not destory when scene change.
        //This singleton must use information from data.
        GlobalInstanceDataSpecify,
        //The singleton only use in this scene & will destory when scene change.
        PerSceneInstance,
    }

    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// </summary>
    /// <summary>
    /// MonoBehaviourSingleton base class that will cause any inheriting class to create itself when referenced in any way at all.
    /// 需要继承monobehaviour的单例模式
    /// </summary>
    public class MBSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        private static object _lock = new object();

        private static bool dontDestroyFindObjOnLoad = false; // Instance from FindObjectOfType.
        private static bool dontDestroyNewObjOnLoad = true; // Instance from AddComponent.

        protected SingletonType singletonType
        {
            set
            {
                switch (value)
                {
                    case SingletonType.GlobalInstance:
                        dontDestroyFindObjOnLoad = true;
                        dontDestroyNewObjOnLoad = true;
                        break;

                    case SingletonType.GlobalInstanceDataSpecify:
                        dontDestroyFindObjOnLoad = true;
                        dontDestroyNewObjOnLoad = false;
                        break;

                    case SingletonType.PerSceneInstance:
                        dontDestroyFindObjOnLoad = false;
                        dontDestroyNewObjOnLoad = false;
                        break;
                }
            }
        }

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {

                    return null;
                }

                if (null == _instance)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            Object[] findOjbs = FindObjectsOfType(typeof (T));

                            if (findOjbs.Length >= 1)
                            {
                                _instance = (T) findOjbs[0];

                                if (findOjbs.Length > 1)
                                {

                                    return _instance;
                                }
                            }

                            if (_instance == null)
                            {
                                GameObject singleton = new GameObject();
                                _instance = singleton.AddComponent<T>();
                                singleton.name = "(singleton) " + typeof (T).ToString();

                                if (dontDestroyNewObjOnLoad)
                                {
                                    DontDestroyOnLoad(singleton);
                                }


                            }
                            else
                            {


                                if (dontDestroyFindObjOnLoad)
                                {
                                    DontDestroyOnLoad(_instance.transform.root);
                                }
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        private static bool applicationIsQuitting = false;

        protected static bool ApplicationIsQuitting
        {
            get { return applicationIsQuitting; }
        }

        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }
    }
}
