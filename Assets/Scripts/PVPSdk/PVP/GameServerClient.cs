using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;
//using TcpClient = SocketEx.TcpClient;
using System.IO;
using ProtoBuf;
using PVPProtobuf_Token;
using PVPClient;

namespace PVPSdk.PVP
{

    ///// <summary>
    ///// 自定义信息接收方
    ///// </summary>
    //public class CUSTOM_MESSAGE_TARGET
    //{
    //    public const int Other = 1;
    //    public const int ALL = 2;
    //}

    /// <summary>
    /// 登陆类型
    /// </summary>
    public enum LoginType
    {
        Unknown = 0,
        /// <summary>
        /// 账号、密码登陆，暂时不提供
        /// </summary>
        Account = 1,
        /// <summary>
        /// 游客登陆
        /// </summary>
        Guest = 2,
        /// <summary>
        /// facebook 账号登录
        /// </summary>
        Facebook = 3,
    }

    /// <summary>
    /// 设备类型
    /// </summary>
    internal enum DeviceType
    {
        Android = 1,
        IOS = 2,
		Mobile = 3,
        OTHER = 99,
    }

    /// <summary>
    /// Update custom data range.
    /// </summary>
    public enum UpdateCustomDataRange
    {
        None = 0,

        /// <summary>
        /// 更新所有内容
        /// </summary>
        All = 1,

        /// <summary>
        /// 只更新发送的
        /// </summary>
        Sended = 2,
    }


    /// <summary>
    /// 链接游戏服务器，立刻返回的结果
    /// </summary>
    public enum ConnectServerOptionalResult{
        /// <summary>
        /// 需要重新登陆
        /// 请先调用 PVPSdk.Player::OpAutoLogin 或者 PVPSdk.Player::OpLoginOrRegister
        /// </summary>
        NeedLogin = 1,

        /// <summary>
        /// 开始连接服务器
        /// </summary>
        StartConnect = 2,

        /// <summary>
        /// 正在连接
        /// </summary>
        ConnectingOrChecking = 3,

        /// <summary>
        /// 已经连接成功并且验证通过
        /// </summary>
        Checked = 4,
    }
    //    #define AUTO_LOGIN_FILE_NAME userAutoLogin;

    /// <summary>
    /// PVP sdk. 对外只用访问 PVPSdk 就好，不要访问任何其他的类
    /// </summary>
    internal sealed class GameServerClient : MonoBehaviour
    {
        internal class TcpRequestTimeoutChecker
        {
            public int requestId {
                get;
                private set;
            }

            public int timeout = 0;

            public int messageTypeId {
                get;
                private set;
            }

            public global::ProtoBuf.IExtensible request {
                get;
                private set;
            }

            //            public

            public TcpRequestTimeoutChecker (int requestId, int timeout, int messageTypeId, global::ProtoBuf.IExtensible request = null)
            {
                this.timeout = timeout + 30;
                this.messageTypeId = messageTypeId;
                this.requestId = requestId;
                this.request = request;
            }
        }

        private Dictionary<int, TcpRequestTimeoutChecker> _tcpRequestTimeoutChecker = new Dictionary<int, TcpRequestTimeoutChecker> ();
        private List<int> _tmpTcpRequestTimeoutChecker = new List<int> ();

        /// <summary>
        /// The tcp client.
        /// </summary>
        private SocketClient _socketClient;

        private string _token;
        private string _ip;
        private int _port;

		private byte[] _secretKeyBytes = new byte[0];

        private Queue<ReceivedProtoEventArgs> _readQueue = new Queue<ReceivedProtoEventArgs> ();
        private static object _lock = new object ();


        private void _RaiseNetworkErrorEvent ()
        {
            Debug.Log("PVPSdk RaiseNetworkErrorEvent  " + Toolkit.GetDateTime());
			PVP.MonitorLog.GetInstance().AddLog("NE");
            if (ICM.handlerRegister != null && ICM.handlerRegister.gameServerNetworkErrorEventHandler != null) {
                ICM.handlerRegister.gameServerNetworkErrorEventHandler ();
            }
        }

		internal const string AUTO_LOGIN_FILE_NAME = "user";

        private void Awake ()
        {
            this._timeoutMessageTypeIdList.AddRange (GameServerClient._canSetTimeOutResponse);
			Toolkit.GetWritablePath();
            StartCoroutine (_RouteReceivedServerMessage ());
            StartCoroutine (_CheckTimeout ());
            DontDestroyOnLoad (gameObject);
        }

		private void Start(){
			MonitorLog.GetInstance().Submit();
		}

        public bool isConnected {
            get {
                return this._socketClient != null && this._socketClient.isConnected;
            }
        }

        private void _OnNetworkError ()
        {
            Debug.Log("PVPSdk OnNetworkError " + Toolkit.GetDateTime());
			MonitorLog.GetInstance().AddLog("ONE");
            ReceivedProtoEventArgs arg = new ReceivedProtoEventArgs ();
            arg.errorCode = ErrorCode.SUCCESS;
            arg.messageTypeId = MessageTypeId.GameServer_ConnectFailed_Notice;
            this._readQueue.Enqueue (arg);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close ()
        {
            if (this._socketClient != null) {
                this._socketClient.Close ();
                this._readQueue.Clear ();
                this._tcpRequestTimeoutChecker.Clear ();
            }
        }

        /// <summary>
        /// 自动登陆
        /// </summary>
		public bool AutoLogin (int connectTimeout, int timeout)
        {
            this.Close ();
            PVPClient.AutoLoginInfo a = Toolkit.LoadFile<PVPClient.AutoLoginInfo> (AUTO_LOGIN_FILE_NAME);

            if (a == null || a.auto_login == false) {
                return false;
            }
            LoginType loginType = (LoginType)a.login_type;
            User_AutoLogin_Request req = new User_AutoLogin_Request ();
            req.app_facebook_access_token = a.facebook_access_token;
            req.app_guest_access_token = a.guest_access_token;
            req.app_key = Config.appKey;
            req.app_login_type = a.login_type;

            switch (loginType) {
            case LoginType.Guest:
                req.app_player_id = (UInt64)a.guest_player_id;
                break;
            case LoginType.Facebook:
                req.app_player_id = (UInt64)a.facebook_player_id;
                break;
            default:
                break;
            }
            req.unique_identifier = SystemInfo.deviceUniqueIdentifier;

            HttpProtocol<User_AutoLogin_Request, User_AutoLogin_Response> httpProtocol = new HttpProtocol<User_AutoLogin_Request, User_AutoLogin_Response> ();
            httpProtocol.SetReqMsg (req);
			httpProtocol.requestId = RequestIdCreater.GetOne();
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (httpProtocol, _OnAutoLoginRes, this._secretKeyBytes, connectTimeout, timeout);
            return true;
        }

        private void _OnAutoLoginRes (Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;
            AutoLoginInfo autoLoginInfo = Toolkit.LoadFile<AutoLoginInfo> (AUTO_LOGIN_FILE_NAME);
            if(autoLoginInfo == null){
                autoLoginInfo = new AutoLoginInfo();
            }

            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                //网络出现问题
                errorCode = ErrorCode.NETWORK_ERROR;
            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<User_AutoLogin_Request, User_AutoLogin_Response> p = proto as HttpProtocol<User_AutoLogin_Request, User_AutoLogin_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        User_AutoLogin_Response res = p.GetResMsg ();
                        if (p.error_code == ErrorCode.SUCCESS && res != null && res.player_id > 0) {
                            PVPGlobal.player = new Player (res);
                            //登陆成功
                            _SetGameServerSocketInfo (res);
                            autoLoginInfo.auto_login = true;
                            autoLoginInfo.login_type = res.login_type;
							PVPGlobal.lastLoginSuccessTime = res.server_time;
                        } else {
                            autoLoginInfo.auto_login = false;
                            PVPGlobal.player = null;
                        }
                    }
                }
            }

            Toolkit.SaveFile (AUTO_LOGIN_FILE_NAME, autoLoginInfo);
            if (ICM.handlerRegister != null && ICM.handlerRegister.autoLoginOrRegisterResponseHandler != null) {
                try{
                    ICM.handlerRegister.autoLoginOrRegisterResponseHandler (errorCode);
                }catch(Exception e){
					Debug.LogWarning(e.Message);
					Debug.LogWarning(e.StackTrace);
                }
            }
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="loginType">Login type.</param>
        /// <param name="account">Account.</param>
        /// <param name="password">Password.</param>
        /// <param name="facebookAccessToken">Facebook access token.</param>
        /// <param name="timeout">Timeout.</param>
		public void LoginOrRegister (LoginType loginType, string account = "", string password = "", string facebookAccessToken = "", long facebookExpirationTime = 0, int connectTimeout = 5, int timeout = 5)
        {
            this.Close ();
            if (String.IsNullOrEmpty (Config.appKey)) {
                throw new Exception ("You have not init pvp sdk , please call PVPGlobal.Init(string appKey) and set appKey. ");
            }

            AutoLoginInfo autoLoginInfo = Toolkit.LoadFile<AutoLoginInfo> (AUTO_LOGIN_FILE_NAME);
            string appGuestAccessToken = "";
            if (autoLoginInfo != null) {
                appGuestAccessToken = autoLoginInfo.guest_access_token;
            }else{
                autoLoginInfo = new AutoLoginInfo();
            }

            PVPProtobuf_Token.User_LoginOrRegister_Request loginRequest = new PVPProtobuf_Token.User_LoginOrRegister_Request ();
            loginRequest.login_type = (Int32)loginType;
            loginRequest.account = account;
            loginRequest.password = password;
            loginRequest.unique_identifier = SystemInfo.deviceUniqueIdentifier;
            loginRequest.app_facebook_access_token = facebookAccessToken;
            loginRequest.app_guest_access_token = appGuestAccessToken;
            loginRequest.facebook_access_token_expires = facebookExpirationTime * 1000;
            autoLoginInfo.auto_login = false;
            autoLoginInfo.facebook_access_token = facebookAccessToken;

            Toolkit.SaveFile<AutoLoginInfo> (AUTO_LOGIN_FILE_NAME, autoLoginInfo);

            loginRequest.app_key = Config.appKey;

            #if UNITY_ANDROID
            loginRequest.device_type = (int)DeviceType.Android;
            #elif UNITY_IOS
            loginRequest.device_type = (int)DeviceType.IOS;
            #else
            loginRequest.device_type = (int)DeviceType.OTHER;
            #endif
			loginRequest.device_type = (int)DeviceType.Android;
            HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> loginProtocol = new HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> ();
            loginProtocol.SetReqMsg (loginRequest);
            loginProtocol.requestId = RequestIdCreater.GetOne();
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (loginProtocol, _OnLoginRes, this._secretKeyBytes, connectTimeout, timeout);
        }

        private void _OnLoginRes (Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;

            AutoLoginInfo autoLoginInfo = Toolkit.LoadFile<AutoLoginInfo> (AUTO_LOGIN_FILE_NAME);
            if(autoLoginInfo == null){
                autoLoginInfo = new AutoLoginInfo();
            }
            autoLoginInfo.auto_login = false;
            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                errorCode = ErrorCode.NETWORK_ERROR;

            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response> p = proto as HttpProtocol<User_LoginOrRegister_Request, User_LoginOrRegister_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        User_LoginOrRegister_Response res = p.GetResMsg ();
                        if (p.error_code == ErrorCode.SUCCESS && res != null && res.player_id > 0) {
                            PVPGlobal.player = new Player (res);
                            //登陆成功
                            _SetGameServerSocketInfo (res);
                            autoLoginInfo.auto_login = true;
                            autoLoginInfo.login_type = res.login_type;
                            LoginType loginType = (LoginType)res.login_type;
                            switch (loginType) {
                            case LoginType.Guest:
                                autoLoginInfo.guest_player_id = res.player_id;
                                if (res.app_guest_access_token != null && res.app_guest_access_token.Length > 0) {
                                    autoLoginInfo.guest_access_token = res.app_guest_access_token;
                                }
                                break;
                            case LoginType.Facebook:
                                autoLoginInfo.facebook_player_id = res.player_id;
								break;
                            default:
                                break;
                            }
                            foreach (string key in res.clear_keys) {
                                switch (key) {
                                case "facebook_player_id":
                                    autoLoginInfo.facebook_player_id = 0;
                                    break;
                                case "facebook_access_token":
                                    autoLoginInfo.facebook_access_token = "";
                                    break;
                                case "guest_access_token":
                                    autoLoginInfo.guest_access_token = "";
                                    break;
                                case "guest_player_id":
                                    autoLoginInfo.guest_player_id = 0;
                                    break;
                                default:
                                    break;
                                }
                            }
							PVPGlobal.lastLoginSuccessTime = res.server_time;
                        } else {
                            PVPGlobal.player = null;        
                        }
                    }
                }
            }

            Toolkit.SaveFile (AUTO_LOGIN_FILE_NAME, autoLoginInfo);
            if (ICM.handlerRegister != null && ICM.handlerRegister.loginOrRegisterResponseHandler != null) {
                try{
                    ICM.handlerRegister.loginOrRegisterResponseHandler (errorCode);
                }catch(Exception e){
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }               
            }
        }

		public bool SocialSearchStranger (string searchString, int connectTimeout = 5, int timeout = 5)
        {
            PVPProtobuf_Token.Social_SearchStranger_Request searchRequest = new PVPProtobuf_Token.Social_SearchStranger_Request ();
            searchRequest.search_str = searchString;

            HttpProtocol<Social_SearchStranger_Request, Social_SearchStranger_Response> socialSearchProtocol = new HttpProtocol<Social_SearchStranger_Request, Social_SearchStranger_Response> ();
            socialSearchProtocol.SetReqMsg (searchRequest);

            socialSearchProtocol.requestId = RequestIdCreater.GetOne();
            socialSearchProtocol.token = this._token;
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (socialSearchProtocol, _OnSocialSearchRes, this._secretKeyBytes, connectTimeout, timeout);
            return true;
        }

        private void _OnSocialSearchRes (Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;

            List<SocialStranger> socialSearchStrangerList = null;
            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                errorCode = ErrorCode.NETWORK_ERROR;

            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<Social_SearchStranger_Request, Social_SearchStranger_Response> p = proto as HttpProtocol<Social_SearchStranger_Request, Social_SearchStranger_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        Social_SearchStranger_Response res = p.GetResMsg ();
                        if (p.error_code == ErrorCode.SUCCESS && res != null) {
                            socialSearchStrangerList = new List<SocialStranger> ();
                            foreach (PVPProtobuf_Token.Social_Stranger ss in res.strangers) {
                                socialSearchStrangerList.Add (new SocialStranger (ss));
                            }
                        }
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.socialSearchStrangerListHandler != null) {
                try{
                    ICM.handlerRegister.socialSearchStrangerListHandler (errorCode, socialSearchStrangerList);
                }catch(Exception e){
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }
            }
        }

		public void OpPaymentCheckGooglePlayBilling(string inAppPurchaseData, string inAppDataSignature, int connectTimeout = 5, int timeout = 5)
        {
            PVPProtobuf_Token.Payment_CheckGooglePlayBilling_Request req = new PVPProtobuf_Token.Payment_CheckGooglePlayBilling_Request ();
            req.inapp_purchase_data = inAppPurchaseData;
            req.inapp_data_signature = inAppDataSignature;
            req.app_key = PVP.Config.appKey;

            HttpProtocol<Payment_CheckGooglePlayBilling_Request, Payment_CheckGooglePlayBilling_Response> req_protocol = new HttpProtocol<Payment_CheckGooglePlayBilling_Request, Payment_CheckGooglePlayBilling_Response> ();
            req_protocol.SetReqMsg (req);
            req_protocol.requestId = RequestIdCreater.GetOne();
            req_protocol.token = this._token;
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (req_protocol, _OnCheckGooglePlayBillingRes, this._secretKeyBytes, connectTimeout, timeout);
        }

        private void _OnCheckGooglePlayBillingRes(Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;
            bool success = false;
            string purchaseData= "";

            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                errorCode = ErrorCode.NETWORK_ERROR;
            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<Payment_CheckGooglePlayBilling_Request, Payment_CheckGooglePlayBilling_Response> p = proto as HttpProtocol<Payment_CheckGooglePlayBilling_Request, Payment_CheckGooglePlayBilling_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        Payment_CheckGooglePlayBilling_Response res = p.GetResMsg ();
                        if (p.error_code == ErrorCode.SUCCESS && res != null) {
							purchaseData = res.inapp_purchase_data;
                            success = (res.is_verified == 0) ? false : true;
                        }
                    }
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.paymentCheckGooglePlayBillingHandler != null) {
                try {
					ICM.handlerRegister.paymentCheckGooglePlayBillingHandler(errorCode, success, purchaseData);
				} catch(Exception e) {
                    Debug.LogError (e.Message);
                    Debug.LogError (e.StackTrace);
                }
            }
        }

        public bool SocialGetAppInfo(string playerInfoAccessToken, int connectTimeout = 5, int timeout = 5) 
        {
            PVPProtobuf_Token.Social_GetAppInfo_Request req = new PVPProtobuf_Token.Social_GetAppInfo_Request ();
            req.player_info_access_token = playerInfoAccessToken;

            HttpProtocol<Social_GetAppInfo_Request, Social_GetAppInfo_Response> req_protocol = new HttpProtocol<Social_GetAppInfo_Request, Social_GetAppInfo_Response> ();
            req_protocol.SetReqMsg (req);
            req_protocol.requestId = RequestIdCreater.GetOne();
            req_protocol.token = this._token;
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (req_protocol, _OnSocialGetAppInfoRes, this._secretKeyBytes, connectTimeout, timeout );
            return true;
        }

        private void _OnSocialGetAppInfoRes(Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;

            PVPSdk.AppPlayerInfo info = null;
            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                errorCode = ErrorCode.NETWORK_ERROR;
            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<Social_GetAppInfo_Request, Social_GetAppInfo_Response> p = proto as HttpProtocol<Social_GetAppInfo_Request, Social_GetAppInfo_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        Social_GetAppInfo_Response res = p.GetResMsg ();
                        if (p.error_code == ErrorCode.SUCCESS && res != null) {
                            info = new PVPSdk.AppPlayerInfo (res.app_player_infos);
                        }
                    }
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.socialGetAppPlayerInfoHandler != null) {
                try {
                    ICM.handlerRegister.socialGetAppPlayerInfoHandler (errorCode, info);
                } catch (Exception e) {
                    Debug.LogError (e.Message);
                    Debug.LogError (e.StackTrace);
                }
            }
        }

        private static int[] _canSetTimeOutResponse = new int[] {
//            MessageTypeId.Lobby_EnterLobby_Response, 
//            MessageTypeId.Lobby_GetLobbyList_Response, 
//            MessageTypeId.Lobby_CancelMatchOpponent_Response, 
//            MessageTypeId.Room_LeaveRoom_Response,
//            MessageTypeId.Room_UpdateMemberCustomData_Response,
            MessageTypeId.Room_UpdateRoomCustomData_Response,
//            MessageTypeId.Room_EnterRoom_Response,
//            MessageTypeId.AppUser_GetInfo_Response,
//            MessageTypeId.AppUser_UpdateCustomData_Response,
//            MessageTypeId.AppUser_UpdateInfo_Response,
//            MessageTypeId.Leaderboard_GetList_Response, 
//            MessageTypeId.Leaderboard_SubmitScore_Response,
            MessageTypeId.Room_NewMessage_Response,
//            MessageTypeId.Lobby_MatchOpponent_Response,
        };

        private List<int> _timeoutMessageTypeIdList = new List<int> ();
//		private NetworkReachability _oldNetworkReachability ;
		private NetworkReachability _networkReachability = NetworkReachability.NotReachable;
		internal NetworkReachability networkReachability{
			get{
				return this._networkReachability;
			}
		}

        /// <summary>
        /// 只路由长连接接收到的数据
        /// 路由接收到的数据
        /// </summary>
        /// <returns>The read message.</returns>
        private IEnumerator _RouteReceivedServerMessage ()
        {
            while (true) {
				if(_networkReachability != Application.internetReachability){
//					_oldNetworkReachability = _networkReachability;
					_networkReachability = Application.internetReachability;
					if(this._socketClient != null){
						this._socketClient.OnStateChange(_networkReachability);
					}
				}

                while (this._readQueue.Count > 0) {
                    ReceivedProtoEventArgs e = this._readQueue.Dequeue ();

                    TcpRequestTimeoutChecker t = null;
                    if (this._timeoutMessageTypeIdList.Contains (e.messageTypeId)) {
                        if (!this._tcpRequestTimeoutChecker.ContainsKey (e.requestId)) {
                        }else{
                            t = this._tcpRequestTimeoutChecker [e.requestId];
                            this._tcpRequestTimeoutChecker.Remove (e.requestId);
                        }
                    }
                    if (this._CheckErrorCode (e.errorCode)) {
                        continue;
                    }
                    switch (e.messageTypeId) {
                    case MessageTypeId.Lobby_GetLobbyList_Response:
                        this._RaiseLobbyGetLobbyListResponse (e);
                        break;
                    case MessageTypeId.Lobby_EnterLobby_Response:
                        this._RaiseEnterLobbyResponse (e);
                        break;
                    case MessageTypeId.Socket_CheckToken_Response:
						this._RaiseCheckTokenResponse (e);
                        break;
                    case MessageTypeId.Room_OtherMemberEnterRoom_Broadcast:
                        this._RaiseOtherMemberEnterRoomBroadcast (e);
                        break;
                    case MessageTypeId.Room_OtherMemberLeaveRoom_Broadcast:
                        this._RaiseOtherMemberLeaveRoomBroadcast (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Response:
                        this._RaiseUpdateRoomCustomDataResponse (e);
                        break;
                    case MessageTypeId.Room_UpdateRoomCustomData_Broadcast:
                        this._RaiseUpdateRoomCustomDataBroadcast (e);
                        break;
                    case MessageTypeId.Room_LeaveRoom_Response:
                        this._RaiseLeaveRoomResponse (e);
                        break;
                    case MessageTypeId.Lobby_MatchOpponent_Response:
                        this._RaiseMatchOpponentResponse (e);
                        break;
                    case MessageTypeId.Connection_OtherConnectionJitter_Broadcast:
                        this._RaiseOtherConnectionJitterBroadcast (e);
                        break;
                    case MessageTypeId.Connection_OtherConnectingRecovery_Broadcast:
                        this._RaiseOtherConnectionRecoveryBroadcast (e);
                        break;
                    case MessageTypeId.Connection_OtherConnectionLosed_Broadcast:
                        this._RaiseOtherConnectionLosedBroadcast (e);
                        break;
                    case MessageTypeId.GameServer_ConnectFailed_Notice:
                        this._RaiseNetworkErrorEvent ();
                        break;
                    case MessageTypeId.GameServer_StartConnect_Notice:
                        this._RaiseGameServerStartConnectEvent ();
                        break;
                    case MessageTypeId.Room_UpdateMemberCustomData_Response:
                        this._RaiseRoomUpdateMemberCustomDataResponseEvent (e);
                        break;
                    case MessageTypeId.Room_UpdateMemberCustomData_Broadcast:
                        this._RaiseRoomUpdateMemberCustomDataBroadcastEvent (e);
                        break;
                    case MessageTypeId.Lobby_CancelMatchOpponent_Response:
                        this._RaiseRoomCancelMatchOpponentResponse (e);
                        break;
                    case MessageTypeId.AppUser_UpdateCustomData_Response:
                        this._RaiseAppUserUpdateCustomDataResponse (e);
                        break;
                    case MessageTypeId.AppUser_UpdateInfo_Response:
                        this._RaiseAppUserUpdateInfoResponse (e);
                        break;
                    case MessageTypeId.Room_NewMessage_Broadcast:
                        this._RaiseRoomNewMessageBroadcastEvent (e);
                        break;
                    case MessageTypeId.Leaderboard_SubmitScore_Response:
                        this._RaiseLeaderboardSubmitScoreEvent (e);
                        break;
                    case MessageTypeId.Room_NewMessage_Response:
                        this._RaiseRoomNewMessageResponse (t, e);
                        break;
                    case MessageTypeId.Lobby_MatchOpponent_Broadcast:
                        break;
                    case MessageTypeId.Lobby_MatchOpponentFinished_Broadcast:
                        this._RaiseRoomMatchOpponentFinishedBroadcast (e);
                        break;
                    case MessageTypeId.Social_GetFriendList_Response:
                        this._RaiseSocialGetFriendListResponse (e);
                        break;
                    case MessageTypeId.Social_AddFriend_Response:
                        this._RaiseSocialAddFriendResponse(e);
                        break;
                    case MessageTypeId.Social_ReceivedChallenge_Broadcast:
                        this._RaiseSocialReceivedChallengeBroadcast(e);
                        break;
                    case MessageTypeId.Social_AcceptFriend_Response:
                        this._RaiseSocialAcceptFriendResponse(e);
                        break;
                    case MessageTypeId.Social_OtherCancelChallenge_Broadcast:
                        this._RaiseSocialOtherCancelChallengeBroadcast(e);
                        break;
                    case MessageTypeId.Social_NewFriendNeedDeleterminied_Broadcast:
                        this._RaiseSocialGetFriendNeedDeterminedBroadcast(e);
                        break;
                    case MessageTypeId.Social_AcceptFriend_Broadcast:
                        this._RaiseSocialAcceptFriendBroadcast(e);
                        break;
					case MessageTypeId.Social_TakeUpChallengeFinished_Broadcast:
						this._RaiseSocialTakeUpChallengeFinishedBroadcast(e);
						break;
					case MessageTypeId.Social_TakeUpChallenge_Response:
						this._RaiseSocialTakeUpChallengeResponse(e);
						break;
					case MessageTypeId.Social_SubmitChallenge_Response:
						this._RaiseSocialSubmitChallengeResponse(e);
						break;
					case MessageTypeId.Social_CancelChallenge_Response:
						this._RaiseSocialCancelChallengeResponse(e);
						break;
                    default:
                        Debug.LogWarning (String.Format ("do nothing  {0}", e.messageTypeId));
                        break;
                    }
                }



                //返回一帧
                yield return 0;
            }    
        }

        private void _OnReceivedBytes (ReceivedProtoEventArgs e)
        {
            lock (_lock) {
                this._readQueue.Enqueue (e);
            }
        }

        private void _SetGameServerSocketInfo (User_AutoLogin_Response protobufMeta)
        {
            this._token = protobufMeta.token;
            this._ip = protobufMeta.ip;
            this._port = protobufMeta.port;
            this._secretKeyBytes = System.Text.Encoding.ASCII.GetBytes (protobufMeta.secret_key);
        }

        private void _SetGameServerSocketInfo (User_LoginOrRegister_Response protobufMeta)
        {
            this._token = protobufMeta.token;
            this._ip = protobufMeta.ip;
            this._port = protobufMeta.port;
            //            this._secretKey = protobufMeta.secret_key;
            this._secretKeyBytes = System.Text.Encoding.ASCII.GetBytes (protobufMeta.secret_key);
        }


        public ConnectServerOptionalResult ConnectServer ()
        {
            if (!PVPGlobal.isLogined) {
                Debug.LogError("!PVPGlobal.isLogined");
                return ConnectServerOptionalResult.NeedLogin;
            }
			if(_socketClient != null && _socketClient.IsSame(this._ip, this._port, this._token, this._secretKeyBytes) && _socketClient.IsAlive() ){
				//连接成功
				if(this._socketClient.IsNormal ){
					return ConnectServerOptionalResult.Checked;
				}
				//
				if(this._socketClient.state == SocketClientState.startConnect || this._socketClient.state == SocketClientState.Connected ){
					return ConnectServerOptionalResult.ConnectingOrChecking;
				}
			}

			if(this._socketClient != null){
				MonitorLog.GetInstance().AddLog(string.Format("cs close {0}", this._socketClient.isConnected));
				this._socketClient.Close();
				this._socketClient = null;
			}

			_socketClient = new SocketClient ();
			_socketClient.OnStateChange(this._networkReachability);
			_socketClient.receiveProtoEventHandler += new ReceivedProtoEventHandler (_OnReceivedBytes);
			_socketClient.networkErrorHandler += new NetworkErrorEventHandler (_OnNetworkError);
			_socketClient.startConnectEventHandler += new StartConnectEventHandler (delegate() {
				ReceivedProtoEventArgs arg = new ReceivedProtoEventArgs ();
				arg.errorCode = ErrorCode.SUCCESS;
				arg.messageTypeId = MessageTypeId.GameServer_StartConnect_Notice;
				this._readQueue.Enqueue (arg);
			});
            
            // 连接服务器
            this._readQueue.Clear();
			MonitorLog.GetInstance().AddLog(String.Format("cs {0} {1}", this._ip, this._port));
//			Debug.Log(String.Format("{0} {1}", this._ip, this._port));
            this._socketClient.Connect (this._ip, this._port, this._token, this._secretKeyBytes);
            return ConnectServerOptionalResult.StartConnect;
        }

        /// <summary>
		/// 进入大厅, 不推荐使用
        /// </summary>
        /// <param name="lobby_id">Lobby identifier.</param>
		public bool EnterLobby (int lobby_id, int timeout = 5)
        {
            Lobby_EnterLobby_Request enterLobby = new Lobby_EnterLobby_Request ();
            enterLobby.lobby_id = lobby_id;
            if (_socketClient.SendData<Lobby_EnterLobby_Request> (RequestIdCreater.GetOne(), MessageTypeId.Lobby_EnterLobby_Request, enterLobby)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_EnterLobby_Response);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 在房间内发送消息
        /// </summary>
        /// <param name="command_id">自定义的消息分类</param>
        /// <param name="message">消息内容</param>
        /// <param name="target_type">接收消息的玩家类型</param>
        public bool SendNewMessage (int command_id, byte[] message, int target_type, int timeout)
        {
            Room_NewMessage_Request new_message = new Room_NewMessage_Request ();
            new_message.custom_command_id = command_id;
            new_message.message = message;
            new_message.target_type = target_type;
			UInt16 requestId = RequestIdCreater.GetOne();
			if (_socketClient.SendData<Room_NewMessage_Request> (requestId, MessageTypeId.Room_NewMessage_Request, new_message)) {
				this._tcpRequestTimeoutChecker [requestId] = new TcpRequestTimeoutChecker (requestId, timeout, MessageTypeId.Room_EnterRoom_Response, new_message);
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseLobbyGetLobbyListResponse (ReceivedProtoEventArgs arg)
        {
            int errorCode = arg.errorCode;
            List<Lobby> l = null;
            if (errorCode == ErrorCode.SUCCESS) {
                l = new List<Lobby> ();
                using (MemoryStream memoryStream = new MemoryStream (arg.bytes)) {
                    memoryStream.Position = 0;
                    Lobby_GetLobbyList_Response lobby_GetLobbyList_Response = Serializer.Deserialize<Lobby_GetLobbyList_Response> (memoryStream);

                    for (int i = 0; i < lobby_GetLobbyList_Response.lobby_list.Count; i++) {
                        l.Add (new Lobby (lobby_GetLobbyList_Response.lobby_list [i]));
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.lobbyGetLobbyListResponseHandler != null) {
                ICM.handlerRegister.lobbyGetLobbyListResponseHandler (errorCode, l);
            }
        }

        /// <summary>
        /// 进入指定大厅响应
        /// </summary>
        private void _RaiseEnterLobbyResponse (ReceivedProtoEventArgs arg)
        {
            if (arg.errorCode == 0 && arg.bytes != null) {
                Lobby_EnterLobby_Response lobby_EnterLobby_Response = _Deserialize<Lobby_EnterLobby_Response> (arg.bytes);
                PVPGlobal.lobby = new Lobby (lobby_EnterLobby_Response.lobby_id, lobby_EnterLobby_Response.name);
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.enterLobbyResponseHandler != null) {
                ICM.handlerRegister.enterLobbyResponseHandler (arg.errorCode);
            }
        }

        private void _RaiseRoomNewMessageBroadcastEvent (ReceivedProtoEventArgs arg)
        {
            RoomNewMessage m = null;

            if (arg.errorCode == ErrorCode.SUCCESS && arg.bytes != null && arg.bytes.Length > 0) {
                PVPProtobuf.Room_NewMessage_Broadcast new_message = _Deserialize<Room_NewMessage_Broadcast> (arg.bytes);
                m = new RoomNewMessage (new_message);
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.roomNewMessageBroadcastHandler != null) {
                ICM.handlerRegister.roomNewMessageBroadcastHandler (arg.errorCode, m);
            }
        }

		private void _RaiseCheckTokenResponse (ReceivedProtoEventArgs arg)
        {
			if (arg.errorCode == ErrorCode.CHECK_TOKEN_ERROR) {
				PVPGlobal.room = null;
				PVPGlobal.lobby = null;
				PVPGlobal.localAppPlayer = null;
			}

			if (arg.errorCode == ErrorCode.SUCCESS && arg.bytes != null) {
                PVPProtobuf.Socket_CheckToken_Response s = _Deserialize<Socket_CheckToken_Response> (arg.bytes);
                if (s.player_info != null) {
                    PVPGlobal.localAppPlayer = new LocalAppPlayer (s.player_info);
                }else{
					Debug.Log("[pvpsdk] s.player_info == null");
				}

                if (s.lobby_info != null) {
                    PVPGlobal.lobby = new Lobby (s.lobby_info);
                } else {
                    PVPGlobal.lobby = null;        
                }

                if (s.room_info != null) {
                    PVPGlobal.room = new Room (s.room_info);
                } else {
                    PVPGlobal.room = null;        
                }
                foreach (PVPProtobuf.AppPlayerInfo appUserInfo in s.app_player_infos) {
                    if (PVPGlobal.appPlayerInfos.ContainsKey (appUserInfo.player_id)) {
                        PVPGlobal.appPlayerInfos [appUserInfo.player_id].FillAppUserInfo (appUserInfo);            
                    } else {
                        PVPGlobal.appPlayerInfos [appUserInfo.player_id] = new AppPlayerInfo (appUserInfo);            
                    }
                }
                PVPGlobal.isMatching = s.is_matching;
				this._socketClient.checkedSocketId = arg.socketId;
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.connectedToGameServerEventHandler != null) {
                ICM.handlerRegister.connectedToGameServerEventHandler (arg.errorCode);
            }
        }

        private void _RaiseOtherMemberEnterRoomBroadcast (ReceivedProtoEventArgs arg)
        {

        }

        private void _RaiseOtherMemberLeaveRoomBroadcast (ReceivedProtoEventArgs arg)
        {
            UInt64 memberPlayerId = 0;
            if (arg.errorCode == ErrorCode.SUCCESS && arg.bytes != null && arg.bytes.Length > 0) {
                Room_OtherMemberLeaveRoom_Broadcast s = _Deserialize<Room_OtherMemberLeaveRoom_Broadcast> (arg.bytes);
                memberPlayerId = s.member_player_id;
                if (PVPGlobal.isInRoom) {
                    PVPGlobal.room.OnOtherMemberLeave (memberPlayerId);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherMemberLeaveRoomBroadcastHandler != null) {
                ICM.handlerRegister.otherMemberLeaveRoomBroadcastHandler (arg.errorCode, memberPlayerId);
            }
        }

        /// <summary>
        /// 获取大厅列表
        /// </summary>
        public bool GetLobbyList (int timeout = 5)
        {
            Lobby_GetLobbyList_Request l = new Lobby_GetLobbyList_Request ();
            l.ext = 1;
            if (_socketClient.SendData<Lobby_GetLobbyList_Request> (RequestIdCreater.GetOne(), MessageTypeId.Lobby_GetLobbyList_Request, l)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_GetLobbyList_Response);
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseRoomUpdateMemberCustomDataResponseEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;
            NewCustomData customData = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                Room_UpdateMemberCustomData_Response b = _Deserialize<Room_UpdateMemberCustomData_Response> (e.bytes);
                customData = new NewCustomData (PVPGlobal.player.playerId, b.command_id, b.custom_data_number, b.updated_data, b.deleted_data);

                if (PVPGlobal.room.memberInfos.ContainsKey (customData.memberPlayerId)) {
                    PVPGlobal.room.memberInfos [customData.memberPlayerId].OnUpdateCustomData (customData);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.roomMemberCustomDataResponseHandler != null) {
                ICM.handlerRegister.roomMemberCustomDataResponseHandler (errorCode, customData);
            }
        }

        private void _RaiseRoomUpdateMemberCustomDataBroadcastEvent (ReceivedProtoEventArgs e)
        {
            int errorCode = e.errorCode;
            NewCustomData customData = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                Room_UpdateMemberCustomData_Broadcast b = _Deserialize<Room_UpdateMemberCustomData_Broadcast> (e.bytes);
                customData = new NewCustomData (b.member_player_id, b.command_id, b.custom_data_number, b.updated_data, b.deleted_data);

                if (PVPGlobal.room.memberInfos.ContainsKey (customData.memberPlayerId)) {
                    PVPGlobal.room.memberInfos [customData.memberPlayerId].OnUpdateCustomData (customData);
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.roomMemberCustomDataBroadcastHandler != null) {
                ICM.handlerRegister.roomMemberCustomDataBroadcastHandler (errorCode, customData);
            }
        }

        private static T _Deserialize<T> (byte[] bytes)
        {
            MemoryStream memoryStream = new MemoryStream (bytes);
            memoryStream.Position = 0;

            T t = Serializer.Deserialize<T> (memoryStream);
            return t;
        }


        /// <summary>
        /// 更新玩家在房间内的自定义数据
        /// </summary>
        /// <param name="update_range">全量更新还是增量更新</param>
        /// <param name="custom_data">更新的内容</param>
        /// <param name="check_data">检测的内容.</param>
        public bool UpdateMemberCustomData (int command_id, UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, int timeout = 5)
        {
            if ((being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }

            Room_UpdateMemberCustomData_Request client = new Room_UpdateMemberCustomData_Request ();
            client.update_range = (int)update_range;
            client.command_id = command_id;
            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<Room_UpdateMemberCustomData_Request> (RequestIdCreater.GetOne(), MessageTypeId.Room_UpdateMemberCustomData_Request, client)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_UpdateMemberCustomData_Response);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 更新房间的自定义内容，这个内容是所有对战玩家都可以更新的，不包括观众
        /// </summary>
        /// <param name="update_range">更新的类型.</param>
        /// <param name="custom_data">更新的内容</param>
        /// <param name="check_data">检查的内容.</param>
        public bool UpdateRoomCustomData (int command_id, UpdateCustomDataRange update_range, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, int update_server_time = 0, int timeout = 5)
        {
            if ((being_deleted_data == null || being_deleted_data.Count == 0) && (being_updated_data == null || being_updated_data.Count == 0)) {
                return false;
            }

            Room_UpdateRoomCustomData_Request client = new Room_UpdateRoomCustomData_Request ();
            client.update_range = (int)update_range;
            client.command_id = command_id;
            client.update_server_time = update_server_time;

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }
			UInt16 requestId = RequestIdCreater.GetOne();
			if (_socketClient.SendData<Room_UpdateRoomCustomData_Request> (requestId, MessageTypeId.Room_UpdateRoomCustomData_Request, client)) {
				this._tcpRequestTimeoutChecker [requestId] = new TcpRequestTimeoutChecker (requestId, timeout, MessageTypeId.Room_UpdateRoomCustomData_Response);
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 在入口处统一检查错误码，针对不在大厅和不在房间 做统一清除
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <returns>
        /// bool True 中断后面的数据处理
        /// False 继续处理
        /// </returns>
        private bool _CheckErrorCode (int errorCode)
        {
            //            if (errorCode == ErrorCode.CHECK_SUM_ERROR) {
            //                if (ICM.handlerRegister.pvpSdkExceptionEventHandler != null) {
            //                    ICM.handlerRegister.pvpSdkExceptionEventHandler (errorCode);
            //                }
            //                return true;
            //            }

            if (errorCode == ErrorCode.LOBBY_NOT_IN_LOBBY_ERROR || errorCode == ErrorCode.LOBBY_ID_NOT_EXIST_ERROR) {
                PVPGlobal.lobby = null;
                PVPGlobal.room = null;
            }
            if (errorCode == ErrorCode.ROOM_NOT_IN_ROOM_ERROR) {
                PVPGlobal.room = null;
            }
            return false;

        }

        private void _RaiseUpdateRoomCustomDataResponse (ReceivedProtoEventArgs e)
        {
            NewCustomData d = null;

            if (e.bytes != null && e.bytes.Length > 0) {
                Room_UpdateRoomCustomData_Response u = _Deserialize<Room_UpdateRoomCustomData_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    //d = new RoomCustomData ();
                    d = new NewCustomData (PVPGlobal.player.playerId, u.command_id, u.custom_data_number, u.updated_data, u.deleted_data, u.round_server_time, u.server_time);
                } else {
                    d = new NewCustomData (PVPGlobal.player.playerId, u.command_id, u.custom_data_number, u.check_data, u.check_data_not_exist, u.round_server_time, u.server_time);
                }
                PVPGlobal.room.OnUpdateCustomData (d);
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.updateRoomCustomDataResponseHandler != null) {
                ICM.handlerRegister.updateRoomCustomDataResponseHandler (e.errorCode, d);
            }
        }

        private void _RaiseUpdateRoomCustomDataBroadcast (ReceivedProtoEventArgs e)
        {
            NewCustomData d = null;
            if (e.errorCode == ErrorCode.SUCCESS) {
                Room_UpdateRoomCustomData_Broadcast u = _Deserialize<Room_UpdateRoomCustomData_Broadcast> (e.bytes);
                d = new NewCustomData (u.member_player_id, u.command_id, u.custom_data_number, u.updated_data, u.deleted_data, u.round_server_time, u.server_time);
                PVPGlobal.room.OnUpdateCustomData (d);
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.updateRoomCustomDataBroadcastHandler != null) {
                ICM.handlerRegister.updateRoomCustomDataBroadcastHandler (e.errorCode, d);
            }
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        public bool LeaveRoom (int timeout = 5)
        {
            Room_LeaveRoom_Request client = new Room_LeaveRoom_Request ();
            client.ext = 1;
            if (_socketClient.SendData<PVPProtobuf.Room_LeaveRoom_Request> (RequestIdCreater.GetOne(), MessageTypeId.Room_LeaveRoom_Request, client)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Room_LeaveRoom_Response));
                return true;
            } else {
                return false;
            }
        }

        public bool SocialGetFriendList (int timeout = 5)
        {
            if (_socketClient.SendData (RequestIdCreater.GetOne(), MessageTypeId.Social_GetFriendList_Request)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Social_GetFriendList_Response));
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialGetFriendListResponse (ReceivedProtoEventArgs e)
        {
            List<SocialFriend> socialFriendList = null;

            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                PVPProtobuf.Social_GetFriendList_Response res = _Deserialize<PVPProtobuf.Social_GetFriendList_Response> (e.bytes);
			
                    socialFriendList = new List<SocialFriend>();
                    foreach (PVPProtobuf.Social_Friend sf in res.friends) {
                        socialFriendList.Add (new SocialFriend (sf));                
                    }
                 
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.socialGetFriendListHandler != null) {
                ICM.handlerRegister.socialGetFriendListHandler (e.errorCode, socialFriendList);
            }
        }


        /// <summary>
        /// Raises the leave room event.
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseLeaveRoomResponse (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                PVPGlobal.room = null;
                if (e.bytes != null && e.bytes.Length > 0) {
                    Room_LeaveRoom_Response r = _Deserialize<Room_LeaveRoom_Response> (e.bytes);
                    if (r.lobby_id == 0) {
                        PVPGlobal.lobby = null;
                    }
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.leaveRoomResponseHandler != null) {
                ICM.handlerRegister.leaveRoomResponseHandler (e.errorCode);
            }
        }



        /// <summary>
        /// 匹配成功对手后返回对手信息
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseMatchOpponentResponse (ReceivedProtoEventArgs e)
        {
            bool isMatchFinished = false;
            List<PlayerMiniInfo> playerMiniInfos = null;
            if (e.errorCode == ErrorCode.SUCCESS || e.errorCode == ErrorCode.LOBBY_MATCH_OPPONENT_ALREADY_IN_ROOM) {
                if (e.bytes != null && e.bytes.Length > 0) {
                    Lobby_MatchOpponent_Response s = _Deserialize<Lobby_MatchOpponent_Response> (e.bytes);
                    if (s.lobby_id > 0) {
                        if (PVPGlobal.lobby == null) {
                            PVPGlobal.lobby = new Lobby (s.lobby_id, s.lobby_name);
                        } else {
                            PVPGlobal.lobby.Set (s.lobby_id, s.lobby_name);        
                        }
                    }
                    isMatchFinished = s.is_match_finished;
                    if (isMatchFinished) {
                        PVPGlobal.isMatching = false;
                        PVPGlobal.room = new Room (s.room_info);
                        foreach (PVPProtobuf.AppPlayerInfo appUserInfo in s.app_player_infos) {
                            if (appUserInfo.player_id == PVPGlobal.player.playerId) {
                                PVPGlobal.localAppPlayer.FillAppUserInfo (appUserInfo);        
                            } else {
                                if (PVPGlobal.appPlayerInfos.ContainsKey (appUserInfo.player_id)) {
                                    PVPGlobal.appPlayerInfos [appUserInfo.player_id].FillAppUserInfo (appUserInfo);            
                                } else {
                                    PVPGlobal.appPlayerInfos [appUserInfo.player_id] = new AppPlayerInfo (appUserInfo);            
                                }

                            }
                        }
                    } else {
                        PVPGlobal.isMatching = true;
                        playerMiniInfos = new List<PlayerMiniInfo> ();
                        foreach (PVPProtobuf.PlayerMiniInfo playerMiniInfo in s.opponent_mini_info) {
                            playerMiniInfos.Add (new PlayerMiniInfo (playerMiniInfo));
                        }
                    }
                }
            }

            if (ICM.handlerRegister.roomMatchOpponentResponseHandler != null) {
                ICM.handlerRegister.roomMatchOpponentResponseHandler (e.errorCode, isMatchFinished, playerMiniInfos);
            }
        }

        /// <summary>
        /// 匹配对手
        /// </summary>
        /// <param name="standard">匹配的邓丽或者分数值.</param>
        /// <param name="range">匹配范围.</param>
        /// <param name="waitForMatch">等待匹配的时间， 0 永久等待</param>
        public bool MatchOpponent (int lobbyId, int standard, int rangeDown = -1, int rangeUp = -1, int waitForMatch = 0, int clusterId = 0,  int versionId = 0, int timeout = 5)
        {
            if (rangeDown >= 0 && rangeUp >= 0 && rangeDown > rangeUp) {
                return false;
            }
            Lobby_MatchOpponent_Request client = new Lobby_MatchOpponent_Request ();
            client.standard = standard;
            client.rangeDown = rangeDown;
            client.rangeUp = rangeUp;
            client.wait_for_match = waitForMatch;
            client.lobby_id = lobbyId;
            client.wait_for_match = waitForMatch;
            client.cluster_id = clusterId;
            client.version_id = versionId;
            if (_socketClient.SendData<Lobby_MatchOpponent_Request> (RequestIdCreater.GetOne(), MessageTypeId.Lobby_MatchOpponent_Request, client)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_MatchOpponent_Response));
                return true;
            } else {
                return false;
            }
        }

        void OnDestroy ()
        {
            if (this._socketClient != null) {
                this._socketClient.Close ();
            }
        }

        private void _RaiseOtherConnectionJitterBroadcast (ReceivedProtoEventArgs e)
        {
            UInt64 otherPlayerId = 0;
            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                Connection_OtherConnectionJitter_Broadcast client = _Deserialize<Connection_OtherConnectionJitter_Broadcast> (e.bytes);
                otherPlayerId = client.player_id;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherConnectionJitterBroadcastHandler != null) {
                ICM.handlerRegister.otherConnectionJitterBroadcastHandler (e.errorCode, otherPlayerId);
            }
        }

        private void _RaiseOtherConnectionRecoveryBroadcast (ReceivedProtoEventArgs e)
        {
            UInt64 otherPlayerId = 0;
            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                Connection_OtherConnectingRecovery_Broadcast client = _Deserialize<Connection_OtherConnectingRecovery_Broadcast> (e.bytes);
                otherPlayerId = client.player_id;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherConnectingRecoveryBroadcastHandler != null) {
                ICM.handlerRegister.otherConnectingRecoveryBroadcastHandler (e.errorCode, otherPlayerId);
            }
        }

        private void _RaiseOtherConnectionLosedBroadcast (ReceivedProtoEventArgs e)
        {
            UInt64 otherPlayerId = 0;
            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                Connection_OtherConnectionLosed_Broadcast client = _Deserialize<Connection_OtherConnectionLosed_Broadcast> (e.bytes);
                otherPlayerId = client.player_id;
                if (PVPGlobal.room != null) {
                    PVPGlobal.room.DelMember (otherPlayerId);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.otherConnectionLosedBroadcastHandler != null) {
                ICM.handlerRegister.otherConnectionLosedBroadcastHandler (e.errorCode, otherPlayerId);
            }
        }

        private void _RaiseGameServerStartConnectEvent ()
        {
            Debug.Log("PVPSdk RaiseGameServerStartConnectEvent   " + Toolkit.GetDateTime());
            if (ICM.handlerRegister != null && ICM.handlerRegister.startConnectToGameServerEventHandler != null) {
                ICM.handlerRegister.startConnectToGameServerEventHandler ();
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns><c>true</c> if this instance cancel match opnnent; otherwise, <c>false</c>.</returns>
        public bool CancelMatchOpnnent (int timeout = 5)
        {
            if (_socketClient.SendData ((RequestIdCreater.GetOne()), MessageTypeId.Lobby_CancelMatchOpponent_Request)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = (new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Lobby_CancelMatchOpponent_Response));
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 记录跳出的一瞬间 是否 还在连接中
        /// </summary>
        private bool _onPause_keepconnect;


		/// <summary>
		/// Closes the connect.
		/// </summary>
		/// <returns>The connect.</returns>
		private void _CloseConnect(){
			_onPause_keepconnect = false;
			if (this._socketClient != null) {
				_onPause_keepconnect = this._socketClient.keepConnecting;
				this._socketClient.Close ();
				MonitorLog.GetInstance().AddLog("AF CL");
			}
		}

//        #if !UNITY_EDITOR
		void OnApplicationFocus (bool focus_status)
		{
			lock (_lock) {
				if (focus_status) {
					CancelInvoke("_CloseConnect");
					if (this._socketClient != null) {
						if (this._onPause_keepconnect) {
							this._socketClient.Connect (this._ip, this._port, this._token, this._secretKeyBytes);
						}
					}
				} else {
					CancelInvoke("_CloseConnect");
					Invoke("_CloseConnect", 5);
				}
			}
		}
//		#endif

        /// <summary>
        /// 匹配到对手
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseRoomCancelMatchOpponentResponse (ReceivedProtoEventArgs e)
        {
            if (ICM.handlerRegister != null && ICM.handlerRegister.roomCancelMatchOpponentResonseHandler != null) {
                PVPGlobal.isMatching = false;
                ICM.handlerRegister.roomCancelMatchOpponentResonseHandler (e.errorCode);
            }
        }


        private void _RaiseAppUserUpdateCustomDataResponse (ReceivedProtoEventArgs e)
        {
            NewCustomData meta = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                AppPlayer_UpdateCustomData_Response r = _Deserialize<AppPlayer_UpdateCustomData_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    meta = new NewCustomData (PVPGlobal.player.playerId, r.command_id, r.custom_data_number, r.updated_data, r.deleted_data); 
                    PVPGlobal.localAppPlayer.OnUpdateCustomData (r.custom_data_number, r.updated_data, r.deleted_data);
                } else {
                    meta = new NewCustomData (PVPGlobal.player.playerId, r.command_id, r.custom_data_number, r.check_data, r.check_data_not_exist);
                    PVPGlobal.localAppPlayer.OnUpdateCustomData (r.custom_data_number, r.check_data, r.check_data_not_exist);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.appPlayerUpdateCustomDataResponseHandler != null) {
                ICM.handlerRegister.appPlayerUpdateCustomDataResponseHandler (e.errorCode, meta);
            }
        }

        public bool UpdateAppUserInfo (int commandId, int level = -1, Int32 score = -1, int winTimes = -1, int loseTimes = -1, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, int timeout = 5)
        {

            AppPlayer_UpdateInfo_Request client = new AppPlayer_UpdateInfo_Request ();

            client.command_id = commandId;
            client.level = level;
            client.score = score;
            client.win_times = winTimes;
            client.lose_times = loseTimes;

            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }
			UInt16 requestId = RequestIdCreater.GetOne();
			if (_socketClient.SendData<AppPlayer_UpdateInfo_Request> (requestId, MessageTypeId.AppUser_UpdateInfo_Request, client)) {
				this._tcpRequestTimeoutChecker [requestId] = new TcpRequestTimeoutChecker (requestId, timeout, MessageTypeId.AppUser_UpdateInfo_Response);
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseAppUserUpdateInfoResponse (ReceivedProtoEventArgs e)
        {
            UpdateAppPlayerInfoResult info = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                AppPlayer_UpdateInfo_Response r = _Deserialize<AppPlayer_UpdateInfo_Response> (e.bytes);
                info = new UpdateAppPlayerInfoResult (e.errorCode, r);
                PVPGlobal.localAppPlayer.OnUpdateInfo (info);
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.appPlayerUpdateInfoResponseHandler != null) {
                ICM.handlerRegister.appPlayerUpdateInfoResponseHandler (e.errorCode, info);
            }
        }

        public bool UpdateAppUserCustomData (int commandId, Dictionary<String, byte[]> being_updated_data = null, List<string> being_deleted_data = null, Dictionary<String, byte[]> check_data = null, List<string> check_data_not_exits = null, int timeout = 5)
        {
            AppPlayer_UpdateCustomData_Request client = new AppPlayer_UpdateCustomData_Request ();
            client.command_id = commandId;
            if (being_updated_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.being_updated_data.Add (p);
                }
            }

            if (being_deleted_data != null) {
                client.being_deleted_data.AddRange (being_deleted_data);
            }

            if (check_data != null) {
                foreach (KeyValuePair<string, byte[]> item in being_updated_data) {
                    PVPProtobuf.Pair p = new PVPProtobuf.Pair ();
                    p.key = item.Key;
                    p.value = item.Value;
                    client.check_data.Add (p);
                }
            }
            if (check_data_not_exits != null) {
                client.check_data_not_exist.AddRange (check_data_not_exits);
            }

            if (_socketClient.SendData<AppPlayer_UpdateCustomData_Request> (RequestIdCreater.GetOne(), MessageTypeId.AppUser_UpdateCustomData_Request, client)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.AppUser_UpdateCustomData_Response);
                return true;
            } else {
                return false;
            }
        }

        public bool AppUserGetUserInfo (int timeout = 5)
        {
            if (_socketClient.SendData ((RequestIdCreater.GetOne()), MessageTypeId.AppUser_GetInfo_Request)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.AppUser_GetInfo_Response);
                return true;
            } else {
                return false;
            }
        }


        private void _RaiseAppUserGetCustomDataResponse (ReceivedProtoEventArgs e)
        {
            List<UInt64> players = null;
            if (e.bytes != null && e.bytes.Length > 0) {
                AppPlayer_GetPlayerInfo_Response r = _Deserialize<AppPlayer_GetPlayerInfo_Response> (e.bytes);
                if (e.errorCode == ErrorCode.SUCCESS) {
                    players = new List<UInt64> ();
                    foreach (PVPProtobuf.AppPlayerInfo item in r.player_infos) {
                        players.Add (item.player_id);
                        if (item.player_id == PVPGlobal.player.playerId) {
                            PVPGlobal.localAppPlayer.FillAppUserInfo (item);
                        } else {
                            if (PVPGlobal.appPlayerInfos.ContainsKey (item.player_id)) {
                                PVPGlobal.appPlayerInfos [item.player_id].FillAppUserInfo (item);                
                            } else {
                                PVPGlobal.appPlayerInfos [item.player_id] = new AppPlayerInfo (item);                
                            }
                        }
                    }
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.appPlayerGetPlayerInfoResponseHandler != null) {
                ICM.handlerRegister.appPlayerGetPlayerInfoResponseHandler (e.errorCode, players);
            }
        }

		public bool LeaderboardGetList (string id, PVPSdk.Leaderboards.SpatialDimension spatialDimension, PVPSdk.Leaderboards.TimeDimension timeDimension, int offset = -1, int length = -1, int connectTimeout = 5, int timeout = 5)
        {
            PVPProtobuf_Token.Leaderboard_GetList_Request r = new Leaderboard_GetList_Request ();
            r.meta = new PVPProtobuf_Token.Leaderboard_Meta ();
            r.meta.id = id;
            r.meta.spatial_dimension = (int)spatialDimension;
            r.meta.time_dimension = (int)timeDimension;
            r.length = length;
            r.offset = offset;

            HttpProtocol<Leaderboard_GetList_Request, Leaderboard_GetList_Response> leaderboardGetList = new HttpProtocol<Leaderboard_GetList_Request, Leaderboard_GetList_Response> ();
            leaderboardGetList.SetReqMsg (r);
            leaderboardGetList.token = this._token;
            leaderboardGetList.requestId = RequestIdCreater.GetOne();
            Http.HttpRequestHandler h = new Http.HttpRequestHandler ();
			h.PostRequest (leaderboardGetList, _OnLeaderboardGetListRes, this._secretKeyBytes, connectTimeout, timeout);
            return true;
        }

        private void _OnLeaderboardGetListRes (Http.HttpRequestHandler.NetworkMsgType type, string message, AbstractHttpProtocol proto)
        {
            int errorCode;
            PVPSdk.Leaderboards.LeaderboardData l = null;
            if (type == Http.HttpRequestHandler.NetworkMsgType.network) {
                //网络出现问题
                errorCode = ErrorCode.NETWORK_ERROR;
            } else {
                errorCode = ErrorCode.SERVICE_ERROR;
                if (proto != null) {
                    HttpProtocol<Leaderboard_GetList_Request, Leaderboard_GetList_Response> p = proto as HttpProtocol<Leaderboard_GetList_Request, Leaderboard_GetList_Response>;
                    if (p != null) {
                        errorCode = p.error_code;
                        Leaderboard_GetList_Response res = p.GetResMsg ();
                        if (errorCode == ErrorCode.SUCCESS && res != null) {
                            l = new PVPSdk.Leaderboards.LeaderboardData (res);
                        }
                    }
                }
            }

            if (ICM.handlerRegister != null && ICM.handlerRegister.leaderboardGetListResponseHandler != null) {
                ICM.handlerRegister.leaderboardGetListResponseHandler (errorCode, l);
            }

        }

        /// <summary>
        /// 排行榜提骄傲积分
        /// </summary>
        /// <returns><c>true</c>, if submit score was leaderboarded, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="score">Score.</param>
        /// <param name="spatialDimension">Spatial dimension.</param>
        /// <param name="timeDimension">Time dimension.</param>
        /// <param name="timeout">Timeout.</param>
        public bool LeaderboardSubmitScore (string id, UInt32 score, PVPSdk.Leaderboards.UpdateType type, int submit_mark = 0, int timeout = 5)
        {
			PVPProtobuf.Leaderboard_SubmitScore_Request r = new PVPProtobuf.Leaderboard_SubmitScore_Request ();
            r.id = id;
            r.score = score;
            r.type = (int)type;
            r.submit_mark = submit_mark;
			if (_socketClient.SendData<PVPProtobuf.Leaderboard_SubmitScore_Request> (RequestIdCreater.GetOne(), MessageTypeId.Leaderboard_SubmitScore_Request, r)) {
//                this._tcpRequestTimeoutChecker [this._RequestuestId] = new TcpRequestTimeoutChecker (this._RequestuestId, timeout, MessageTypeId.Leaderboard_SubmitScore_Response);
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseLeaderboardSubmitScoreEvent (ReceivedProtoEventArgs e)
        {
            if (ICM.handlerRegister != null && ICM.handlerRegister.leaderboardSubmitScoreResponseHandler != null) {
                ICM.handlerRegister.leaderboardSubmitScoreResponseHandler (e.errorCode);
            }
        }

        /// <summary>
        /// Raises the room match opponent broadcast.
        /// </summary>
        /// <param name="e">E.</param>
        private void _RaiseRoomMatchOpponentBroadcast (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS) {
                if (e.bytes != null && e.bytes.Length > 0) {
                    Lobby_MatchOpponent_Broadcast r = _Deserialize<  Lobby_MatchOpponent_Broadcast> (e.bytes);
                    if (ICM.handlerRegister != null && ICM.handlerRegister.roomMatchOpponentBroadcastHandler != null) {

                        PlayerMiniInfo l = null;
                        if (r.opponent_mini_info != null) { 
                            l = new PlayerMiniInfo (r.opponent_mini_info);
                        }
                        ICM.handlerRegister.roomMatchOpponentBroadcastHandler (e.errorCode, l, r.leave_player_id);
                    }
                }
            }
        }

        private void _RaiseRoomMatchOpponentFinishedBroadcast (ReceivedProtoEventArgs e)
        {
            if (e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                Lobby_MatchOpponentFinished_Broadcast s = _Deserialize<Lobby_MatchOpponentFinished_Broadcast> (e.bytes);
                PVPGlobal.room = new Room (s.room_info);
                foreach (PVPProtobuf.AppPlayerInfo appUserInfo in s.app_player_infos) {
                    if (appUserInfo.player_id == PVPGlobal.player.playerId) {
                        PVPGlobal.localAppPlayer.FillAppUserInfo (appUserInfo);        
                    } else {
                        if (PVPGlobal.appPlayerInfos.ContainsKey (appUserInfo.player_id)) {
                            PVPGlobal.appPlayerInfos [appUserInfo.player_id].FillAppUserInfo (appUserInfo);            
                        } else {
                            PVPGlobal.appPlayerInfos [appUserInfo.player_id] = new AppPlayerInfo (appUserInfo);            
                        }
                    }
                }
            }
            PVPGlobal.isMatching = false;
            if (ICM.handlerRegister != null && ICM.handlerRegister.roomMatchOpponentFinishedBroadcastHandler != null) {
                ICM.handlerRegister.roomMatchOpponentFinishedBroadcastHandler (e.errorCode);
            }
        }

        private void _RaiseRoomNewMessageResponse (TcpRequestTimeoutChecker t, ReceivedProtoEventArgs e)
        {
            RoomNewMessage roomNewMessage = null;
            if (t != null && t.request != null) {
                PVPProtobuf.Room_NewMessage_Request r = t.request as PVPProtobuf.Room_NewMessage_Request;
                if (r != null && PVPGlobal.player != null) {
                    roomNewMessage = new RoomNewMessage (PVPGlobal.player.playerId, t.request as PVPProtobuf.Room_NewMessage_Request);
                }
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.roomNewMessageResponseHandler != null) {
                ICM.handlerRegister.roomNewMessageResponseHandler (e.errorCode, roomNewMessage);
            }
        }

		private UInt16 _PingPongLogInterval = 0;
		private UInt16 _submitLogInterval = 0;

        /// <summary>
        /// 检测请求数据的超时
        /// </summary>
        /// <returns>The timeout.</returns>
        private IEnumerator _CheckTimeout ()
        {
            float t = 0;
            TcpRequestTimeoutChecker c = null;
            while (true) {
                if (this._tcpRequestTimeoutChecker.Count > 0) {
                    this._tmpTcpRequestTimeoutChecker.Clear ();
                    t = Time.deltaTime;
                    foreach (KeyValuePair<int,TcpRequestTimeoutChecker> item in this._tcpRequestTimeoutChecker) {
                        c = item.Value;
                        c.timeout -= 2;

                        if (c.timeout <= 0) {
                            this._tmpTcpRequestTimeoutChecker.Add (item.Key);
                            Debug.LogWarning(String.Format( "PVPSdk timeout and will be remove {0} {1} {2}",Toolkit.GetDateTime(), c.messageTypeId, c.requestId ));
                        }
                    }

                    if (this._tmpTcpRequestTimeoutChecker.Count > 0) {
                        for (int i = 0; i < this._tmpTcpRequestTimeoutChecker.Count; i++) {
                            this._tcpRequestTimeoutChecker.Remove (this._tmpTcpRequestTimeoutChecker [i]);            
                        }
                        this._tmpTcpRequestTimeoutChecker.Clear ();
                    }
                }
				if(_PingPongLogInterval >= 3){
					MonitorLog.GetInstance().AddLog(string.Format("pp {0}", this.PingPongTime.ToString()));
					_PingPongLogInterval = 0;
				}else{
					++ _PingPongLogInterval;
				}

				if(_submitLogInterval >= 15){
					MonitorLog.GetInstance().Submit();
					_submitLogInterval = 0;
				}else{
					++ _submitLogInterval ;
				}

				yield return new WaitForSeconds (2);
            }
        }

        public bool SocialAddFriend(UInt64 toPlayerid){
            Social_AddFriend_Request client = new Social_AddFriend_Request();
            client.to_player_id = toPlayerid;

            if (_socketClient.SendData<Social_AddFriend_Request> (RequestIdCreater.GetOne(), MessageTypeId.Social_AddFriend_Request, client)) {
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialAddFriendResponse(ReceivedProtoEventArgs e){
            UInt64 toPlayerId = 0;
            if (e.bytes != null && e.bytes.Length > 0) {
                PVPProtobuf.Social_AddFriend_Response r =  _Deserialize<Social_AddFriend_Response> (e.bytes);
                toPlayerId = r.to_player_id;
            }
            if (ICM.handlerRegister != null && ICM.handlerRegister.socialAddFriendResponseHanlder != null) {
                ICM.handlerRegister.socialAddFriendResponseHanlder (e.errorCode, toPlayerId);
            }
        }

        private void _RaiseSocialReceivedChallengeBroadcast(ReceivedProtoEventArgs e){
            SocialChallengeInfo s = null;
            if(e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0){
                PVPProtobuf.Social_ReceivedChallenge_Broadcast b = _Deserialize<Social_ReceivedChallenge_Broadcast> (e.bytes);
                s = new SocialChallengeInfo(b);
                if(ICM.handlerRegister!=null && ICM.handlerRegister.socialReceivedChallengeBroadcastHandler!=null){
                    ICM.handlerRegister.socialReceivedChallengeBroadcastHandler(e.errorCode, s);
                }
            }
        }

        public bool SocialTakeUpChallenge(string key, bool isTakeUped){
            PVPProtobuf.Social_TakeUpChallenge_Request r = new Social_TakeUpChallenge_Request ();
            r.key = key;
            r.is_takeUped = isTakeUped;
            if (_socketClient.SendData<Social_TakeUpChallenge_Request> (RequestIdCreater.GetOne(), MessageTypeId.Social_TakeUpChallenge_Request, r)) {
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialOtherCancelChallengeBroadcast(ReceivedProtoEventArgs e){
            if(e.errorCode == ErrorCode.SUCCESS && e.bytes != null && e.bytes.Length > 0) {
                PVPProtobuf.Social_OtherCancelChallenge_Broadcast sb = _Deserialize<PVPProtobuf.Social_OtherCancelChallenge_Broadcast>(e.bytes);
                // where errorCode is not zero(SUCCESS), the server will not broadcast this message
                if(ICM.handlerRegister!=null && ICM.handlerRegister.socialReceivedChallengeBroadcastHandler!=null){
                    ICM.handlerRegister.socialOtherCancelChallengeBroadcastHandler(e.errorCode, sb.key, sb.challenge_player_id);
                }
            }
        }

        public bool SocialAcceptFriend(UInt64 fromPlayerId, bool isAgreed){
            PVPProtobuf.Social_AcceptFriend_Request socialAcceptFriendRequest = new Social_AcceptFriend_Request();
            socialAcceptFriendRequest.from_player_id = fromPlayerId;
            socialAcceptFriendRequest.is_agreed = isAgreed;
            if (_socketClient.SendData<Social_AcceptFriend_Request> (RequestIdCreater.GetOne(), MessageTypeId.Social_AcceptFriend_Request, socialAcceptFriendRequest)) {
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialAcceptFriendResponse(ReceivedProtoEventArgs e){
            List<UInt64> fromPlayerIds = null;
            bool isFriend = false;
            if(e.bytes!= null && e.bytes.Length > 0){
                PVPProtobuf.Social_AcceptFriend_Response s = _Deserialize<PVPProtobuf.Social_AcceptFriend_Response>(e.bytes);
                fromPlayerIds = s.from_player_ids;
                isFriend = s.is_friend;
            }
            if(ICM.handlerRegister!=null && ICM.handlerRegister.socialAcceptFriendResponseHandler!=null){
                ICM.handlerRegister.socialAcceptFriendResponseHandler(e.errorCode, fromPlayerIds, isFriend);
            }
        }

        private void _RaiseSocialAcceptFriendBroadcast(ReceivedProtoEventArgs e){
            if(e.bytes!=null && e.bytes.Length > 0){
                PVPProtobuf.Social_AcceptFriend_Broadcast s = _Deserialize<PVPProtobuf.Social_AcceptFriend_Broadcast>(e.bytes);
                if(ICM.handlerRegister!=null && ICM.handlerRegister.socialAcceptFriendResponseHandler!=null){
                    ICM.handlerRegister.socialAcceptFriendBroadcastHandler(e.errorCode, s.to_player_id, s.is_friend);
                }
            }
        }

		private void _RaiseSocialTakeUpChallengeFinishedBroadcast(ReceivedProtoEventArgs e){
			string key = "";
			string challengePlayerId = "";
			if (e.bytes!=null && e.bytes.Length > 0) {
				PVPProtobuf.Social_TakeUpChallengeFinished_Broadcast s = _Deserialize<PVPProtobuf.Social_TakeUpChallengeFinished_Broadcast>(e.bytes);
				key = s.key;
				if(e.errorCode == ErrorCode.SUCCESS && s.room_info!=null){
					PVPGlobal.isMatching = false;
					if(PVPGlobal.lobby == null){
						PVPGlobal.lobby = new Lobby(s.lobby_id, "");
					}else{
						PVPGlobal.lobby.Set(s.lobby_id, "");
					}
					PVPGlobal.room = new Room (s.room_info);
					for (int i =0 ; i<s.app_player_infos.Count; ++i){
						PVPProtobuf.AppPlayerInfo appUserInfo = s.app_player_infos[i];
						if (appUserInfo.player_id == PVPGlobal.player.playerId) {
							PVPGlobal.localAppPlayer.FillAppUserInfo (appUserInfo);        
						} else {
							if (PVPGlobal.appPlayerInfos.ContainsKey (appUserInfo.player_id)) {
								PVPGlobal.appPlayerInfos [appUserInfo.player_id].FillAppUserInfo (appUserInfo);            
							} else {
								PVPGlobal.appPlayerInfos [appUserInfo.player_id] = new AppPlayerInfo (appUserInfo);            
							}
						}
					}

				}
			}
			if(ICM.handlerRegister!=null && ICM.handlerRegister.socialTakeUpChallengeFinishedBroadcastHandler!=null){
				ICM.handlerRegister.socialTakeUpChallengeFinishedBroadcastHandler(e.errorCode, key, challengePlayerId);
			}
		}

		private void _RaiseSocialTakeUpChallengeResponse(ReceivedProtoEventArgs e){
			String key = "";
			bool isTakeUped = false;
			if(e.bytes!=null && e.bytes.Length > 0){
				PVPProtobuf.Social_TakeUpChallenge_Response r = _Deserialize<PVPProtobuf.Social_TakeUpChallenge_Response>(e.bytes);
				isTakeUped = r.is_takeUped;
				key = r.key;
			}
			if(ICM.handlerRegister!=null && ICM.handlerRegister.socialTakeUpChallengeResponseHandler!=null){
				ICM.handlerRegister.socialTakeUpChallengeResponseHandler(e.errorCode, key, isTakeUped);
			}
		}

        public bool SocialGetFriendNeedDetermined(){
            if (_socketClient.SendData (RequestIdCreater.GetOne(), MessageTypeId.Social_GetFriendNeedDetermined_Request)) {
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialGetFriendNeedDeterminedBroadcast(ReceivedProtoEventArgs e){
            bool isAll = false;
            List<SocialStranger> ss = null;
            if(e.bytes!= null && e.bytes.Length > 0){
                PVPProtobuf.Social_NewFriendNeedDeleterminied_Broadcast s = _Deserialize<PVPProtobuf.Social_NewFriendNeedDeleterminied_Broadcast>(e.bytes);
                isAll = s.is_all;
                ss = new List<SocialStranger>();
                foreach(PVPProtobuf.Social_Stranger item in s.list){
                    ss.Add(new SocialStranger(item));
                }
            }
            if(ICM.handlerRegister!=null && ICM.handlerRegister.socialNewFriendNeedDeterminedBroadcastHandler!=null){
                ICM.handlerRegister.socialNewFriendNeedDeterminedBroadcastHandler(e.errorCode, isAll, ss);
            }
        }

        public bool SocialSubmitChallenge(int lobbyId, UInt64 challengePlayerId){
            PVPProtobuf.Social_SubmitChallenge_Request r = new PVPProtobuf.Social_SubmitChallenge_Request();
            r.lobby_id = lobbyId;
            r.challenge_player_id = challengePlayerId;
            if (_socketClient.SendData<Social_SubmitChallenge_Request> (RequestIdCreater.GetOne(), MessageTypeId.Social_SubmitChallenge_Request, r)) {
                return true;
            } else {
                return false;
            }
        }

        private void _RaiseSocialSubmitChallengeResponse(ReceivedProtoEventArgs e){
            if(ICM.handlerRegister!=null && ICM.handlerRegister.socialSubmitChallengeResponseHandler!=null){
                ICM.handlerRegister.socialSubmitChallengeResponseHandler(e.errorCode);
            }
        }

		public bool SocialCancelChallenge(string key){
			PVPProtobuf.Social_CancelChallenge_Request r = new PVPProtobuf.Social_CancelChallenge_Request();
			r.key = key;
			if (_socketClient.SendData<PVPProtobuf.Social_CancelChallenge_Request> (RequestIdCreater.GetOne(), MessageTypeId.Social_CancelChallenge_Request, r)) {
				return true;
			} else {
				return false;
			}
		}

		private void _RaiseSocialCancelChallengeResponse(ReceivedProtoEventArgs e){
			if(ICM.handlerRegister!=null && ICM.handlerRegister.socialCancelChallengeResponse!=null){
				ICM.handlerRegister.socialCancelChallengeResponse(e.errorCode);
			}
		}

		internal Int64 PingPongTime{
			get{
				if(this._socketClient != null){
					return this._socketClient.PingPongTime;
				}else{
					return 0;
				}
			}
		}
    }
}