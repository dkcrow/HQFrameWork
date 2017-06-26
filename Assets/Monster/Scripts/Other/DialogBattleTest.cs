using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HQFrameWork;
using UnityEngine;

public class DialogBattleTest : UGUIDialogBase
{
    //public UIText ContentText;
    //public UIInputField InputFieldTest;
    public UIButton AddBtn;
    public UIButton DelBtn;
    public UIButton SendPosBtn;
    public UIButton RightBtn;
    public UIButton LeftBtn;

    public override void Init(GameObject obj)
    {
        
        base.Init(obj);
        //ContentText = GetCtrlByName("ContentText") as UIText;
        //InputFieldTest = GetCtrlByName("InputFieldTest") as UIInputField;
        AddBtn = GetCtrlByName("AddBtn") as UIButton;
        DelBtn = GetCtrlByName("DelBtn") as UIButton;
        SendPosBtn = GetCtrlByName("SendPosBtn") as UIButton;
        LeftBtn = GetCtrlByName("LeftBtn") as UIButton;
        RightBtn = GetCtrlByName("RightBtn") as UIButton;
        //ICM.handlerRegister.roomNewMessageBroadcastHandler += OnReceiveMsg;
        //ICM.handlerRegister.roomMemberCustomDataResponseHandler += OnMemberUpdateInfo;
        //ICM.handlerRegister.roomMemberCustomDataBroadcastHandler += OnReceiveOtherMemberUpdateInfo;
        AddBtn.AddOnClickListener(() => EntityManager.Instance.AddEntityByType(Random.Range(2,6),Random.Range(1000,999999), Random.Range(-CustomDefine.WorldHalfWidth, CustomDefine.WorldHalfWidth), Random.Range(-CustomDefine.WorldHalfHeight, CustomDefine.WorldHalfHeight)));
        //DelBtn.AddOnClickListener(() => EntityManager.Instance.RemoveEntityById(EntityManager.Instance._entityManagerDictionary.Keys.ElementAt(0)));
        LeftBtn.AddOnClickListener(() =>
        {
            NetWorkManager.Instance.SendMovLeftRequest();
        });
        RightBtn.AddOnClickListener(() =>
        {
            NetWorkManager.Instance.SendMovRightRequest();
        });
       
        //SendPosBtn.AddOnClickListener(() => EntityManager.Instance.UpdatePlayerPos(new List<CustomDefine.PlayerPosSyncMsg>()
        //{
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.1f,posZ = 1.1f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.2f,posZ = 1.2f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.3f,posZ = 1.3f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.4f,posZ = 1.4f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.5f,posZ = 1.5f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.6f,posZ = 1.6f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.7f,posZ = 1.7f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.8f,posZ = 1.8f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.9f,posZ = 1.9f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 2f,posZ = 2f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.9f,posZ = 1.9f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.8f,posZ = 1.8f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.7f,posZ = 1.7f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.6f,posZ = 1.6f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.5f,posZ = 1.5f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.4f,posZ = 1.4f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.3f,posZ = 1.3f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.2f,posZ = 1.2f},
        //    new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1.1f,posZ = 1.1f},

        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //    //new CustomDefine.PlayerPosSyncMsg(){id=EntityManager.Instance._playerManagerDictionary.Keys.ElementAt(0),posX = 1,posZ = 1},
        //}));
    }

    void SendMsg()
    {
        //if (InputFieldTest.Text != "")
        //{
        //    byte[] toSendBytes = System.Text.Encoding.UTF8.GetBytes(InputFieldTest.Text);
        //    //NetWorkManager.Instance.BroadcastMsg(233, toSendBytes);
        //    //NetWorkManager.Instance.UpdatePlayerInfo(123, UpdateCustomDataRange.All, null,
        //    //    new List<string>() {InputFieldTest.Text});
        //}
    }

    public override void ShowDlg(bool bShow)
    {
        base.ShowDlg(bShow);
        
    }

    public override void OnChangeScene()
    {
        base.OnChangeScene();
        //ICM.handlerRegister.roomNewMessageBroadcastHandler -= OnReceiveMsg;
    }

    void ShowMsg(object msg)
    {
        //ContentText.Text += msg + "\n";
    }
    //void OnReceiveMsg(int errorCode, RoomNewMessage m)
    //{

    //   ShowMsg(System.Text.Encoding.UTF8.GetString(m.message)+" commandID "+m.customCommandId);
    //}

    //void OnMemberUpdateInfo(int errorCode, NewCustomData meta)
    //{
    //    ShowMsg("response "+meta.commandId );
    //}

    //void OnReceiveOtherMemberUpdateInfo(int errorCode, NewCustomData meta)
    //{
    //   ShowMsg(meta.commandId);
    //}
}
