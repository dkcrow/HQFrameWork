using System;

namespace PVPSdk.PVP
{
    public class MessageTypeId
    {

        public const int Socket_CheckToken_Request = 1;
        public const int Socket_CheckToken_Response = 2;
        public const int Socket_HeartBeat_Ping = 3;
        public const int Socket_GetServerTime_Request = 4;
        public const int Socket_GetServerTime_Response = 5;
        public const int User_LoginOrRegister_Request = 6;
        public const int User_LoginOrRegister_Response = 7;
        public const int User_AutoLogin_Request = 8;
        public const int User_AutoLogin_Response = 9;
        public const int Socket_StartConnect_Notice = 10;
        public const int Socket_HeartBeat_Pong = 11;

        public const int Lobby_GetLobbyList_Request = 201;
        public const int Lobby_GetLobbyList_Response = 202;

        public const int Lobby_EnterLobby_Request = 203;
        public const int Lobby_EnterLobby_Response = 204;

        //      // 创建房间
        //      public const int Room_CreateRoom_Request = 101;
        //      public const int Room_CreateRoom_Response = 102;

        // 进入房间
        public const int Room_EnterRoom_Request = 109;
        public const int Room_EnterRoom_Response = 110;

        //      // 列出所有可见房间
        //      public const int Room_GetRoomList_Request = 111;
        //      public const int Room_GetRoomList_Response = 112;

        // 对指定对象发消息
        //        public const int Message_CustomTargetMessage_Request = 105;
        //        public const int Message_CustomTargetMessage_Response = 106;

        // 发对房间多人发消息
        public const int Room_NewMessage_Request = 107;
        public const int Room_NewMessage_Broadcast = 108;
        //        public const int Lobby_MatchOpponentFinished_Broadcast= 148;
        //        public const int Room_NewMessage_Response = 149;
        //        public const int Lobby_MatchOpponent_Broadcast = 150 ;

        //      // 聊天
        //      public const int Chat_SendChat_Request = 103;
        //      public const int Chat_SendChat_Response = 104;
        //
        //        //房间内
        //      public const int Room_SendChat_Request = 113;
        //      public const int Room_SendChat_Response = 114;

        //      public const int Room_SendRoomCacheMessage_Request = 115;
        //      public const int Room_ReceiveRoomCacheMessage_Response = 116;

        //      public const int Room_RoomCacheMessageList_Request = 117;
        //      public const int Room_RoomCacheMessageList_Response = 118;

        public const int Room_OtherMemberEnterRoom_Broadcast = 119;

        public const int Room_NewRoomMaster_Response = 120;

        //      public const int Room_StartGame_Request = 121;
        //      public const int Room_FinishGame_Request = 122;

        public const int Room_LeaveRoom_Request = 123;
        public const int Room_LeaveRoom_Response = 124;

        public const int Room_OtherMemberLeaveRoom_Broadcast = 125;

        //      public const int Room_StartBattle_Request = 126;
        //      public const int Room_StartBattle_Response = 127;
        //
        //      public const int Room_NextTurn_Request = 128;
        //      public const int Room_NextTurn_Response = 129;

        public const int Room_UpdateMemberCustomData_Request = 130;
        public const int Room_UpdateMemberCustomData_Broadcast = 131;
        public const int Room_UpdateMemberCustomData_Response = 132;

        public const int Room_UpdateRoomCustomData_Request = 133;
        public const int Room_UpdateRoomCustomData_Broadcast = 134;
        public const int Room_UpdateRoomCustomData_Response = 135;

        //        public const int Room_EnterRandomRoom_Request = 136;
        //        public const int Room_EnterRandomRoom_Response = 137;

        public const int Lobby_MatchOpponent_Request = 138;
        public const int Lobby_MatchOpponent_Response = 139;
        public const int Connection_OtherConnectionLosed_Broadcast = 140;
        //        public const int Room_NextTurn_Broadcast = 141;
        //        public const int Room_CompleteBattle_Request = 142;
        //        public const int Room_CompleteBattle_Broadcast = 143;
        public const int Lobby_CancelMatchOpponent_Request = 144;
        public const int Lobby_CancelMatchOpponent_Response = 145;

        public const int Room_GetFriendsState_Request = 146;
        public const int Room_GetFriendsState_Response = 147;

        public const int Lobby_MatchOpponentFinished_Broadcast = 148;
        public const int Room_NewMessage_Response = 149;
        public const int Lobby_MatchOpponent_Broadcast = 150;
        public const int Connection_OtherConnectingRecovery_Broadcast = 151;
        public const int Connection_OtherConnectionJitter_Broadcast = 152;

        public const int AppUser_UpdateCustomData_Request = 301;
        public const int AppUser_UpdateCustomData_Response = 302;
        public const int AppUser_GetInfo_Request = 303;
        public const int AppUser_GetInfo_Response = 304;
        public const int AppUser_UpdateInfo_Request = 305;
        public const int AppUser_UpdateInfo_Response = 306;

        public const int Leaderboard_GetList_Request = 401;
        public const int Leaderboard_GetList_Response = 402;
        public const int Leaderboard_SubmitScore_Request = 403;
        public const int Leaderboard_SubmitScore_Response = 404;

        public const int Social_GetFriendList_Request = 501;
        public const int Social_GetFriendList_Response = 502;
        public const int Social_SubmitChallenge_Request = 503;
        public const int Social_SubmitChallenge_Response = 504;
        public const int Social_ReceivedChallenge_Broadcast = 505;
        public const int Social_TakeUpChallenge_Request = 506;
        public const int Social_TakeUpChallenge_Response = 507;
        public const int Social_TakeUpChallengeFinished_Broadcast = 508;
        public const int Social_CancelChallenge_Request = 509;
        public const int Social_CancelChallenge_Response = 510;
        public const int Social_AddFriend_Request = 511;
        public const int Social_AddFriend_Response = 512;
        public const int Social_AcceptFriend_Request = 513;
        public const int Social_AcceptFriend_Response = 514;
        public const int Social_SearchStranger_Request = 515;
        public const int Social_SearchStranger_Response = 516;
        public const int Social_GetFriendNeedDetermined_Request = 517;
        public const int Social_NewFriendNeedDeleterminied_Broadcast = 518;
        public const int Social_OtherCancelChallenge_Broadcast = 519;
        public const int Social_AcceptFriend_Broadcast = 520;
        public const int Social_GetAppInfo_Response = 521;

        public const int GameServer_ConnectFailed_Notice = 90001;
        public const int GameServer_StartConnect_Notice = 90002;
    }
}

