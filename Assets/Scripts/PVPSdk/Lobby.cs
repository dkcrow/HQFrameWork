using System;

namespace PVPSdk
{
    /// <summary>
    /// 大厅
    /// </summary>
    public class Lobby
    {
         /// <summary>
         /// 大厅的id， 后台需要先行配置好大厅数据，大厅不支持客户端创建
         /// </summary>
         /// <value>The lobby identifier.</value>
        public int lobbyId {
            get;
            private set;
        }

        /// <summary>
        /// 大厅的名称, please do not use it any more
        /// </summary>
        /// <value>The name.</value>
        public string name {
            get;
            private set;
        }

		internal Lobby (int lobbyId, string name)
        {
            this.lobbyId = lobbyId;
            this.name = name;
        }

		internal Lobby(PVPProtobuf.LobbyInfo lobbyInfo){
            this.lobbyId = lobbyInfo.lobby_id;
            this.name = lobbyInfo.name;
        }

        internal void Set(int lobbyId, string name){
            this.lobbyId = lobbyId;
            this.name = name;
        }


        /// <summary>
        /// 建议使用 OpMatchOpponentWithLobbyId 更加方便
        /// </summary>
        /// <returns><c>true</c>, if match opponent was oped, <c>false</c> otherwise.</returns>
        public bool OpMatchOpponent(int standard, int rangeDown = -1, int rangeUp = -1, int waitForMatch=0, int clusterId=0, int versionId=0){
            return PVP.ICM.gameServerClient.MatchOpponent (this.lobbyId, standard, rangeDown, rangeUp, waitForMatch, clusterId, versionId, 5);
        }


        /// <summary>
        /// 玩家匹配对手玩家
        /// </summary>
        /// <returns><c>true</c>, if match opponent was oped, <c>false</c> otherwise.</returns>
        /// <param name="lobbyId">大厅编号</param>
        /// <param name="standard">匹配的基准.</param>
        /// <param name="rangeDown">匹配的下线，-1 表示无下限.</param>
        /// <param name="rangeUp">匹配的上线，-1 表示无上限.</param>
        /// <param name="waitForMatch">匹配的等待时间.</param>
        /// <param name="clusterId">匹配的分类，例如游戏设定角色有 “男” “女”，只能“男女” 匹配，不允许 “男男”、“女女”匹配，那么可以设定 “男” 1， “女“ 2，匹配时提交clusterId 1（男） 或者 2 （女）进行分类
        /// 默认值 0，表示可以和其他所有类匹配
        /// </param>
        /// <param name="versionId">匹配的版本号，主要用于：游戏升级后，新版本不能喝旧版本对战，那么可以使用心得versionId 进行分割</param>
        public static bool OpMatchOpponentWithLobbyId(int lobbyId, int standard, int rangeDown = -1, int rangeUp = -1, int waitForMatch=0, int clusterId=0, int versionId= 0){
            return PVP.ICM.gameServerClient.MatchOpponent (lobbyId, standard, rangeDown, rangeUp, waitForMatch, clusterId, versionId, 5);
        }

        /// <summary>
        /// 取消匹配对手玩家
        /// </summary>
        /// <returns><c>true</c>, if cancel match opnnent was oped, <c>false</c> otherwise.</returns>
        /// <param name="timeout">Timeout.</param>
        public bool OpCancelMatchOpnnent (int timeout = 5){
            return PVP.ICM.gameServerClient.CancelMatchOpnnent (timeout);
        }

    }
}

