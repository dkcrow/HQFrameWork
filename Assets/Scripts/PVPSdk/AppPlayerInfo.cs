using System;

using System.Collections.Generic;
using PVPProtobuf;

namespace PVPSdk
{

//
//    public class AppUserCustomDataMeta{
//        public Dictionary<string, byte[]> _data {
//            get;
//            private set;
//        }
//    }

    /// <summary>
    /// 应用里面的用户信息
    /// 其他玩家的信息也是用这个类
    /// </summary>
    public class AppPlayerInfo {
        /// <summary>
        /// 玩家 uid
        /// </summary>
        /// <value>The uid.</value>
        public UInt64 playerId {
            get;
            protected set;
        }

        /// <summary>
        /// 内部使用，忽略
        /// </summary>
        /// <value>The custom data number.</value>
        internal uint customDataNumber {
            get;
            private set;
        }
        /// <summary>
        /// 玩家在游戏中的自定义内容
        /// </summary>
        /// <value>The custom data.</value>
        public Dictionary<string, byte[]> customData {
            get;
            protected set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <value>The name.</value>
        public string name {
            get;
            protected set;
        }

        /// <summary>
        /// 头像地址
        /// </summary>
        /// <value>The avatar.</value>
        public string avatar {
            get;
            protected set;
        }
        /// <summary>
        /// 等级，等级的更新必须递增，不能减少
        /// </summary>
        /// <value>The level.</value>
        public int level {
            get;
            protected set;
        }

        /// <summary>
        /// 积分，积分的更新必须递增，不能减少
        /// </summary>
        /// <value>The score.</value>
        public Int32 score {
            get;
            protected set;
        }

        /// <summary>
        /// 胜利次数。胜利次数的更新必须递增，不能减少
        /// </summary>
        /// <value>The window times.</value>
        public int winTimes{
            get;
            protected set;
        }

        /// <summary>
        /// 失败次数。失败次数的更新必须递增，不能减少
        /// </summary>
        /// <value>The lose times.</value>
        public int loseTimes {
            get;
            protected set;
        }

        /// <summary>
        /// 更新次数，内部使用，忽略
        /// </summary>
        /// <value>The number.</value>
        internal UInt32 number {
            get;
            private set;
        }
        public string countryCode{
            get;
            private set;
        }
        
        public PVP.LoginType loginType {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PVPSdk.AppUserInfo"/> class.
        /// </summary>
        internal AppPlayerInfo (PVPProtobuf.AppPlayerInfo u) {
            this.playerId = u.player_id;
            this.avatar = u.avatar;
            this.level = u.level;
            this.score = u.score;
            this.loseTimes = u.lose_times;
            this.winTimes = u.win_times;
            this.name = u.name;
            this.number = u.number;
            this.customDataNumber = u.custom_data_number;
            this.countryCode = u.country_code;
            this.customData = new Dictionary<string, byte[]> ();
            foreach (PVPProtobuf.Pair item in u.custom_data) {
                this.customData [item.key] = item.value;
            }
            this.countryCode = u.country_code;
            if (u.app_login_type == 1) {
                this.loginType = PVP.LoginType.Account;
            } else if (u.app_login_type == 2) {
                this.loginType = PVP.LoginType.Guest;
            } else if (u.app_login_type == 3) {
                this.loginType = PVP.LoginType.Facebook;
            } else {
                this.loginType = PVP.LoginType.Unknown;
            }
        }

        internal AppPlayerInfo(PVPProtobuf_Token.AppPlayerInfo u) {
            this.playerId = u.player_id;
            this.name = u.name;
            this.avatar = u.avatar;
            this.level = u.level;
            this.score = u.score;
            this.loseTimes = u.lose_times;
            this.winTimes = u.win_times;
            this.number = u.number;
            this.customDataNumber = u.custom_data_number;
            this.customData = new Dictionary<string, byte[]> ();
            foreach (PVPProtobuf_Token.Pair item in u.custom_data) {
                this.customData [item.key] = item.value;
            }
            this.countryCode = u.country_code;
            if (u.app_login_type == 1) {
                this.loginType = PVP.LoginType.Account;
            } else if (u.app_login_type == 2) {
                this.loginType = PVP.LoginType.Guest;
            } else if (u.app_login_type == 3) {
                this.loginType = PVP.LoginType.Facebook;
            } else {
                this.loginType = PVP.LoginType.Unknown;
            }
        }

        internal void FillAppUserInfo (PVPProtobuf.AppPlayerInfo u){
            this.playerId = u.player_id;
            this.avatar = u.avatar;
            this.level = u.level;
            this.score = u.score;
            this.loseTimes = u.lose_times;
            this.winTimes = u.win_times;
            this.name = u.name;
            this.number = u.number;
            this.countryCode = u.country_code;
            this.customDataNumber = u.custom_data_number;
            this.customData.Clear ();
            foreach (PVPProtobuf.Pair item in u.custom_data) {
                this.customData [item.key] = item.value;
            }

            if (u.app_login_type == 1) {
                this.loginType = PVP.LoginType.Account;
            } else if (u.app_login_type == 2) {
                this.loginType = PVP.LoginType.Guest;
            } else if (u.app_login_type == 3) {
                this.loginType = PVP.LoginType.Facebook;
            } else {
                this.loginType = PVP.LoginType.Unknown;
            }
        }

        internal void OnUpdateCustomData (uint customDataNumber,  List<Pair> updatedData, List<string> deletedData){
            this.customDataNumber = customDataNumber;
            for (int i = 0; i < updatedData.Count; i++) {
                this.customData [updatedData [i].key] = updatedData [i].value;
            }

            for (int i = 0; i > deletedData.Count; i++) {
                if (this.customData.ContainsKey (deletedData [i])) {
                    this.customData.Remove (deletedData [i]);
                }
            }
        }

        internal void OnUpdateInfo(PVP.UpdateAppPlayerInfoResult r){
            if (r.level >= 0) {
                this.level = r.level;
            }
            if (r.score >= 0) {
                this.score = r.score;
            }

            if (r.winTimes >= 0) {
                this.winTimes = r.winTimes;
            }

            if (r.loseTimes >= 0) {
                this.loseTimes = r.loseTimes;
            }

            if (r.number > 0) {
                this.number = r.number;
            }

            if (r.customDataNumber > 0) {
                this.customDataNumber = r.customDataNumber;
            }

            foreach(KeyValuePair<string, byte[]> item in r.updatedData){
                this.customData [item.Key] = item.Value;
            }

            for (int i = 0; i > r.deletedData.Count; i++) {
                if (this.customData.ContainsKey (r.deletedData [i])) {
                    this.customData.Remove (r.deletedData [i]);
                }
            }
        }
    }
}

