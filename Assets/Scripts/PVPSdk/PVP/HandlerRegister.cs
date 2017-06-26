using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
//using TcpClient = SocketEx.TcpClient;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;

namespace PVPSdk.PVP
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class HandlerRegister
    {
        public Handler.AutoLoginOrRegisterResponseHandler autoLoginOrRegisterResponseHandler ; 

        /// <summary>
        /// 用户登录（如果用户不存在就先注册后登陆），操作完成后，系统回调 loginOrRegisterEventHandler 委托实例
        /// </summary>
        public Handler.LoginOrRegisterResponHandler loginOrRegisterResponseHandler ;


        /// <summary>
        /// 游戏服务器 gameServer 使用了长连接。
        /// 在建立连接，以及保持连接过成功如果出现中断情况，都会自动尝试重连，在每次建立连接时，系统会回调 startConnectToGameServerEventHandler 委托实例
        /// 如果重连3次都不成功，系统会触发 gameServerNetworkErrorEventHandler 委托实例
        /// </summary>
        public Handler.StartConnectToGameServerEventHandler startConnectToGameServerEventHandler;

        /// <summary>
        /// 连接游戏服务器成功后，系统会回调 connectedToGameServerEventHandler 委托实例，
        /// 如果连接游戏服务器成功并且通过验证， 应用内的用户信息可以通过 PVPGlobal.localAppPlayerInfo 获取
        /// </summary>
        public Handler.ConnectedToGameServerEventHandler connectedToGameServerEventHandler;


        /// <summary>
        /// 连接 游戏服务器 网络出现异常，系统会回调 gameServerNetworkErrorEventHandler 委托实例
        /// </summary>
        public Handler.GameServerNetworkErrorEventHandler gameServerNetworkErrorEventHandler;


        /// <summary>
        /// sdk 异常情况的处理，根据不同的异常情况，会分那会不同的errorCode
        /// 2015-11-11
        /// 当数据包验证不通过，系统会回调 pvpSdkExceptionEventHandler 委托实例，并返回errorCode = ErrorCode.CHECK_SUM_ERROR，这时建议让玩家重新登录游戏，刷新数据
        ///  
        /// </summary>
//        public Handler.PVPSdkExceptionEventHandler pvpSdkExceptionEventHandler;

        /// <summary>
        /// 获取游戏服大厅列表， 系统回调 lobbyGetLobbyListResponseHandler 委托实例
        /// </summary>
        internal Handler.LobbyGetLobbyListResponHandler lobbyGetLobbyListResponseHandler;

        /// <summary>
        /// 进入指定大厅操作完成后，系统回调 enterLobbyResponseHandler 委托实例
        /// </summary>
		internal Handler.EnterLobbyResponseHandler enterLobbyResponseHandler;

        /// <summary>
        /// 
        /// </summary>
        public Handler.RoomNewMessageResponseHandler roomNewMessageResponseHandler;
       
//        /// <summary>
//        /// 获取房间列表委托实例
//        /// </summary>
//        public Handler.GetRoomListResponseHandler getRoomListResponseHandler;
//
//        public Handler.EnteredRoomResponseHandler enterRoomResponseHandler;

//        public Handler.Room

        /// <summary>
        /// 房间内某个玩家发送广播消息，房间内其他玩家接收消息，系统会回调 roomNewMessageBroadcastHandler 委托实例
        /// </summary>
        public Handler.RoomNewMessageBroadcastHandler roomNewMessageBroadcastHandler;

        /// <summary>
        /// 玩家离开房间，操作完成后系统回调 leaveRoomResponseHandler 委托实例
        /// 房间内其他玩家 会收到通知，系统回调 otherMemberLeaveRoomBroadcastHandler 委托实例
        /// </summary>
        public Handler.LeaveRoomResponseHandler leaveRoomResponseHandler;

        /// <summary>
        /// 玩家离开房间，操作完成后系统回调 leaveRoomResponseHandler 委托实例
        /// 房间内其他玩家 会收到通知，系统回调 otherMemberLeaveRoomBroadcastHandler 委托实例
        /// </summary>
        public Handler.OtherMemberLeaveRoomBroadcastHandler otherMemberLeaveRoomBroadcastHandler;

        /// <summary>
        /// 房间内某个玩家更新自定义房间数据， 更新完成后系统回调 roomMemberCustomDataResponseHandler 委托实例
        /// 房间内所有其他的玩家都会接收到通知，系统回调 roomMemberCustomDataBroadcastHandler 委托实例
        /// </summary>
        public Handler.RoomMemberCustomDataResponseHandler roomMemberCustomDataResponseHandler;

        /// <summary>
        /// 房间内某个玩家更新自定义房间数据， 更新完成后系统回调 roomMemberCustomDataResponseHandler 委托实例
        /// 房间内所有其他的玩家都会接收到通知，系统回调 roomMemberCustomDataBroadcastHandler 委托实例
        /// </summary>
        public Handler.RoomMemberCustomDataBroadcastHandler roomMemberCustomDataBroadcastHandler;


        /// <summary>
        /// 玩家更新房间内公共信息，更新成功后系统回调 updateRoomCustomDataResponseHandler 委托实例
        /// 委托的声明中，有 两个参数，
        /// public delegate void RoomCustomDataResponseHandler (int errorCode, NewCustomData meta);
        /// 如果 errorCode == ErrorCode.SUCCESS，那么 NewCustomData 会返回实际更新的内容
        /// 如果更是失败，erroCode == ErrorCode.ROOM_UPDATE_ROOM_CHECK_DATA_NOT_PASS, 那么 NewCustomData 会返回验证不通过的内容
        /// 无论是否更新成功， Room.customData 都会同步到和服务器数据一致
        /// 
        /// 
        /// 如果更新成功，房间内的其他玩家会收到广播通知，系统会回调 updateRoomCustomDataBroadcastHandler 委托实例
        /// </summary>
        public Handler.RoomCustomDataResponseHandler updateRoomCustomDataResponseHandler;

        /// <summary>
        /// 玩家更新房间内公共信息，更新成功后系统回调 updateRoomCustomDataResponseHandler 委托实例
        /// 房间内的其他玩家会收到广播通知，系统会回调 updateRoomCustomDataBroadcastHandler 委托实例
        /// </summary>
        public Handler.RoomCustomDataBroadcastHandler updateRoomCustomDataBroadcastHandler;


        /// <summary>
        /// 房间匹配对手,
        /// errorCode 表示 提交“匹配对手请求”是否成功
        /// isMatchFinished 表示是否同时匹配对手接收，如果 True，那么就表示房间创建成功，可以进行游戏。可以在PVPGlobal.room 字段获取房间详细信息
        ///                                      如果 False，那么玩家还没有匹配完毕，PlayerMiniInfos 表示已经匹配的玩家列表（包括自己）
        /// 在大厅内匹配对手玩家，匹配成功后，系统回调 roomMatchOpponentResponseHandler 委托实例
        /// 并会初始化好 对手玩家的信息，对手玩家的信息存放在 PVPGlobal.appPlayerInfos 里
        /// </summary>
        public Handler.RoomMatchOpponentResponseHandler roomMatchOpponentResponseHandler;


        /// <summary>
        /// 匹配中途的广播，只对3人或者3人以上的房间匹配才有效，
        /// 2个人的房间可以不处理这个回调
        /// </summary>
        public Handler.RoomMatchOpponentBroadcastHandler roomMatchOpponentBroadcastHandler;

        /// <summary>
        /// 匹配成功或者匹配超时的回调
        /// 玩家A 先提交匹配请求， 玩家B 后提交匹配请求，玩家A 和玩家B 匹配成功了，那么玩家A 会收到 roomMatchOpponentFinishedBroadcastHandler 回调
        /// 玩家B 会收到 roomMatchOpponentResponseHandler 回调
        /// </summary>
        public Handler.RoomMatchOpponentFinishedBroadcastHandler roomMatchOpponentFinishedBroadcastHandler;


        /// <summary>
        /// 同一房间内的其他玩家网络出现异常，不稳定，系统会回调 otherConnectionJitterBroadcastHandler 委托实例，业务层可以暂停游戏或者忽略这个通知
        /// </summary>
        public Handler.OtherConnectionJitterBroadcastHandler otherConnectionJitterBroadcastHandler;

        /// <summary>
        /// 同一房间内的其他玩家的网络恢复正常了，系统会回调 otherConnectingRecoveryBroadcastHandler 委托实例，业务层可以继续游戏或者直接忽略这个通知
        /// </summary>
        public Handler.OtherConnectingRecoveryBroadcastHandler otherConnectingRecoveryBroadcastHandler;


        /// <summary>
        /// 玩家网络出现异常导致离开了房间，房间内的玩家会接收到这个信息，系统回调 otherLoseConnectionBroadcastHandler 委托实例
        /// </summary>
        public Handler.OtherConnectionLosedBroadcastHandler otherConnectionLosedBroadcastHandler;


       

        /// <summary>
        /// 玩家发送匹配对手请求会，可以选择取消匹配
        /// 发送选择取消匹配请求后，系统完成操作，并回调 roomCancelMatchOpponentResonseHandler 委托实例
        /// </summary>
        public Handler.RoomCancelMatchOpponentResonseHandler roomCancelMatchOpponentResonseHandler;

       
        /// <summary>
        /// 玩家更新 应用内玩家的自定义信息，请求结束后，系统会回调 appPlayerUpdateCustomDataResponseHandler 委托实例
        /// </summary>
        public Handler.AppPlayerUpdateCustomDataResponseHandler appPlayerUpdateCustomDataResponseHandler;

        /// <summary>
        /// 玩家更新 应用内玩家的个人信息（包括自定义），请求结束后，系统会回调 appPlayerUpdateInfoResponseHandler 委托实例
        /// </summary>
        public Handler.AppPlayerUpdateInfoResponseHandler appPlayerUpdateInfoResponseHandler;

        /// <summary>
        /// 获取其他玩家的游戏内玩家信息，系统响应后会回调 appPlayerGetPlayerInfoResponseHandler 委托实例
        /// 如果获取成功，其他玩家的游戏内玩家信息 存放在 PVPGlobal.appPlayerInfos 字典里面
        /// </summary>
        public Handler.AppPlayerGetPlayerInfoResponseHandler appPlayerGetPlayerInfoResponseHandler;

        /// <summary>
        /// 获取排行榜列表，系统响应请求后，会回调 leaderboardGetListResponseHandler 委托实例
        /// </summary>
        public Handler.LeaderboardGetListResponseHandler leaderboardGetListResponseHandler;

        /// <summary>
        /// google play 支付验证使用
        /// </summary>
        public Handler.PaymentCheckGooglePlayBillingHandler paymentCheckGooglePlayBillingHandler;
       
        /// <summary>
        /// 提交更新排行榜玩家的分数，更新完成后，系统会回调 leaderboardSubmitScoreResponseHandler 委托实例
        /// </summary>
        public Handler.LeaderboardSubmitScoreResponseHandler leaderboardSubmitScoreResponseHandler;

        /// <summary>
        /// 获取好友列表的回调
        /// </summary>
        public Handler.SocialGetFriendListHandler socialGetFriendListHandler;

        public Handler.SocialSearchStrangerListHandler socialSearchStrangerListHandler;

        public Handler.SocialAddFriendResponseHanlder socialAddFriendResponseHanlder;

        public Handler.SocialAcceptFriendResponseHandler socialAcceptFriendResponseHandler;

        public Handler.SocialReceivedChallengeBroadcastHandler socialReceivedChallengeBroadcastHandler;

        public Handler.SocialTakeUpChallengeResponseHandler socialTakeUpChallengeResponseHandler;

        public Handler.SocialOtherCancelChallengeBroadcastHandler socialOtherCancelChallengeBroadcastHandler;

        public Handler.SocialNewFriendNeedDeterminedBroadcastHandler socialNewFriendNeedDeterminedBroadcastHandler;

        public Handler.SocialSubmitChallengeResponseHandler socialSubmitChallengeResponseHandler;

        public Handler.SocialAcceptFriendBroadcastHandler socialAcceptFriendBroadcastHandler;

		public Handler.SocialTakeUpChallengeFinishedBroadcastHandler socialTakeUpChallengeFinishedBroadcastHandler;

        public Handler.SocialGetAppInfoHandler socialGetAppPlayerInfoHandler;

		public Handler.SocialCancelChallengeResponse socialCancelChallengeResponse;
    }
}