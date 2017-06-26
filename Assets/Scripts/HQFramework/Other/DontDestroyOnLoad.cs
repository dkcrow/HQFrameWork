using UnityEngine;


//=====================================================================================/
/// <summary>
/// weding
/// 添加该角本的 GameObject一直存在直接程序结束或手动删除 包含子目录都是DontDestroyOnLoad
/// </summary>
//=====================================================================================.
public class DontDestroyOnLoad : MonoBehaviour 
{
    void Awake()
    {
     if(Application.isPlaying)  DontDestroyOnLoad(gameObject);
    }
}

