using UnityEngine;
using HQFrameWork;

  
    public class UIRoot : UGUIMonoBehaviour
{
        public GameObject gmobjCanvas2D;
        public GameObject gmobjCanvas3D;
        public GameObject DialogRoot;
        public Camera Camera3d;
        public Camera Camera2d;
        void Awake()
        {
            base.Init(null);
        }

    }


