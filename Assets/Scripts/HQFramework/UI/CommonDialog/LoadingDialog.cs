using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using HQFrameWork;
using UnityEngine;

public class LoadingDialog : UGUIDialogBase {
    private string[] tips = new string[]
    {
        "小提示：1",
        "小提示：2",
        "小提示：3",
        "小提示：4",
        "小提示：5",
        "小提示：6",
        "小提示：7",
        "小提示：8",
    };
    private UIText labPercent;      //百分比
    private UIText labTips;         //下方提示信息
    private UIImage imgProgress;     //旋转进度条
    private Tweener tweener;
    public override void Init(GameObject obj)
    {
        base.Init(obj);
        labPercent = GetCtrlByName("Lab_LoadNum") as UIText;
        labTips = GetCtrlByName("Lab_LoadTips") as UIText;
        //imgProgress = GetCtrlByName("Img_Load") as UIImage;
        Messenger.Instance.AddEventListener(MessageEventType.OnEnterScene, OnSceneAllReady);
        //tweener = imgProgress.RectTrans.DOBlendableRotateBy(new Vector3(0, 0, 360), 2).SetLoops(-1, LoopType.Restart);
    }
    public override void ShowDlg(bool bShow)
    {
        base.ShowDlg(bShow);
        if (bShow)
        {
            transform.SetAsLastSibling();
            tweener.Play();
        }
        else
        {
            tweener.Pause();
        }
    }
 

    public IEnumerator ShowLoading( )
    {
        ShowDlg(true);
        SetTipStr();
        int progress = 0;
        while (progress++<100)
        {
            SetPercent(progress);
            yield return new WaitForEndOfFrame();
        }
      
    }
    private void OnSceneAllReady()
    {
        SetPercent(100);
        if (gameObject.activeSelf)
        {
            StartCoroutine(WaitToClose());
        }
    }
    IEnumerator WaitToClose()
    {
        yield return new WaitForSeconds(0.8f);
        ShowDlg(false);
    }
    private void SetTipStr()
    {
        string info = "";
        //if (UserInfo.Instance.m_RoleAttribute.dwLevel < 11)
        //{
        //    info = tips[0];
        //}
        //else if (UserInfo.Instance.m_RoleAttribute.dwLevel < 21)
        //{
        //    info = tips[1];
        //}
        //else if (UserInfo.Instance.m_RoleAttribute.dwLevel < 31)
        //{
        //    info = tips[2];
        //}
        //else if (UserInfo.Instance.m_RoleAttribute.dwLevel < 51)
        //{
        //    info = tips[3];
        //}
        //else if (UserInfo.Instance.m_RoleAttribute.dwLevel < 101)
        //{
        //    info = tips[4];
        //}
        SetTips(info);
    }
    /// <summary>
    /// 设置加载进度
    /// </summary>
    /// <param name="per"></param>
    public void SetPercent(float per)
    {
        labPercent.Text = per.ToString() + "%";
    }
    /// <summary>
    /// 设置tips
    /// </summary>
    /// <param name="str"></param>
    public void SetTips(string str)
    {
        labTips.Text = str;
    }
   
}
