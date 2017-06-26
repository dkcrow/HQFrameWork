using System;
using System.Collections.Generic;
using UnityEngine;
using HQFrameWork;
//using PVPSdk;
//using PVPSdk.PVP;
using UnityEngine.UI;


///<summary>
///请在这里定注释:
///<summary>
public class DialogLogin : UGUIDialogBase
{

    private UIButton ButtonJoin;
    private UIInputField InputFieldUserName;
    private UIButton ButtonQuit;
    private UIButton ButtonSelectServer;
    private UIButton ButtonShop;
    private UIText TxtConnectState;
    // 重写基类方法
    public override void Init(GameObject obj)
	{
		base.Init(obj);
        
        ButtonJoin = GetCtrlByName("ButtonJoin") as UIButton;
        //ButtonQuit = GetCtrlByName("ButtonQuit") as UIButton;
        ButtonShop = GetCtrlByName("ButtonShop") as UIButton;
        //ButtonSelectServer = GetCtrlByName("ButtonSelectServer") as UIButton;
        //InputFieldUserName = GetCtrlByName("InputFieldUserName") as UIInputField;
        //TxtConnectState = GetCtrlByName("TxtConnectState") as UIText;
        //InputFieldUserName.Text = CustomDefine.PlayerName;-
        ButtonJoin.AddOnClickListener(Login);
        //ButtonQuit.AddOnClickListener(Application.Quit);
        ButtonShop.AddOnClickListener(JoinGame);

        Messenger.Instance.AddEventListener<string>(MessageEventType.ConnectInfo, UpdateConnectingState);
        //PVPGlobal.handlerRegister.loginOrRegisterResponseHandler += OnLoginOrRegister;
        //PVPGlobal.handlerRegister.connectedToGameServerEventHandler += OnConnectedToGameServer;
        //PVPGlobal.handlerRegister.gameServerNetworkErrorEventHandler += OnConnectGameServerFailed;
        //PVPGlobal.handlerRegister.leaveRoomResponseHandler += OnLeaveRoom;//离开房间成功
        //PVPGlobal.handlerRegister.otherMemberLeaveRoomBroadcastHandler += OnMembersLeaveRoom;//自己离开房间
        //PVPGlobal.handlerRegister.roomMatchOpponentResponseHandler += OnMatchPlayerFinish;

	}


    void GameStart()
    {
        DataSubjectManager.Instance.Notify(DataSubjectManager.GetData<DataGame>(), (int)DataGame.EventType.OnEnterGame);
    }

    /// <summary>
    /// 暂定为游客登录
    /// </summary>
    void Login()
    {
        print("join Now");
        //NetWorkManager.Instance.Login(LoginType.Guest, "","");
        //PVPSdk.Player.OpLoginOrRegister(LoginType.Guest);
        //PhotonNetwork.playerName= CustomDefine.PlayerName = InputFieldUserName.Text;
        //PlayerPrefs.SetString("PlayerName", CustomDefine.PlayerName);
        //PhotonNetWorkManager.Instance.CreateOrJoinRoom();
    }

    void JoinGame()
    {
        //if (PVPGlobal.isConnected)
        //{
        //    NetWorkManager.Instance.MatchPlayer(1, 1);
        //}
        
    }

    public override void OnChangeScene()
    {
        base.OnChangeScene();
        Messenger.Instance.RemoveEventListener<string>(MessageEventType.ConnectInfo, UpdateConnectingState);
        DestroyObject(this.gameObject);
    }

    public void UpdateConnectingState(string connectInfo)
    {
        TxtConnectState.Text = "ConnectingState:" + " " + connectInfo;
        //ButtonJoin.Interactable = PhotonNetwork.connectionState == ConnectionState.Connected;
    }

    public void OnLoginOrRegister(int errorCode)
    {
        print("get now");
        switch (errorCode)
        {
            //case ErrorCode.SUCCESS:
            //    print("login success now" + errorCode);
            //    NetWorkManager.Instance.ConnectGameServer();//此时pvpglobal.player已经被自动分配,开始连接服务器
            //    break;
            //case ErrorCode.NETWORK_ERROR:
            //    print("login fail now");
            //    break;
        }

    }

    
    public void OnConnectedToGameServer(int errorCode)
    {

        //print("连接服务器成功 你好"+PVPGlobal.localAppPlayer.name);
  
       
    }


    //void OnGetLobbyList(int errorCode, List<Lobby> lobbyInfoList)
    //{
    //    foreach (var lobby in lobbyInfoList)
    //    {
    //        print(lobby.name);
    //    }
    //}

    void OnConnectGameServerFailed()
    {
        print("OnConnectGameServerFailed");
    }

    void OnMembersLeaveRoom(int errorCode, UInt64 memberPlayerId)
    {
        print("one member leave room now id is"+ memberPlayerId);
    }

    void OnLeaveRoom(int resultCode)
    {
        print("self leave room now"+ resultCode);
    }


    private bool bIsEnterGame = false;
    //void OnMatchPlayerFinish(int errorCode, bool isMatchFinished, List<PlayerMiniInfo> PlayerMiniInfos)
    //{
    //    if (isMatchFinished == false)
    //    {
    //        print("正在匹配,当前队列"+PlayerMiniInfos.Count);
    //        return;
    //    }

    //   if(bIsEnterGame)
    //        return;

    //   if (isMatchFinished)
    //    {
    //        print("匹配成功,开始转场景");
    //        HQSceneManager.Instance.ChangeScene<TestBattleScene>();
    //        bIsEnterGame = true;
    //    }
    //}
}
