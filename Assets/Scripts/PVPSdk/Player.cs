using System;

using PVPProtobuf_Token;
using UnityEngine;

namespace PVPSdk
{
    /// <summary>
    /// 用户的全局信息
    /// </summary>
    public class Player
    {
        /// <summary>
        /// 用户 playerId，
        /// </summary>
        /// <value>The uid.</value>
        public UInt64 playerId{ get; private set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        /// <value>The name.</value>
        public string name{ get; private set; }

        /// <summary>
        /// 用户的国家代号，例如“CN”:中国， “US”:美国，“UK”:英国， “UNKNOW”: 未知
        /// </summary>
        /// <value>The country code.</value>
        public string countryCode{ get; private set; }

        /// <summary>
        /// 头像
        /// </summary>
        /// <value>The avatar.</value>
		public string avatar {get;private set;}

        /// <summary>
        /// 用户的登陆类型，
        /// 包括 游客登陆、 facebook 登陆
        /// </summary>
        /// <value>The type of the login.</value>
        public PVP.LoginType loginType {
            get;
            private set;
        }

		public bool isCountryCodeChangeable {
			get;
			private set;
		}

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="userInfo">User info.</param>
		internal Player (PVPProtobuf_Token.User_LoginOrRegister_Response userInfo)
        {
            this.playerId = userInfo.player_id;
            this.name = userInfo.name;
            this.loginType = (PVP.LoginType)userInfo.login_type;
            this.countryCode = userInfo.country_code;
			this.avatar = userInfo.avatar;
			this.isCountryCodeChangeable = userInfo.is_country_code_changeable;
        }

		internal Player (PVPProtobuf_Token.User_AutoLogin_Response userInfo)
        {
            this.playerId = userInfo.player_id;
            this.name = userInfo.name;
            this.loginType = (PVP.LoginType)userInfo.login_type;
            this.countryCode = userInfo.country_code;
			this.avatar = userInfo.avatar;
			this.isCountryCodeChangeable = userInfo.is_country_code_changeable;
        }

        /// <summary>
        /// 进行登出操作，清除本地用户数据
        /// </summary>
        public void OpLoginOut ()
        {
            PVPGlobal.lobby = null;
            PVPGlobal.room = null;
            PVPGlobal.player = null;
            PVPGlobal.localAppPlayer = null;
            PVPGlobal.appPlayerInfos.Clear ();
            PVP.ICM.gameServerClient.Close ();
			PVPClient.AutoLoginInfo autoLoginInfo = PVP.Toolkit.LoadFile<PVPClient.AutoLoginInfo> (PVP.GameServerClient.AUTO_LOGIN_FILE_NAME);
			if (autoLoginInfo != null) {
				autoLoginInfo.auto_login = false;
				PVP.Toolkit.SaveFile (PVP.GameServerClient.AUTO_LOGIN_FILE_NAME, autoLoginInfo);
			}
        }

        /// <summary>
        /// 调用自动登录
        /// </summary>
        /// <returns><c>true</c>, if auto login was oped, <c>false</c> otherwise.</returns>
		public static bool OpAutoLogin (int connectTimeout = 5, int timeout = 5)
        {
            PVPGlobal.player = null;
			return PVP.ICM.gameServerClient.AutoLogin (connectTimeout, timeout);
        }

        /// <summary>
        /// 用户登录，如果用户不存在，那么先注册后登陆
        /// 支持游客登陆和 facebook 登陆，
        /// 如果使用 facebook 账号登陆，需要先行加载调用facebook sdk 的用户登陆功能，并传入facebook 的用户 access token
        /// facebook sdk for unity3d 可在facebook 开发者网站下载
        /// </summary>
        /// <param name="loginType">登录类型， 支持游客登录 和 facebook 账号登陆</param>
        /// <param name="account">账号，暂时不用.</param>
        /// <param name="password">密码，暂时不用.</param>
        /// <param name="facebookAccessToken">Facebook access token.</param>
        /// <param name="timeout">响应超时时间.</param>
		public static void OpLoginOrRegister (PVP.LoginType loginType, string account = "", string password = "", string facebookAccessToken = "", long facebookExpirationTime = 0, int connectTimeout = 5, int timeout = 5)
        {
            PVPGlobal.player = null;
			PVP.ICM.gameServerClient.LoginOrRegister (loginType, account, password, facebookAccessToken, facebookExpirationTime, connectTimeout, timeout);
        }

        /// <summary>
        /// 当用户登陆成功后，连接游戏服务器
        /// 连接游戏服务器成功后，系统会回调 PVPGlobal.handlerRegister.connectedToGameServerEventHandler  委托实例，并创建 PVPGlobal.localAppUser 实例
        /// </summary>
        /// <returns><c>true</c> 可以进行连接 <c>false</c> 连接失败.</returns>
        public PVP.ConnectServerOptionalResult OpConnectGameServer ()
        {
            return PVP.ICM.gameServerClient.ConnectServer ();
        }

    }
}