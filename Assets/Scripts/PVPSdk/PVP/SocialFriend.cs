using System;

namespace PVPSdk.PVP
{
    /// <summary>
    /// 好友来源
    /// </summary>
    public enum SocialSourceFromType{
        /// <summary>
        /// 游戏内部添加的好友
        /// </summary>
        _internal = 1,
        /// <summary>
        /// 来源于facebook 的好友
        /// </summary>
        facebook = 3,
    }

    /// <summary>
    /// 好友类
    /// </summary>
    public class SocialFriend
    {
        public UInt64 playerId {
            get;
            private set;
        }

        /**
         * facebook 平台的uid
         */
        public string facebookUid {
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

        public SocialSourceFromType sourceFrom {
            get;
            private set;
        }

        public string playerInfoAccessToken{
            get;
            private set;
        }

		internal SocialFriend(PVPProtobuf_Token.Social_Friend socialFriend){
            this.playerId = socialFriend.player_id;
            this.name = socialFriend.name;
            this.facebookUid = socialFriend.facebook_uid;
            this.name = socialFriend.name;
            this.avatar = socialFriend.avatar;
            this.sourceFrom = (SocialSourceFromType) socialFriend.source_from;
        }

		internal SocialFriend(PVPProtobuf.Social_Friend socialFriend){
            this.playerId = socialFriend.player_id;
            this.name = socialFriend.name;
            this.facebookUid = socialFriend.facebook_uid;
            this.name = socialFriend.name;
            this.avatar = socialFriend.avatar;
            this.sourceFrom = (SocialSourceFromType) socialFriend.source_from;
            this.playerInfoAccessToken = socialFriend.player_info_access_token;
        }
    }
}

