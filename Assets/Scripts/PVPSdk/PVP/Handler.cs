using System;
using System.Collections.Generic;

/**
 * 回调命名空进
 */ 
namespace PVPSdk.PVP.Handler
{
    //响应的委托

    /// <summary>
    /// 用户自动登陆
    /// </summary>
    public delegate void AutoLoginOrRegisterResponseHandler(int errorCode);

    /// <summary>
    /// 用户登录（如果用户不存在就先注册，后登陆），操作完成后的委托，
    /// errorCode 错误码 ErrorCode.SUCCESS: 登录成功，登录成功后可以访问 PVPGlobal.Player 获取用户的基本信息
    /// 错误码 ErrorCode.NETWORK_ERROR: 网络异常
    /// </summary>
    public delegate void LoginOrRegisterResponHandler (int errorCode);

    /// <summary>
    /// Lobby get lobby list respon handler.
    /// </summary>
    internal delegate void LobbyGetLobbyListResponHandler (int errorCode,List<Lobby> lobbyInfoList);

    /// <summary>
    /// PVPSdk.Lobby::OpEnter() 进入大厅成功后的回调，建议不要使用
    /// </summary>
    internal delegate void EnterLobbyResponseHandler (int errorCode);
//    public delegate void GetRoomListResponseHandler (int errorCode,List<PVPSdk.Room> rooms);
//    public delegate void EnteredRoomResponseHandler (int errorCode);

    /// <summary>
    /// 调用 PVPGlobak.room.OpLeaveRoom() 的回调
    /// </summary>
    public delegate void LeaveRoomResponseHandler (int errorCode);


    public delegate void RoomMatchOpponentResponseHandler (int errorCode, bool isMatchFinished, List<PlayerMiniInfo> PlayerMiniInfos);


    public delegate void SocketCheckTokenResponseHandler (int errorCode);


    public delegate void RoomCustomDataResponseHandler (int errorCode, NewCustomData meta);
    public delegate void RoomNewMessageResponseHandler(int errorCode, RoomNewMessage meta);

    public delegate void RoomMemberCustomDataResponseHandler (int errorCode, NewCustomData meta);
    public delegate void RoomCancelMatchOpponentResonseHandler (int errorCode);

    public delegate void AppPlayerUpdateInfoResponseHandler (int errorCode, UpdateAppPlayerInfoResult meta);
    public delegate void AppPlayerUpdateCustomDataResponseHandler (int errorCode,NewCustomData meta);

    public delegate void AppPlayerGetPlayerInfoResponseHandler (int errorCode,List<UInt64> playerId);

    //排行榜
    public delegate void LeaderboardSubmitScoreResponseHandler (int errorCode);
    public delegate void LeaderboardGetListResponseHandler (int errorCode,PVPSdk.Leaderboards.LeaderboardData l);


    //广播的消息
    public delegate void RoomNewMessageBroadcastHandler (int errorCode,RoomNewMessage m);
    public delegate void OtherMemberEnterRoomBroadcastHandler (int errorCode, List<PVPSdk.Room.MemberInfo> newMembers);
    public delegate void OtherMemberLeaveRoomBroadcastHandler (int errorCode, UInt64 memberPlayerId);
    public delegate void RoomMemberCustomDataBroadcastHandler (int errorCode, NewCustomData meta);
    public delegate void RoomCustomDataBroadcastHandler (int errorCode,NewCustomData meta);


    /// <summary>
    /// 其他玩家网络抖动，正在尝试重连服务器，客户端可以尝试暂停游戏，等待对方重连，也可以继续游戏，忽略通知
    /// </summary>
    public delegate void OtherConnectionJitterBroadcastHandler(int errorCode, UInt64 otherPlayerId);

    /// <summary>
    /// 同一房间内的其他玩家网络恢复正常，广播通知，客户端接收到通知可以继续游戏，或者忽略这个通知
    /// </summary>
    public delegate void OtherConnectingRecoveryBroadcastHandler(int errorCode, UInt64 otherPlayerId);

    /// <summary>
    /// 其他玩家已经断线了，服务器清楚了断线玩家的实时信息了，客户端请进行业务处理
    /// </summary>
    public delegate void OtherConnectionLosedBroadcastHandler (int errorCode, UInt64 otherPlayerId);


    /// <summary>
    /// 在一个超过两人的比赛中（例如3人或4人的比赛），在等待其他玩家出场的过程中，会出现 
    /// 1、增加对手，
    /// 2、已匹配对手选择离开。
    /// 出现以上情况，系统会自动调用RoomMatchOpponentBroadcastHandler 委托的实例广播通知
    /// </summary>
    public delegate void RoomMatchOpponentBroadcastHandler(int errorCode, PlayerMiniInfo playerMiniInfo, UInt64 leavedPlayerId);

    /// <summary>
    /// Room match opponent finished broadcast handler.
    /// </summary>
    public delegate void RoomMatchOpponentFinishedBroadcastHandler (int errorCode);

    //网络事件
    /// <summary>
    /// 开始连接游戏服务器
    /// </summary>
    public delegate void StartConnectToGameServerEventHandler ();

    /// <summary>
    /// 连接游戏服务器成功
    /// </summary>
    public delegate void ConnectedToGameServerEventHandler (int errorCode);

    /// <summary>
    /// 连接游戏服务器失败，连接已断开
    /// </summary>
    public delegate void GameServerNetworkErrorEventHandler ();

    /// <summary>
    /// 异常委托
    /// </summary>
    public delegate void PVPSdkExceptionEventHandler(int errorCode);

    /// <summary>
    /// get friends list
    /// </summary>
    public delegate void SocialGetFriendListHandler(int errorCode, List<SocialFriend> sfs);

    /// <summary>
    /// search strange list
    /// </summary>
    public delegate void SocialSearchStrangerListHandler(int errorCode, List<SocialStranger> sss);

    public delegate void PaymentCheckGooglePlayBillingHandler(int errorCode, bool isValid, string purchaseData);

    /// <summary>
    /// Social add friend response hanlder.
    /// </summary>
    public delegate void SocialAddFriendResponseHanlder(int errorCode, UInt64 playerId);

    public delegate void SocialAcceptFriendResponseHandler(int errorCode, List<UInt64> fromPlayerId, bool isFriend);

    public delegate void SocialNewFriendNeedDeleterminiedBroadcastHandler(int errorCode, bool isAll, List<SocialStranger> ss);

    public delegate void SocialReceivedChallengeBroadcastHandler(int errorCode, SocialChallengeInfo c);

    public delegate void SocialTakeUpChallengeResponseHandler(int errorCode, string key, bool isTakeUped);

    public delegate void SocialOtherCancelChallengeBroadcastHandler(int errorCode, string key, UInt64 challengePlayerId);

    public delegate void SocialNewFriendNeedDeterminedBroadcastHandler(int errorCode, bool isAll, List<SocialStranger> ss);

    public delegate void SocialSubmitChallengeResponseHandler(int errorCode);

    public delegate void SocialAcceptFriendBroadcastHandler(int errorCode, UInt64 toPlayerId, bool isFriend);

    public delegate void SocialGetAppInfoHandler(int errorCode, PVPSdk.AppPlayerInfo info);

	public delegate void SocialCancelChallengeResponse(int errorCode);

/// <summary>
/// Social take up challenge finished broadcast handler。
/// </summary>
	public delegate void SocialTakeUpChallengeFinishedBroadcastHandler(int errorCode, String key, String challengePlayerId);
}

