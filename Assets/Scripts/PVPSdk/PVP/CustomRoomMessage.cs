using System;


namespace PVPSdk.PVP
{
    public class RoomNewMessage
    {
        public UInt64 fromPlayerId {
            get;
            private set;
        }

        public int customCommandId {
            get;
            private set;
        }

        public byte[] message {
            get;
            private set;
        }

		internal RoomNewMessage(UInt64 playerId, PVPProtobuf.Room_NewMessage_Request meta){
            this.fromPlayerId = playerId;
            this.customCommandId = meta.custom_command_id;
            this.message = meta.message;
        }

		internal RoomNewMessage (PVPProtobuf.Room_NewMessage_Broadcast meta) {
            this.fromPlayerId = meta.from_player_id;
            this.customCommandId = meta.custom_command_id;
            this.message = meta.message;
        }
    }
}

