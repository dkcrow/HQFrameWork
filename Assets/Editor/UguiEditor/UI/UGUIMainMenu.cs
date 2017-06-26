
//=====================================================================================.
using System.ComponentModel;
using HQFrameWork;
using UnityEditor;

[Localizable(false)]
static class UGUIMainMenu
{
    private const string MenuRootName = "Tools/HQ_UGUIEditor/";

    /// <summary>
    /// 编辑器主窗口
    /// </summary>
    [MenuItem(MenuRootName + "OpenUGUIEditor", false, 1)]
    public static void OpenUGUIEditor()
	{
        UGUIEditorWindow.OpenWindow();
    }


    [MenuItem(MenuRootName + "GenerateUGUIScript", false, 2)]
    public static void CreateWizard()
    {
        EditorWindow.GetWindow<UIDialogScriptWizard>(false);
    }
}
