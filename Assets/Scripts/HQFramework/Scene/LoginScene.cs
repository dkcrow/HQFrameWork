
using System.Collections;
using HQFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    
    public override EnumSceneType SceneType
    {
        get
        {
            return EnumSceneType.LOGIN;
        }
    }

    public override void OnEnterScene()
    {
        //PhotonNetWorkManager.Instance.Init();
        UGUIDialogManager.Instance.GetInstance<DialogLogin>().ShowDlg(true);
        Messenger.Instance.AddEventListener(MessageEventType.OnJoinedRoom, OnJoinRoomSuccess);
        
    }

    public override void OnLeaveScene()
    {
        Messenger.Instance.RemoveEventListener(MessageEventType.OnJoinedRoom, OnJoinRoomSuccess);
    }

    void OnJoinRoomSuccess()
    {   
       HQSceneManager.Instance.ChangeScene<BattleScene>(); 
    }
}