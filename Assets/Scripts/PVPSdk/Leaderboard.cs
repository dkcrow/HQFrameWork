using System;

namespace PVPSdk {
    public class Leaderboard {
        /// <summary>
        /// 向排行榜提交分数
        /// </summary>
        /// <returns><c>true</c>, if leaderboard submit score was oped, <c>false</c> otherwise.</returns>
        /// <param name="id"> 排行榜的id，服务器支持多个排行榜，例如游戏由两个活动，各创建了一个排行榜，那么游戏上传分数是就需要标示提交到哪个排行榜
        /// 排行榜 id 需要在服务器后台配置 .</param>
        /// <param name="score">积分.</param>
        /// <param name="updateType">增量更新 还是 全量更新</param>
        /// <param name="submitMark">主要针对于增量更新，增量更新时，为了防止重复提交同一次的分数，累加多次，服务器会记录上次提交的 submitMark，
        /// 如果 submitMark 连续两次相同，会不做更新（服务器已经在上次更新积分成功了），并返回响应错误码</param>
        /// <param name="timeout">Timeout.</param>
		public static bool OpLeaderboardSubmitScore(string id, UInt32 score, PVPSdk.Leaderboards.UpdateType updateType, int submitMark = 0, int timeout = 5){
            if (!PVPGlobal.isLogined) {
                return false;
            }
			return PVP.ICM.gameServerClient.LeaderboardSubmitScore (id, score, updateType, submitMark, timeout);
        }

        /// <summary>
        /// 获取排行榜
        /// 服务器会对每个排行榜（服务器已经配置的排行榜 id）进行 空间维度（spatialDimension） 和 时间维度 （timeDimension） 的细分，
        /// 所以获取排行榜时，需要指定具体的空间维度（spatialDimension） 和 时间维度 （timeDimension）
        /// </summary>
        /// <returns><c>true</c>, if leaderboard get list was oped, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="spatialDimension">Spatial dimension.</param>
        /// <param name="timeDimension">Time dimension.</param>
        /// <param name="timeout">Timeout.</param>
		public static bool OpLeaderboardGetList(string id, PVPSdk.Leaderboards.SpatialDimension spatialDimension , PVPSdk.Leaderboards.TimeDimension timeDimension, int offset = -1, int length = -1, int connectTimeut = 5, int timeout = 5){
            if (!PVPGlobal.isLogined) {
                return false;
            }
			return PVP.ICM.gameServerClient.LeaderboardGetList(id, spatialDimension, timeDimension, offset, length, connectTimeut,  timeout );
        }
    }
}

