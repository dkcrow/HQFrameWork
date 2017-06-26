//=====================================================================================/

//=====================================================================================.
namespace HQFrameWork
{
    using UnityEngine;
    using UnityEditor;
    class UGUIEditorWindow : EditorWindow
    {
        private bool justEnabled;
        public bool Initialized;
        private string errorString;
        private bool bHasError = false;
        private readonly string strPathRoot = "BasePrefabs/";
        private MessageType curMessageType = MessageType.None;

        private Object objDlg = null;
        private Object objImage = null;
        private Object objButton = null;
        private Object objCheckBox = null;
        private Object objDropdown = null;
        private Object objInputField = null;
        private Object objListView = null;
        private Object objPanel = null;
        private Object objRawImage = null;
        private Object objScrollbar = null;
        private Object objSlider = null;
        private Object objText = null;

        //在创建控件时是否自动选中控件
        private bool bAutoSelectCreateCtrl = false;

        /// <summary>
        /// 打开UGUI编辑器
        /// </summary>
        public static void OpenWindow()
        {
            GetWindow<UGUIEditorWindow>();
        }

        private void PreLoadBaseUI()
        {
            objDlg = Resources.Load(strPathRoot + "Dialog");
            objImage = Resources.Load(strPathRoot + "Image");
            objButton = Resources.Load(strPathRoot + "Button");
            objCheckBox = Resources.Load(strPathRoot + "CheckBox");
            objDropdown = Resources.Load(strPathRoot + "Dropdown");
            objInputField = Resources.Load(strPathRoot + "InputField");
            objListView = Resources.Load(strPathRoot + "ListView");
            objPanel = Resources.Load(strPathRoot + "Panel");
            objRawImage = Resources.Load(strPathRoot + "RawImage");
            objScrollbar = Resources.Load(strPathRoot + "Scrollbar");
            objSlider = Resources.Load(strPathRoot + "Slider");
            objText = Resources.Load(strPathRoot + "Text");
        }

        public void InitWindow()
        {
            titleContent.text = "七酷UGUI编辑器";
            //minSize = new Vector2(800, 600);
            PreLoadBaseUI();
            wantsMouseMove = true;
            Initialized = true;
        }

        public void Initialize()
        {
          //  instance = this;
            InitWindow();
        }
        private static void ControlGroup(string title)
        {
            GUILayout.Space(10f);
            GUILayout.Label(title, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Space(5f);
        }
        public  void DoGUI()
        {
            ControlGroup("设置");
            bAutoSelectCreateCtrl = EditorGUILayout.Toggle("在创建新控件自动选中", bAutoSelectCreateCtrl, new GUILayoutOption[0]);

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
       
            // UnityEditor.Selection.activeGameObject
            if (GUILayout.Button(new GUIContent("错误检查", "请选择检查类型"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CheckError();
            }
            if (GUILayout.Button(new GUIContent("清理错误显示", "清除错误显示"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                errorString = "";
            }
            GUILayout.EndHorizontal();
            ControlGroup("控件列表");
            if (GUILayout.Button(new GUIContent("Dialog", "对话框"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.DIALOG);
            }
            if (GUILayout.Button(new GUIContent("Image", "Image"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.IMAGE);
            }
            if (GUILayout.Button(new GUIContent("RawImage", "渲染纹理用"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.RAWIMAGE);
            }
            if (GUILayout.Button(new GUIContent("Button", "按钮"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.BUTTON);
            }
            if (GUILayout.Button(new GUIContent("Panel", "面板容器"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.PANEL);
            }
            if (GUILayout.Button(new GUIContent("Text", "文本"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.TEXT);
            }
            if (GUILayout.Button(new GUIContent("CheckBox", "复选控件"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.CHECKBOX);
            }
            if (GUILayout.Button(new GUIContent("Slider", "Slider"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.SLIDER);
            }
            if (GUILayout.Button(new GUIContent("ScrollBar", "ScrollBar"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.SCROLLBAR);
            }
            if (GUILayout.Button(new GUIContent("Dropdown", "Dropdown"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.DROPDOWN);
            }
            if (GUILayout.Button(new GUIContent("InputField", "输入框"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.INPUTFIELD);
            }
            if (GUILayout.Button(new GUIContent("ListView", "ListView"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
            {
                CreateCtrl(UGUICtrlType.LISTVIEW);
            }

            if (!string.IsNullOrEmpty(errorString))
            {
                EditorGUI.indentLevel = 0;
                EditorGUILayout.HelpBox(errorString, curMessageType, true);
            }
        }

        private void CreateCtrl(UGUICtrlType ctrlType)
        {
            GUILayout.Space(10f);
            GameObject obj = null;
            switch (ctrlType)
            {
                case UGUICtrlType.BUTTON:
                    obj = GameObject.Instantiate(objButton) as GameObject;
                    obj.name = "Button(请重命名)";
                    break;
                case UGUICtrlType.IMAGE:
                    obj = GameObject.Instantiate(objImage) as GameObject;
                    obj.name = "Image(请重命名)";
                    break;
                case UGUICtrlType.RAWIMAGE:
                    obj = GameObject.Instantiate(objRawImage) as GameObject;
                    obj.name = "RawImage(请重命名)";
                    break;
                case UGUICtrlType.CHECKBOX:
                    obj = GameObject.Instantiate(objCheckBox) as GameObject;
                    obj.name = "CheckBox(请重命名)";
                    break;
                case UGUICtrlType.PANEL:
                    obj = GameObject.Instantiate(objPanel) as GameObject;
                    obj.name = "Panel(请重命名)";
                    break;
                case UGUICtrlType.SLIDER:
                    obj = GameObject.Instantiate(objSlider) as GameObject;
                    obj.name = "Slider(请重命名)";
                    break;
                case UGUICtrlType.SCROLLBAR:
                    obj = GameObject.Instantiate(objScrollbar) as GameObject;
                    obj.name = "Scrollbar(请重命名)";
                    break;
                case UGUICtrlType.DROPDOWN:
                    obj = GameObject.Instantiate(objDropdown) as GameObject;
                    obj.name = "Dropdown(请重命名)";
                    break;
                case UGUICtrlType.INPUTFIELD:
                    obj = GameObject.Instantiate(objInputField) as GameObject;
                    obj.name = "InputField(请重命名)";
                    break;
                case UGUICtrlType.LISTVIEW:
                    obj = GameObject.Instantiate(objListView) as GameObject;
                    obj.name = "ListView(请重命名)";
                    break;
                case UGUICtrlType.TEXT:
                    obj = GameObject.Instantiate(objText) as GameObject;
                    obj.name = "Text(请重命名)";
                    break;
                case UGUICtrlType.DIALOG:
                    obj = GameObject.Instantiate(objDlg) as GameObject;
                    obj.name = "Dialog(请重命名)";
                    break;
                default:
                    obj = null;
                    break;
            }

            if(null == obj)
            {
                return;
            }
           
            GameObject activeGameObject = UnityEditor.Selection.activeGameObject;
            if (null == activeGameObject)
            {
                curMessageType = MessageType.Error;
                errorString = "请选中父节点!";
            }
            else
            {
                RectTransform rectTrans = activeGameObject.GetRectTransform();
                if (null == rectTrans)
                {
                    curMessageType = MessageType.Error;
                    errorString = "父节点没有RectTransform组件,请检查!";
                }
                else
                {
                    obj.GetRectTransform().SetParent(rectTrans, false);
                    if(bAutoSelectCreateCtrl || ctrlType == UGUICtrlType.DIALOG)
                    {
                        UnityEditor.Selection.activeGameObject = obj;
                    }
                }
            }
        }

        public void OnGUI()
        {
            if (justEnabled)
            {
                Initialize();
                justEnabled = false;
                Initialized = true;
            }
            DoGUI();
        }

        private void CheckError()
        {
            if(null == UnityEditor.Selection.activeGameObject)
            {
                curMessageType = MessageType.Error;
                errorString = "请在u3dHierarchy中选中对话框结点检测";
                return;
            }
            else
            {
                string activeGameObjectName = UnityEditor.Selection.activeGameObject.name;
                if (activeGameObjectName == "Dialog")
                {
                    curMessageType = MessageType.Warning;
                    errorString = "对话框名应起一个和功能相对应的英文或者拼音名!";
                    return;
                }
                else if(activeGameObjectName == "Canvas2D" || activeGameObjectName == "Canvas3D" || activeGameObjectName == "Camera3D" || activeGameObjectName == "Camera2D" || activeGameObjectName == "UIRoot" || activeGameObjectName == "EventSystem")
                {
                    curMessageType = MessageType.Warning;
                    errorString = "请选择对话框结点!";
                    return;
                }

                if (UnityEditor.Selection.activeGameObject.name == "Dialog")
                {
                    curMessageType = MessageType.Warning;
                    errorString = "对话框名应起一个和功能相对应的英文或者拼音名!";
                    return;
                }
                else
                {
                     UGUICtrlBase[] ctrls = UnityEditor.Selection.activeGameObject.GetComponentsInChildren<UGUICtrlBase>(true);
                    string strName = "";
                    for (int i = 0; i < ctrls.Length; ++i)
                    {
                        strName = ctrls[i].name;
                        for (int j = i + 1; j < ctrls.Length; ++j)
                        {
                            if(strName == ctrls[j].name)
                            {
                                curMessageType = MessageType.Error;
                                errorString = "检查有重名控件!控件名为:" + strName + "-控件类型为:" + ctrls[j].GetType().FullName;
                                UnityEditor.Selection.activeGameObject = ctrls[j].gameObject;
                                return;
                            }
                        }
                    }
                 }
            }
            errorString = "";
        }

        protected virtual void OnEnable()
        {
            justEnabled = true;
        }

    }
}
