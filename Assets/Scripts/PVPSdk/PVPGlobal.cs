using UnityEngine;
using System;
using System.Collections.Generic;


/*! \mainpage pvpsdk u3d 版本在线文档
 *
 * \section 简要描述
 *
 * 零、游戏开发者拥有由对战平台分配的 appKey\n
 * \n
 * 一、PVPGlobal.Init(appKey);\n
 * \n
 * 二、绑定委托实例 \n
 * 例如用户登陆： \n
 * PVPGlobal.handlerRegister.loginOrRegisterResponseHandler += OnLoginOrRegister;\n
 *  \n
 * public void OnLoginOrRegister(int errorCode){\n
 *  // 业务逻辑代码 \n
 * }\n
 * \n
 * 三、调用PVPSdk 接口代码登陆 \n
 * 例如登陆：PVPSdk.Player.OpLoginOrRegister (…);\n
 * \n
 * \n
 * 四、连接游戏服务器 gameServer\n
 * 登陆成功后，PVPGlobal.player 会被系统自动实例化，调用 PVPGlobal.player.OpConnectGameServer(); 连接游戏服务器 \n
 * \n
 * 五、连接游戏服务器成功后，PVPGlobal.localAppUser 会被系统自动实例化，接着就可以进行匹配对手： \n
 * PVPSdk.Lobby.OpMatchOpponentWithLobbyId(…);\n
 * \n
 * 六、匹配对手成功后，PVPGlobal.room 会被系统自动实例化，接着游戏可以进行游戏逻辑处理，并广播数据 \n
 * 有\n
 * PVPGlobal.room.OpUpdateRoomCustomData(…);\n
 * PVPGlobal.room.OpUpdateMemberCustomData (…);\n
 * PVPGlobal.room.OpSendNewMessage (…);\n
 *  \n
 * 七、对战结束后，可以调用 \n
 * PVPGlobal.localAppUser.OpUpdateAppPlayerCustomData(…)\n
 * PVPGlobal.localAppUser.OpUpdateAppPlayerInfo(…);\n
 * 更新玩家信息 \n
 * \n
 * 以及上传积分： \n
 * PVPGlobal.localAppUser.OpLeaderboardSubmitScore(…)\n
 */
namespace PVPSdk {


    /// <summary>
    /// PVPSdk 全局类
    /// </summary>
    public sealed class PVPGlobal
    {
        /// <summary>
        /// sdk 版本号
        /// </summary>
        public const string VERSION = "0.2.10-p1";
		
		/// <summary>
        /// 表示用户是否已经登录
        /// </summary>
        public static bool isLogined {
            get{
                return player != null;
            }
        }

        /// <summary>
        /// 用户全局信息，如果用户没有登陆，那么这个变量是 NULL，用户登陆成功后，系统会自动填充这个字段
        /// </summary>
        public static Player player {
            get;
            internal set;
        }

        /// <summary>
        /// 当前玩家是否在一个大厅里面
        /// </summary>
        /// <value><c>true</c> if is in lobby; otherwise, <c>false</c>.</value>
        public static bool isInLobby{
            get{
                return lobby != null;
            }
        }

        /// <summary>
        /// 用户当前所在的大厅，如果用户没有进入大厅，那么 lobby 是 null
        /// </summary>
        public static Lobby lobby = null;

        public static double lastLoginSuccessTime {
            get;
            internal set;
        }

		private static bool _isMatching = false;

        /// <summary>
        /// 是否正在匹配
        /// </summary>
        /// <value><c>true</c> if is matching; otherwise, <c>false</c>.</value>
        public static bool isMatching {
            get {
                return _isMatching;
            }
            internal set {
                _isMatching = value;
            }
        }

        /// <summary>
        /// 当前玩家是否在一个房间里面
        /// </summary>
        /// <value><c>true</c> if is in room; otherwise, <c>false</c>.</value>
        public static bool isInRoom{
            get{
                return lobby != null && room != null;
            }
        }

        private static Room _room = null;
        /// <summary>
        /// 用户当前所在房间，如果用户没有进入房间，那么 room 是 null
        /// </summary>
        public static Room room {
            get {
                return _room;
            }
            internal set {
                _room = value;
            }
        }

        /// <summary>
        /// 是否连接游戏服务器并通过验证
        /// 这个只是表示曾经连接上游戏服务器，不表示当前连接成功
        /// </summary>
        public static bool connectedToGameServer {
            get{
                return localAppPlayer != null;
            }
        }


		/// <summary>
		/// 
		/// </summary>
		/// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
		public static bool isConnected{
			get{
				return PVP.ICM.gameServerClient.isConnected;
			}
		}

		/// <summary>
		/// 毫秒数
		/// </summary>
		/// <value>The socket delayed.</value>
		public static Int64 PingPongTime{
			get{
				return PVP.ICM.gameServerClient.PingPongTime;
			}
		}

        private static LocalAppPlayer _localAppPlayer = null;

        /// <summary>
        /// 用户应用内信息，例如玩家在游戏内的胜利场次、失败场次、积分、等级以及自定义的信息，
        /// 这个数据是在游戏服务器获取，所以玩家即使已经登陆， PVPGlobal.isLogin == True, 这个字段也有可能是NULL 
        /// 连接成功 gameServer 后，系统自动填充这个字段
        /// 可以通过 PVPGlobal.connectedToGameServer == True 判断PVPGlobal.localAppUser 是否有效
        /// </summary>
        public static LocalAppPlayer localAppPlayer {
            get {
                return _localAppPlayer;
            }
            internal set {
                _localAppPlayer = value;
            }
        }


        private static Dictionary<UInt64, AppPlayerInfo> _appPlayerInfos = new Dictionary<UInt64, AppPlayerInfo> ();

        /// <summary>
        /// hash 字典，
        ///      key ：UInt32，其他玩家的uid ；
        ///      value ：AppUserInfo 其他玩家的应用内信息例如其他玩家的 名称（name）、头像（avatar）、胜利场次、失败场次以及自定义消息等
        /// 当玩家匹配对手成功后，系统会自动填充对手玩家的信息到 PPVGlobal.appuserInfos 内。
        /// </summary>
        public static Dictionary<UInt64, AppPlayerInfo> appPlayerInfos {
            get {
                return _appPlayerInfos;
            }
            internal set{
                _appPlayerInfos = value;
            }
        }

        /// <summary>
        /// handlerRegister，针对对战平台的各个事件定义了一系列的委托实例
        /// 例如 handlerRegister.loginOrRegisterEventHandler 用户登录注册响应事件的委托实例，当用户进行登录操作，服务器响应后会回调 handlerRegister.loginOrRegisterEventHandler 委托实例
        /// </summary>
        /// <value>The handler register.</value>
        public static PVP.HandlerRegister handlerRegister{
            get{
                return PVP.ICM.handlerRegister;
            }
        }

        /// <summary>
        /// 初始化sdk ，在调用sdk其他函数 前必须进行初始化
        /// </summary>
        /// <param name="appKey">应用的appKey, 由对战平台配置.</param>
        public static void Init(string appKey){
            PVP.Config.appKey = appKey;
            if (appKey.EndsWith ("_qd")) {
                PVP.Config.HttpUri = PVP.Config.QdevHttpUri;
            } else {
				if(appKey.EndsWith("_al")){
					PVP.Config.HttpUri = PVP.Config.ALIHttpUri;
				}else{
                	PVP.Config.HttpUri = PVP.Config.AWSHttpUri;
				}
            }
        }


    }
}
