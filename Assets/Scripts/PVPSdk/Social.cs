using System;

using PVPProtobuf_Token;
using UnityEngine;

namespace PVPSdk
{
    /// <summary>
    /// 社交
    /// </summary>
    public class Social
    {
        /// <summary>
        /// 获取好友列表
        /// </summary>
        /// <returns><c>true</c>, if get friend list was oped, <c>false</c> otherwise.</returns>
        public static bool OpGetFriendList(){
            return PVP.ICM.gameServerClient.SocialGetFriendList ();
        }

        /// <summary>
        /// 查询陌生人
        /// </summary>
        /// <returns><c>true</c>, if search stranger was oped, <c>false</c> otherwise.</returns>
        /// <param name="searchString">Search string.</param>
        /// <param name="timeout">Timeout.</param>
        public static bool OpSearchStranger(string searchString,int timeout = 5){
            return PVP.ICM.gameServerClient.SocialSearchStranger (searchString, timeout);
        }
        /// <summary>
        /// 想其他玩家发送添加好友请求
        /// </summary>
        /// <returns><c>true</c>, if add friend was oped, <c>false</c> otherwise.</returns>
        /// <param name="toPlayerId">To player identifier.</param>
        public static bool OpAddFriend (UInt64 toPlayerId)
        {
            return PVP.ICM.gameServerClient.SocialAddFriend (toPlayerId);
        }

        /// <summary>
        /// 收到陌生人添加请求后，同意或者拒绝请求
        /// </summary>
        /// <returns><c>true</c>, if accept friend was oped, <c>false</c> otherwise.</returns>
        /// <param name="fromPlayerId">From player identifier.</param>
        /// <param name="isAgreed">If set to <c>true</c> is agreed.</param>
        public static bool OpAcceptFriend(UInt64 fromPlayerId, bool isAgreed){
            return PVP.ICM.gameServerClient.SocialAcceptFriend(fromPlayerId, isAgreed);
        }

        /// <summary>
        /// 主动获取收到的陌生人添加好友请求
        /// </summary>
        /// <returns><c>true</c>, if get friend need determined was oped, <c>false</c> otherwise.</returns>
        public static bool OpGetFriendNeedDetermined(){
            return PVP.ICM.gameServerClient.SocialGetFriendNeedDetermined();
        }


        /// <summary>
		/// 提交邀请对战申请
        /// </summary>
        /// <returns><c>true</c>, if submit challenge was oped, <c>false</c> otherwise.</returns>
        /// <param name="lobbyId">Lobby identifier.</param>
        /// <param name="challengePlayerId">Challenge player identifier.</param>
        public static bool OpSubmitChallenge(int lobbyId, UInt64 challengePlayerId){
            return PVP.ICM.gameServerClient.SocialSubmitChallenge(lobbyId, challengePlayerId);
        }

		/// <summary>
		/// 接收到对手的邀请信息后，进行同意或者拒绝操作
		/// </summary>
		/// <returns><c>true</c>, if take up challenge was oped, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="isTakeUped">If set to <c>true</c> is take uped.</param>
		public static bool OpTakeUpChallenge(string key, bool isTakeUped){
			return PVP.ICM.gameServerClient.SocialTakeUpChallenge( key,  isTakeUped);
		}

        /// <summary>
        /// 根据playerInfoAccessToken 查询其他玩家的信息
        /// </summary>
        /// <returns><c>true</c>, if get app player info was oped, <c>false</c> otherwise.</returns>
        /// <param name="playerInfoAccessToken">Player info access token.</param>
        public static bool OpGetAppPlayerInfo(string playerInfoAccessToken, int timeout = 5) {
            return PVP.ICM.gameServerClient.SocialGetAppInfo (playerInfoAccessToken, timeout);
        }

		/// <summary>
		/// 发出对战邀请或者同意对方的邀请对战申请后，取消已经存在的申请
		/// </summary>
		/// <returns><c>true</c>, if cancel challenge was oped, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		public static bool OpCancelChallenge(string key){
			return PVP.ICM.gameServerClient.SocialCancelChallenge(key);
		}
    }
}

