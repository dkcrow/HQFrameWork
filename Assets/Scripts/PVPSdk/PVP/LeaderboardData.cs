using System;
using System.Collections.Generic;

namespace PVPSdk.Leaderboards
{
    /// <summary>
    /// 排行榜的地区维度
    /// 可以是国家、好友、全球
    /// </summary>
    public enum SpatialDimension
    {
        Country = 1,
        Friends = 2,
        World = 3,

    }

    /// <summary>
    /// 排行榜时间维度
    /// 可以是永久的、上周的
    /// </summary>
    public enum TimeDimension
    {
        LastWeek = 1,
        AllTime = 2,
        ThisWeek = 3,
    }

    /// <summary>
    /// 提交积分的方式
    /// 一种是全量更新，例如服务器上用户的积分是1000分，提交 1100 分，那么就更新为 1100分
    /// 另一种是增量更新，例如服务器上用户的积分是1000分，提交 1100 分，那么就更新为 2100分
    /// </summary>
    public enum UpdateType
    {
        /// <summary>
        /// 全量更新
        /// </summary>
        TotalScore = 1,
        /// <summary>
        /// 增量更新
        /// </summary>
        AddScore = 2,
    }

    /// <summary>
    /// 排行榜的标示，
    /// 包括 排行榜名称
    /// 区域维度
    /// 时间维度
    /// </summary>
    public class Meta
    {
        public string id {
            get;
            private set;
        }

        public SpatialDimension spatialDimension {
            get;
            private set;
        }

        public TimeDimension timeDimension {
            get;
            private set;
        }

//        public Meta (PVPProtobuf.Leaderboard.Meta meta)
//        {
//            this.id = meta.id;
//            this.spatialDimension = (PVPSdk.Leaderboards.SpatialDimension)meta.spatial_dimension;
//            this.timeDimension = (PVPSdk.Leaderboards.TimeDimension)meta.time_dimension;
//        }

		internal Meta (PVPProtobuf_Token.Leaderboard_Meta meta)
        {
            this.id = meta.id;
            this.spatialDimension = (PVPSdk.Leaderboards.SpatialDimension)meta.spatial_dimension;
            this.timeDimension = (PVPSdk.Leaderboards.TimeDimension)meta.time_dimension;
        }
    }

    public class LeaderboardMemberinfo
    {
        public UInt64 playerId {
            get;
            private set;
        }

        public string name {
            get;
            private set;
        }

        public string avatar {
            get;
            private set;
        }

        public Int32 score {
            get;
            private set;
        }

        public UInt32 order {
            get;
            private set;
        }

        public string countryCode {
            get;
            private set;
        }

        public Int32 level {
            get;
            private set;
        }

        public Int32 winTimes {
            get;
            private set;
        }

        public Int32 loseTimes {
            get;
            private set;
        }

        public string playerInfoAccessToken {
            get;
            private set;
        }

        internal LeaderboardMemberinfo (PVPProtobuf_Token.Leaderboard_Memberinfo m)
        {
            this.playerId = m.player_id;
            this.name = m.name;
            this.avatar = m.avatar;
            this.score = m.score;
            this.order = m.order;
            this.countryCode = m.country_code;
            this.level = m.level;
            this.winTimes = m.win_times;
            this.loseTimes = m.lose_times;
            this.playerInfoAccessToken = m.player_info_access_token;
        }
    }

    /// <summary>
    /// 排行榜结果，
    /// 列表包括前10名数据，以及玩家自己的数据
    /// </summary>
    public class LeaderboardData
    {
        public Meta meta {
            get;
            private set;
        }

        public int offset{
            get;
            private set;
        }

        public int nextOffset{
            get;
            private set;
        }

        public int remainingTime{
            get;
            private set;
        }


        /// <summary>
        /// 列表包括前10名数据，以及玩家自己的数据，列表排序由高到低
        /// </summary>
        /// <value>The list.</value>
        public List<LeaderboardMemberinfo> list { get; private set; }

		internal LeaderboardData (PVPProtobuf_Token.Leaderboard_GetList_Response r)
        {
            this.meta = new Meta (r.meta);
            this.list = new List<LeaderboardMemberinfo> ();
            this.offset = r.offset;
            this.nextOffset = r.next_offset;
            this.remainingTime = r.remaining_time;
            foreach (PVPProtobuf_Token.Leaderboard_Memberinfo m in r.member_infos) {
                this.list.Add (new LeaderboardMemberinfo (m));   
            }
        }
    }
}

