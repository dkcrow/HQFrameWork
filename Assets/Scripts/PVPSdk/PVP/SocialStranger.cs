using System;

namespace PVPSdk
{
    /// <summary>
    /// 陌生人
    /// </summary>
    public class SocialStranger
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

		internal SocialStranger(PVPProtobuf_Token.Social_Stranger ss){
            this.playerId = ss.player_id;
            this.name = ss.name;
            this.avatar = ss.avatar;
        }

		internal SocialStranger(PVPProtobuf.Social_Stranger ss){
            this.playerId = ss.player_id;
            this.name = ss.name;
            this.avatar = ss.avatar;
        }
    }
}

