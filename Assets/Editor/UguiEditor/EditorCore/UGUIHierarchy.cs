using UnityEngine;
using System.Collections;
using UnityEditor;
public class UGUIHierarchy
{
    private const string PATH = "BasePrefabs/";


    [MenuItem("GameObject/UI/HQUGUI/UGUIText")]
    public static void UGUIText()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "Text")) as GameObject;
        obj.name = "Text(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIImage")]
    public static void UGUIImage()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "Image")) as GameObject;
        obj.name = "Image(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIButton")]
    public static void UGUIButton()
    {
      GameObject  obj = GameObject.Instantiate(Resources.Load(PATH + "Button")) as GameObject;
      obj.name = "Button(请重命名)";
      SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIDialog")]
    public static void UGUIDialog()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "Dialog")) as GameObject;
        obj.name = "Dialog(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIRAWIMAGE")]
    public static void UGUIRAWIMAGE()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "RAWIMAGE")) as GameObject;
        obj.name = "RAWIMAGE(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUICheckBox")]
    public static void UGUICheckBox()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "CheckBox")) as GameObject;
        obj.name = "CheckBox(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUISlider")]
    public static void UGUISlider()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "Slider")) as GameObject;
        obj.name = "Slider(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIScrollBar")]
    public static void UGUIScrollBar()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "ScrollBar")) as GameObject;
        obj.name = "ScrollBar(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIDropdown")]
    public static void UGUIDropdown()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "Dropdown")) as GameObject;
        obj.name = "Dropdown(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIInputField")]
    public static void UGUIInputField()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "InputField")) as GameObject;
        obj.name = "InputField(请重命名)";
        SetParent(obj);
    }
    [MenuItem("GameObject/UI/HQUGUI/UGUIListView")]
    public static void UGUIListView()
    {
        GameObject obj = GameObject.Instantiate(Resources.Load(PATH + "ListView")) as GameObject;
        obj.name = "ListView(请重命名)";
        SetParent(obj);
    }
    private static void SetParent(GameObject obj)
    {
        if (null != Selection.activeGameObject)
           obj.transform.SetParent(Selection.activeGameObject.transform, false); 
    }
}
