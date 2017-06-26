using System;

namespace PVPSdk
{
    public class PlayerMiniInfo
    {
        public UInt64 playerId { get; private set;}

        public string name { get; private set;}
        public string avatar { get; private set;}
        public Int32 level { get; private set;}


		internal PlayerMiniInfo(PVPProtobuf.PlayerMiniInfo u){
            this.playerId = u.player_id;
            this.name = u.name;
            this.avatar = u.avatar;
            this.level = u.level;
        }
    }
}

