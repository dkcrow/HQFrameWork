using System;
using PVPProtobuf;
using System.Collections;
using System.Collections.Generic;

namespace PVPSdk
{
    /// <summary>
    /// 当前玩家的游戏信息
    /// </summary>
    public class LocalAppPlayer : AppPlayerInfo
    {
		internal LocalAppPlayer(PVPProtobuf.AppPlayerInfo p) : base(p){
        }
		internal LocalAppPlayer(PVPProtobuf_Token.AppPlayerInfo p) : base(p){
        }


        /// <summary>
        /// 获取游戏服务器的游戏大厅列表，不建议使用
        /// </summary>
        /// <returns><c>true</c>, if get lobby list was oped, <c>false</c> otherwise.</returns>
        /// <param name="timeout">Timeout.</param>
        public bool OpGetLobbyList (int timeout = 5){
            return PVP.ICM.gameServerClient.GetLobbyList ();
        }

        /// <summary>
        /// 进入大厅，不建议使用
        /// </summary>
        /// <returns><c>true</c>, if lobby was entered, <c>false</c> otherwise.</returns>
        public bool OpEnterLobby(int lobbyId, int timeout = 5){
            return PVP.ICM.gameServerClient.EnterLobby (lobbyId, timeout);
        }

        /// <summary>
        /// 更新游戏内的用户信息，更新描述
        /// 功能与 OpUpdateAppUserCustomData 类似，增加了更新 level、score、winTimes、loseTimes 字段
        /// </summary>
        /// <returns><c>true</c>, if update app user info was oped, <c>false</c> otherwise.</returns>
        /// <param name="level">等级.</param>
        /// <param name="score">积分.</param>
        /// <param name="winTimes">胜利次数.</param>
        /// <param name="loseTimes">失败次数.</param>
        /// <param name="beingUpdatedData">要更新的自定义信息.</param>
        /// <param name="beingDeletedData">要删除的自定义信息.</param>
        /// <param name="checkData">在进行更新操作前，服务器会比较checkData 的内容，可以为NULL。 如果 checkData 的内容和服务器存放的内容不一致，验证不通过.</param>
        /// <param name="checkDataNotExits">在进行更新操作前，服务器会检测 checkDataNotExits 列表，可以为NULL。如果服务器存在 checkDataNotExits 列表的任意一个字段，验证不通过</param>
        public bool OpUpdateAppUserInfo (int commandId, int level = -1, Int32 score = -1, int winTimes = -1, int loseTimes = -1, Dictionary<String, byte[]> beingUpdatedData = null, List<string> beingDeletedData = null, Dictionary<String, byte[]> checkData = null, List<string> checkDataNotExits = null){
            if (level < this.level && score < this.score && winTimes < this.winTimes && loseTimes < this.loseTimes && (beingDeletedData == null || beingDeletedData.Count == 0) && (beingUpdatedData == null || beingUpdatedData.Count == 0)) {
                return false;
            }

            return PVP.ICM.gameServerClient.UpdateAppUserInfo (commandId, level, score, winTimes, loseTimes, beingUpdatedData, beingDeletedData,checkData, checkDataNotExits, 5);
        }

        /// <summary>
        /// 更新用户的自定义数据 custom data
        /// custom data 是一个 hash 字典表，key：string， value: byte[]
        /// 使用场景：例如可以在 custom data 里面存放玩家在游戏里面的“彩币”数量、“比赛平均时长”等
        /// 
        /// checkData 和 checkDataNotExits 用来做数据验证，
        /// 更新成功或者失败的结果说明请查看 PVPGloabl.handlerRegister.appUserUpdateCustomDataResponseHandler 描述
        /// </summary>
        /// <returns><c>true</c>, if update app user custom data was oped, <c>false</c> otherwise.</returns>
        /// <param name="beingUpdatedData">要更新的内容.</param>
        /// <param name="beingDeletedData">要删除的字段列表.</param>
        /// <param name="checkData">在进行更新操作前，服务器会比较checkData 的内容，可以为NULL。 如果 checkData 的内容和服务器存放的内容不一致，验证不通过.</param>
        /// <param name="checkDataNotExits">在进行更新操作前，服务器会检测 checkDataNotExits 列表，可以为NULL。如果服务器存在 checkDataNotExits 列表的任意一个字段，验证不通过</param>
        public bool OpUpdateAppUserCustomData (int commandId, Dictionary<String, byte[]> beingUpdatedData = null, List<string> beingDeletedData = null, Dictionary<String, byte[]> checkData = null, List<string> checkDataNotExits = null)
        {
            if (PVPGlobal.localAppPlayer == null) {
                return false;
            }
            if ((beingDeletedData == null || beingDeletedData.Count == 0) && (beingUpdatedData == null || beingUpdatedData.Count == 0)) {
                return false;
            }
            return PVP.ICM.gameServerClient.UpdateAppUserCustomData (commandId, beingUpdatedData, beingDeletedData, checkData, checkDataNotExits, 5);
        }
    }
}

