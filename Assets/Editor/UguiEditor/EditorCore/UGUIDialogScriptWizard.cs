//=====================================================================================/
/// <summary>
/// weding
/// UI Dialog角本生成器
/// </summary>
//=====================================================================================.
namespace HQFrameWork
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    public class UIDialogScriptWizard : EditorWindow
    {
       // private string[] actionCategories;
        private string actionFolder = "Dialogs";
        private string dialogName = "";
        private string code;
        private Vector2 controlsScrollPosition;
        private string customCategory = "///<summary>\n///请在这里定注释:\n///<summary>";
        private bool draggingPreviewDivider;
        private string errorString;
        //private bool folderSameAsCategory = true;
        private string fullFileName;
        private bool handlesInit = true;

        //private bool handlesOnExit;
        //private bool handlesOnFixedUpdate;
        //private bool handlesOnLateUpdate;
        //private bool handlesOnUpdate;
        //private bool hasCustomErrorChecker;

        private bool handlesCtrls = true;

        private string strCtrlList = "";

        private bool isValid;
        private string localAssetFilename;
        private Rect previewDividerRect;
        private float previewHeight;
        private Vector2 previewScrollPosition;
        private string rootFolder = "FvsT/Scripts/UI/";
        private int selectedCategory;

        private string strWizardPreviewHeight = "UIDialogScriptWizard.PreviewHeight";
        private string strRootFolder = "UIDialogScriptWizard.RootFolder";

        private UGUICtrlBase[] ctrls = null;
        private string strInitCtrl = "";
        private string strCtrlPrefix = "ctrl_";
        


        //消息监听处理
        private bool handlesButtonListen = true;
        private string strOnEventListenerButton = "";
        private string strInitOnEventListenerButton = "";

        [Localizable(false)]
        private void BuildCode()
        {
            code = "using UnityEngine;\nusing HQFrameWork;\nusing UnityEngine.UI;\n\n\n";
            code = code + customCategory + "\n";

            code = code + "public class " + dialogName + " : UGUIDialogBase\n{\n\n";

            if (handlesCtrls)
            {
                code = code + strCtrlList;
            }


            if (handlesInit)
            {
                string str = BuildOverrideMethod("void Init(GameObject obj)", "重写基类方法", "base.Init(obj);");
                
                if (handlesCtrls)
                {
                    str = str.Replace(@"}", "");

                    if(str.Length > 2)
                    {
                        str = str.Remove(str.Length - 2);
                    }
                    str += strInitCtrl;

                    if(handlesButtonListen)
                    {
                        str += strInitOnEventListenerButton;
                    }
                    
                    str += "\n\t}\n\n";
                }
                code = code + str;
            }

            //btn 监听
            if(handlesButtonListen)
            {
                code = code + strOnEventListenerButton;
            }

            //if (handlesOnUpdate)
            //{
            //    code = code + BuildOverrideMethod("void OnUpdate()", "进入状态每帧调用", "");
            //}
            //if (handlesOnFixedUpdate)
            //{
            //    code = code + BuildOverrideMethod("void OnFixedUpdate()", "", "");
            //}
            //if (handlesOnLateUpdate)
            //{
            //    code = code + BuildOverrideMethod("void OnLateUpdate()", "", "");
            //}
            //if (handlesOnExit)
            //{
            //    code = code + BuildOverrideMethod("void OnExit()", "出状态时调用", "");
            //}
            //if (hasCustomErrorChecker)
            //{
            //    code = code + BuildOverrideMethod("string ErrorCheck()", "自定义错误检查", "// 无错返回null.\n\nreturn null;");
            //}
            code = code + "\n}\n";
        }


        [Localizable(false)]
        private void BuildAllCtrls()
        {
            strCtrlList = "";
            strInitCtrl = "";
            strOnEventListenerButton = "";
            strInitOnEventListenerButton = "";

            //类别排序
            string strPanel = "";
            string strImage = "";
            string strRawImage = "";
            string strButton = "";
            string strText = "";
            string strCheckBox = "";
            string strInputField = "";
            string strScrollbar = "";
            string strSlider = "";
            string strDropdown = "";
            string strListView = "";

            //获取
            string strPanelGet = "";
            string strImageGet = "";
            string strRawImageGet = "";
            string strButtonGet = "";
            string strTextGet = "";
            string strCheckBoxGet = "";
            string strInputFieldGet = "";
            string strScrollbarGet = "";
            string strSliderGet = "";
            string strDropdownGet = "";
            string strListViewGet = "";

            if (null != ctrls && ctrls.Length > 0)
            {
                strCtrlList = strCtrlList + "\t#region 所有控件";

                foreach(UGUICtrlBase ctrl in ctrls)
                {
                    SetStrCtrlPrefix(ctrl.ctrlType);
                    switch (ctrl.ctrlType)
                    {
                        case UGUICtrlType.PANEL:
                            {
                                //定义部分
                                strPanel += "\n\t private UIPanel\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";

                                //初始化部分
                                strPanelGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "this[\"" + ctrl.gameObject.name + "\"] as UIPanel;";
                            }
                            break;
                        case UGUICtrlType.BUTTON:
                            {
                                strButton += "\n\t private Button\t\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strButtonGet += "\n\t\t" + strCtrlPrefix +  ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIButton).button;";

                                strInitOnEventListenerButton += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + ".onClick.AddListener("+ "On" + ctrl.gameObject.name + "Click);";
                                //消息监听部分
                                strOnEventListenerButton = strOnEventListenerButton + BuildMethod("void On" + ctrl.gameObject.name + "Click()", "", "" , "private");
                            }
                            break;
                        case UGUICtrlType.IMAGE:
                            {
                                strImage += "\n\t private Image\t\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strImageGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIImage).image;";
                            }
                            break;
                        case UGUICtrlType.RAWIMAGE:
                            {
                                strRawImage += "\n\t private RawImage\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strRawImageGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIRawImage).rawImage;";
                            }
                            break;
                        case UGUICtrlType.TEXT:
                            {
                                strText += "\n\t private Text\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strTextGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIText).text;";
                            }
                            break;
                        case UGUICtrlType.CHECKBOX:
                            {
                                strCheckBox += "\n\t private Toggle\t\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strCheckBoxGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UICheckBox).checkBox;";
                            }
                            break;
                        case UGUICtrlType.SLIDER:
                            {
                                strSlider += "\n\t private Slider\t\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strSliderGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UISlider).slider;";
                            }
                            break;
                        case UGUICtrlType.SCROLLBAR:
                            {
                                strScrollbar += "\n\t private Scrollbar\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strScrollbarGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIScrollbar).scrollbar;";
                            }
                            break;
                        case UGUICtrlType.DROPDOWN:
                            {
                                strDropdown += "\n\t private Dropdown\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strDropdownGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIDropdown).dropdown;";
                            }
                            break;
                        case UGUICtrlType.INPUTFIELD:
                            {
                                strInputField += "\n\t private InputField\t\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strInputFieldGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIInputField).inputField;";
                            }
                            break;
                        case UGUICtrlType.LISTVIEW:
                            {
                                strListView += "\n\t private UIListView\t\t" + strCtrlPrefix + ctrl.gameObject.name + ";";
                                strListViewGet += "\n\t\t" + strCtrlPrefix + ctrl.gameObject.name + " = " + "(this[\"" + ctrl.gameObject.name + "\"] as UIListView);";
                            }
                            break;
                    }
                }


                if(!string.IsNullOrEmpty(strPanelGet))
                {
                    strPanelGet = "\n\t\t//Panel ctrl" + strPanelGet + "\n";
                }
                if (!string.IsNullOrEmpty(strImageGet))
                {
                    strImageGet = "\n\t\t//Image ctrl" + strImageGet + "\n";
                }
                if (!string.IsNullOrEmpty(strRawImageGet))
                {
                    strRawImageGet = "\n\t\t//RawImage ctrl" + strRawImageGet + "\n";
                }
                if (!string.IsNullOrEmpty(strButtonGet))
                {
                    strButtonGet = "\n\t\t//Button ctrl" + strButtonGet + "\n";
                }
                if (!string.IsNullOrEmpty(strTextGet))
                {
                    strTextGet = "\n\t\t//Text ctrl" + strTextGet + "\n";
                }
                if (!string.IsNullOrEmpty(strCheckBoxGet))
                {
                    strCheckBoxGet = "\n\t\t//CheckBox ctrl" + strCheckBoxGet + "\n";
                }
                if (!string.IsNullOrEmpty(strInputFieldGet))
                {
                    strInputFieldGet = "\n\t\t//InputField ctrl" + strInputFieldGet + "\n";
                }
                if (!string.IsNullOrEmpty(strScrollbarGet))
                {
                    strScrollbarGet = "\n\t\t//Scrollbar ctrl" + strScrollbarGet + "\n";
                }
                if (!string.IsNullOrEmpty(strSliderGet))
                {
                    strSliderGet = "\n\t\t//Slider ctrl" + strSliderGet + "\n";
                }

                if (!string.IsNullOrEmpty(strDropdownGet))
                {
                    strDropdownGet = "\n\t\t//Dropdown ctrl" + strDropdownGet + "\n";
                }

                if (!string.IsNullOrEmpty(strListViewGet))
                {
                    strListViewGet = "\n\t\t//strListView ctrl" + strListViewGet + "\n";
                }
                strInitCtrl = strPanelGet + strImageGet + strRawImageGet + strButtonGet + strTextGet + strCheckBoxGet + strInputFieldGet + strScrollbarGet + strSliderGet + strDropdownGet + strListViewGet;

                strInitCtrl = "\n\t\t#region 初始化所有控件" + strInitCtrl + "\n\t\t#endregion";
                strCtrlList = strCtrlList + strPanel + strImage + strRawImage + strButton + strText + strCheckBox + strInputField + strScrollbar + strSlider + strDropdown + strListView;
                strCtrlList += "\n\t#endregion\n\n";
                strOnEventListenerButton = "\n\t#region Button监听\n" + strOnEventListenerButton + "\t#endregion";
            }
        }

        [Localizable(false)]
        private static string BuildOverrideMethod(string methodName, string comment = "", string body = "", string le = "public")
        {
            string str = "";
            if (!string.IsNullOrEmpty(comment))
            {
                str = str + "\t// " + comment + "\n";
            }
            return (((str + "\tpublic override " + methodName + "\n\t{\n") + "\t\t" + body.Replace("\n", "\n\t\t") + "\n") + "\t}\n\n");
        }

        [Localizable(false)]
        private static string BuildMethod(string methodName, string comment = "", string body = "", string le = "public")
        {
            string str = "";
            if (!string.IsNullOrEmpty(comment))
            {
                str = str + "\t// " + comment + "\n";
            }
            return (((str + "\t" + le + " " + methodName + "\n\t{\n") + "\t\t" + body.Replace("\n", "\n\t\t") + "\n") + "\t}\n\n");
        }

        private static void ControlGroup(string title)
        {
            GUILayout.Space(10f);
            GUILayout.Label(title, EditorStyles.boldLabel, new GUILayoutOption[0]);
            GUILayout.Space(5f);
        }

        private void CopyCodeToClipboard()
        {
            EditorGUIUtility.systemCopyBuffer = code;
        }

        [Localizable(false)]
        private void HandleDragPreviewDivider()
        {
            switch (Event.current.type)
            {
                case UnityEngine.EventType.MouseDown:
                    if (previewDividerRect.Contains(Event.current.mousePosition))
                    {
                        draggingPreviewDivider = true;
                    }
                    break;

                case UnityEngine.EventType.MouseUp:
                    draggingPreviewDivider = false;
                    EditorPrefs.SetFloat(strWizardPreviewHeight, previewHeight);
                    break;

                default:
                    if (draggingPreviewDivider && Event.current.isMouse)
                    {
                        previewHeight = (base.position.height - Event.current.mousePosition.y) - 60f;
                        base.Repaint();
                    }
                    break;
            }
            previewHeight = Mathf.Clamp(previewHeight, 40f, base.position.height - 200f);
        }
        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string SetStrCtrlPrefix(UGUICtrlType type)
        {
            switch (type)
            {
                case UGUICtrlType.UNKNOW:
                    strCtrlPrefix = "ctrl";
                    break;
                case UGUICtrlType.DIALOG:
                    strCtrlPrefix = "dlg";
                    break;
                case UGUICtrlType.IMAGE:
                    strCtrlPrefix = "img";
                    break;
                case UGUICtrlType.RAWIMAGE:
                    strCtrlPrefix = "rawImg";
                    break;
                case UGUICtrlType.BUTTON:
                    strCtrlPrefix = "btn";
                    break;
                case UGUICtrlType.PANEL:
                    strCtrlPrefix = "panel";
                    break;
                case UGUICtrlType.TEXT:
                    strCtrlPrefix = "lab";
                    break;
                case UGUICtrlType.CHECKBOX:
                    strCtrlPrefix = "toggle";
                    break;
                case UGUICtrlType.SLIDER:
                    strCtrlPrefix = "sld";
                    break;
                case UGUICtrlType.SCROLLBAR:
                    strCtrlPrefix = "scrBar";
                    break;
                case UGUICtrlType.DROPDOWN:
                    strCtrlPrefix = "drop";
                    break;
                case UGUICtrlType.INPUTFIELD:
                    strCtrlPrefix = "input";
                    break;
                case UGUICtrlType.LISTVIEW:
                    strCtrlPrefix = "listView";
                    break;
                default:
                    strCtrlPrefix = "ctrl";
                    break;
            }
            return strCtrlPrefix;
        }
        //private bool HasUpdateMethod()
        //{
        //    if (!handlesOnUpdate && !handlesOnFixedUpdate)
        //    {
        //        return handlesOnLateUpdate;
        //    }
        //    return true;
        //}

        private bool IsValidSetup()
        {
            errorString = "";
            if (string.IsNullOrEmpty(dialogName))
            {
                errorString = errorString + "无效名称" + Environment.NewLine;
            }
            if (!CodeGenerator.IsValidLanguageIndependentIdentifier(dialogName))
            {
                errorString = errorString + "无效名称,请确认是否命名了或者是否有无效字符!" + Environment.NewLine;
            }
            if (File.Exists(fullFileName))
            {
                errorString = errorString + "对话框角本文件已存在" + Environment.NewLine;
            }
            return (errorString == "");
        }

        [Localizable(false)]
        private void OnEnable()
        {
            titleContent.text = "对话框脚本生成编辑器";
            rootFolder = EditorPrefs.GetString(strRootFolder, "FvsT/Resources/ResourcesAB/Prefabs/UI/");
            previewHeight = EditorPrefs.GetFloat(strWizardPreviewHeight, 200f);
            base.wantsMouseMove = true;
            //List<string> list = Enum.GetNames(typeof(ActionCategory)).ToList<string>();
            //list.Sort();
            //actionCategories = list.ToArray();
            if (base.position.height < 400f)
            {
                base.position = new Rect(base.position.x, base.position.y, base.position.width, 600f);
            }
            base.minSize = new Vector2(500f, 400f);
            UpdateGUI();
        }

        private void OnFocus()
        {
            UpdateGUI();
            draggingPreviewDivider = false;
        }


        private static string TextFieldWithHint(string text, string hint, params GUILayoutOption[] layoutOptions)
        {
            bool changed = GUI.changed;
            Color contentColor = GUI.contentColor;
            string str = text;
            if (string.IsNullOrEmpty(str))
            {
                str = hint;
                Color textColor = EditorStyles.label.normal.textColor;
                textColor.a = 0.5f;
                GUI.contentColor = textColor;
            }
            GUI.changed = false;
            str = EditorGUILayout.TextField(str, layoutOptions);
            GUI.contentColor = contentColor;
            if (GUI.changed)
            {
                return str;
            }
            GUI.changed = changed;
            return text;
        }

        private void OnGUI()
        {
            HandleDragPreviewDivider();
            controlsScrollPosition = EditorGUILayout.BeginScrollView(controlsScrollPosition, new GUILayoutOption[0]);
            EditorGUI.indentLevel = 1;
            ControlGroup("对话框名称,注意对话框名应和prefab名一样,在点击同时会获取所有ctrls");
            if (GUILayout.Button(new GUIContent("生成对话框名和拉取控件", "请在Hierarchy中先中对话框根GameObject.然后点击就可以获取对话框名和所有组件"), new GUILayoutOption[] { GUILayout.MaxWidth(200f) }))
            {
                ctrls = null;
                GameObject obj = UnityEditor.Selection.activeGameObject;
                if(null == obj)
                {
                    EditorUtility.DisplayDialog("错误", "请在U3D编辑器的Hierarchy中先中对话框GameObject", "确定");
                }
                else if(!obj.name.Contains("Dialog"))
                {
                    EditorUtility.DisplayDialog("错误", "你在u3d编辑器中选定的不是对话框,或者对话框GameObject命名不规范,对话框必须以Dialog结尾!", "确定");
                }
                else
                {
                    dialogName = obj.name;
                    ctrls = obj.GetComponentsInChildren<UGUICtrlBase>();
                    BuildAllCtrls();
                }
            }

            dialogName = TextFieldWithHint(dialogName, "对话框名称会和你选中Hierarchy中对话框的GameObject名相同", new GUILayoutOption[0]);
            ControlGroup("存储对话框角本代码文件夹");
            rootFolder = EditorGUILayout.TextField("根文件夹", rootFolder, new GUILayoutOption[0]);

            ControlGroup("添加功能和方法");
            handlesCtrls = EditorGUILayout.Toggle("自动处理所有ctrl", handlesCtrls, new GUILayoutOption[0]);
            handlesInit = EditorGUILayout.Toggle("Init", handlesInit, new GUILayoutOption[0]);

            handlesButtonListen = EditorGUILayout.Toggle("自动Button监听处理", handlesButtonListen, new GUILayoutOption[0]);

            //handlesOnUpdate = EditorGUILayout.Toggle("OnUpdate", handlesOnUpdate, new GUILayoutOption[0]);
            //handlesOnFixedUpdate = EditorGUILayout.Toggle("OnFixedUpdate", handlesOnFixedUpdate, new GUILayoutOption[0]);
            //handlesOnLateUpdate = EditorGUILayout.Toggle("OnLateUpdate", handlesOnLateUpdate, new GUILayoutOption[0]);
            //handlesOnExit = EditorGUILayout.Toggle("OnExit", handlesOnExit, new GUILayoutOption[0]);
            //hasCustomErrorChecker = EditorGUILayout.Toggle("错误检查函数", hasCustomErrorChecker, new GUILayoutOption[0]);
            EditorGUILayout.EndScrollView();
            if (!isValid)
            {
                EditorGUI.indentLevel = 0;
                EditorGUILayout.HelpBox(errorString, MessageType.Error, true);
            }
            GUILayout.BeginHorizontal(EditorStyles.toolbar, new GUILayoutOption[0]);
            GUILayout.Label("代码预览", new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (Event.current.type == UnityEngine.EventType.Repaint)
            {
                previewDividerRect = GUILayoutUtility.GetLastRect();
            }
            EditorGUIUtility.AddCursorRect(previewDividerRect, MouseCursor.ResizeVertical);
            previewScrollPosition = EditorGUILayout.BeginScrollView(previewScrollPosition, new GUILayoutOption[] { GUILayout.MinHeight(previewHeight) });
            GUILayout.Label(code, new GUILayoutOption[0]);
            EditorGUILayout.EndScrollView();
            GUILayout.Label("路径前缀" + localAssetFilename, new GUILayoutOption[0]);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUI.enabled = isValid;
            if (GUILayout.Button("保存", new GUILayoutOption[0]))
            {
                SaveCustomAction();
                GUIUtility.ExitGUI();
            }
            else
            {
                GUI.enabled = true;
                if (GUILayout.Button(new GUIContent("查找文件", "查一个文件"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
                {
                    PingScriptFile();
                }
                if (GUILayout.Button(new GUIContent("复制代码", "将生成的代吗复制到剪切板"), new GUILayoutOption[] { GUILayout.MaxWidth(100f) }))
                {
                    CopyCodeToClipboard();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
                EditorGUI.indentLevel = 0;
                if (GUI.changed)
                {
                    UpdateGUI();
                    GUIUtility.ExitGUI();
                }
            }
        }

        [Localizable(false)]
        private void PingScriptFile()
        {
            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/" + localAssetFilename));
        }

        private void SaveCustomAction()
        {
            Debug.Log("生成自定义动作文件"  + fullFileName);
            string directoryName = Path.GetDirectoryName(fullFileName);
            if (string.IsNullOrEmpty(directoryName))
            {
                Debug.LogError(string.Format("无效路径", fullFileName));
            }
            else
            {
                try
                {
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                }
                catch (Exception)
                {
                    Debug.LogError("不能创建目录" + directoryName);
                    return;
                }
                using (StreamWriter writer = new StreamWriter(fullFileName))
                {
                    writer.Write(code);
                    writer.Close();
                }
                AssetDatabase.Refresh();
                PingScriptFile();
            }
        }

        [Localizable(false)]
        private void UpdateGUI()
        {
            previewHeight = Mathf.Clamp(previewHeight, 40f, base.position.height - 190f);
            EditorPrefs.SetString(strRootFolder, rootFolder);
            //if (folderSameAsCategory)
            //{
            //    actionFolder = string.IsNullOrEmpty(customCategory) ? actionCategories[selectedCategory] : customCategory;
            //}
            string str = Path.Combine(rootFolder, actionFolder);
            localAssetFilename = Path.Combine(str, dialogName + ".cs");
            fullFileName = Path.Combine(Application.dataPath, localAssetFilename);
            localAssetFilename = localAssetFilename.Replace(@"\", "/");
            fullFileName = fullFileName.Replace(@"\", "/");
            BuildCode();
            isValid = IsValidSetup();
        }
    }
}

