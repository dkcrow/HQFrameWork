using System;
using System.Collections;
using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk.PVP {
    public class UpdateAppPlayerInfoResult {
        public int level {
            get;
            private set;
        }

        public Int32 score {
            get;
            private set;
        }

        public int winTimes {
            get;
            private set;
        }
        public int loseTimes {
            get;
            private set;
        }

        public uint number { get; private set; }

        public uint customDataNumber{ get; private set;}

        public int commandId {
            get;
            private set;
        }


        public Dictionary<string, byte[]> updatedData {
            get;private set;
        }

        public List<string> deletedData { get; private set; }

		internal UpdateAppPlayerInfoResult(int errorCode, PVPProtobuf.AppPlayer_UpdateInfo_Response r){
            this.commandId = r.command_id;
            this.level = r.level;
            this.score = r.score;
            this.winTimes = r.win_times;
            this.loseTimes = r.lose_times;
            this.number = r.number;
            this.customDataNumber = r.custom_data_number;
            this.updatedData = new Dictionary<string, byte[]> ();
            this.deletedData = new List<string> ();
            if (errorCode == ErrorCode.SUCCESS) {
                foreach (Pair item in r.updated_data) {
                    this.updatedData [item.key] = item.value;
                }
                this.deletedData.AddRange (r.deleted_data);
            } else {
                foreach (Pair item in r.check_data) {
                    this.updatedData [item.key] = item.value;
                }
                this.deletedData.AddRange (r.check_data_not_exist);
            }
        }

    }
}

